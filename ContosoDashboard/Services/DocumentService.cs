using System.Text.RegularExpressions;
using ContosoDashboard.Data;
using ContosoDashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoDashboard.Services;

public interface IDocumentService
{
    Task<DocumentValidationResult> ValidateUploadAsync(string fileName, long fileSizeInBytes, string title, string category, int? projectId = null, int? taskId = null);
    Task<Document?> GetDocumentByIdAsync(int documentId, int requestingUserId);
    Task<List<Document>> GetVisibleDocumentsAsync(int requestingUserId, string? searchTerm = null, string? category = null, int? projectId = null);
    Task<Document> UploadAsync(Document document, Stream fileStream, int requestingUserId);
    Task<bool> DeleteAsync(int documentId, int requestingUserId);
}

public class DocumentUploadValidator
{
    private const long MaxFileSizeBytes = 25L * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".jpg", ".jpeg", ".png"
    };

    public static DocumentValidationResult Validate(string fileName, long fileSizeInBytes, string title, string category)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return DocumentValidationResult.Invalid("Please choose a file to upload.");
        }

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            return DocumentValidationResult.Invalid("Unsupported file type. Please upload a supported document such as PDF, Word, Excel, PowerPoint, text, JPEG, or PNG.");
        }

        if (fileSizeInBytes <= 0)
        {
            return DocumentValidationResult.Invalid("The selected file is empty.");
        }

        if (fileSizeInBytes > MaxFileSizeBytes)
        {
            return DocumentValidationResult.Invalid("The file exceeds the 25 MB upload limit.");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return DocumentValidationResult.Invalid("Document title is required.");
        }

        if (title.Trim().Length > 255)
        {
            return DocumentValidationResult.Invalid("Document title must be 255 characters or fewer.");
        }

        if (!Document.IsValidCategory(category))
        {
            return DocumentValidationResult.Invalid("Please select a valid document category.");
        }

        return DocumentValidationResult.Valid();
    }
}

public class DocumentValidationResult
{
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    public static DocumentValidationResult Valid() => new() { IsValid = true };
    public static DocumentValidationResult Invalid(string errorMessage) => new() { ErrorMessage = errorMessage };
}

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public DocumentService(ApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<DocumentValidationResult> ValidateUploadAsync(string fileName, long fileSizeInBytes, string title, string category, int? projectId = null, int? taskId = null)
    {
        var validation = DocumentUploadValidator.Validate(fileName, fileSizeInBytes, title, category);
        if (!validation.IsValid)
        {
            return validation;
        }

        if (projectId.HasValue)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectId.Value);
            if (project == null)
            {
                return DocumentValidationResult.Invalid("The selected project could not be found.");
            }
        }

        if (taskId.HasValue)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId.Value);
            if (task == null)
            {
                return DocumentValidationResult.Invalid("The selected task could not be found.");
            }
        }

        return validation;
    }

    public async Task<Document?> GetDocumentByIdAsync(int documentId, int requestingUserId)
    {
        var document = await _context.Documents
            .Include(d => d.UploadedByUser)
            .Include(d => d.Project)
            .Include(d => d.Shares)
            .Include(d => d.Activities)
            .FirstOrDefaultAsync(d => d.DocumentId == documentId && !d.IsDeleted);

        if (document == null)
        {
            return null;
        }

        var isOwner = document.UploadedByUserId == requestingUserId;
        var isProjectManager = document.Project != null && document.Project.ProjectManagerId == requestingUserId;
        var isProjectMember = document.Project != null && document.Project.ProjectMembers.Any(pm => pm.UserId == requestingUserId);
        var isSharedUser = document.Shares.Any(s => s.IsActive && s.SharedWithUserId == requestingUserId);
        var isAdmin = await _context.Users.AnyAsync(u => u.UserId == requestingUserId && u.Role == UserRole.Administrator);

        if (!isOwner && !isProjectManager && !isProjectMember && !isSharedUser && !isAdmin)
        {
            return null;
        }

        return document;
    }

    public async Task<List<Document>> GetVisibleDocumentsAsync(int requestingUserId, string? searchTerm = null, string? category = null, int? projectId = null)
    {
        var user = await _context.Users
            .Include(u => u.ProjectMemberships)
            .FirstOrDefaultAsync(u => u.UserId == requestingUserId);

        if (user == null)
        {
            return new List<Document>();
        }

        var query = _context.Documents
            .Include(d => d.UploadedByUser)
            .Include(d => d.Project)
            .Where(d => !d.IsDeleted)
            .AsQueryable();

        var projectIds = new List<int>();
        if (user.ProjectMemberships.Any())
        {
            projectIds = user.ProjectMemberships.Select(pm => pm.ProjectId).ToList();
        }

        query = query.Where(d =>
            d.UploadedByUserId == requestingUserId ||
            (d.ProjectId != null && (d.Project.ProjectManagerId == requestingUserId || projectIds.Contains(d.ProjectId.Value))) ||
            d.Shares.Any(s => s.IsActive && s.SharedWithUserId == requestingUserId) ||
            d.Shares.Any(s => s.IsActive && s.SharedWithProjectId != null && projectIds.Contains(s.SharedWithProjectId.Value)) ||
            user.Role == UserRole.Administrator);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalized = searchTerm.Trim();
            query = query.Where(d =>
                d.Title.Contains(normalized) ||
                (d.Description != null && d.Description.Contains(normalized)) ||
                (d.Tags != null && d.Tags.Contains(normalized)) ||
                d.UploadedByUser.DisplayName.Contains(normalized) ||
                (d.Project != null && d.Project.Name.Contains(normalized)));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(d => d.Category == category);
        }

        if (projectId.HasValue)
        {
            query = query.Where(d => d.ProjectId == projectId.Value || d.Shares.Any(s => s.IsActive && s.SharedWithProjectId == projectId.Value));
        }

        return await query
            .OrderByDescending(d => d.UploadDateUtc)
            .ToListAsync();
    }

    public async Task<Document> UploadAsync(Document document, Stream fileStream, int requestingUserId)
    {
        if (document == null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        var validation = DocumentUploadValidator.Validate(document.FileName, document.FileSizeBytes, document.Title, document.Category);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException(validation.ErrorMessage);
        }

        document.Title = document.Title.Trim();
        document.Category = document.Category.Trim();
        document.UploadedByUserId = requestingUserId;
        document.UploadDateUtc = DateTime.UtcNow;
        document.UpdatedDateUtc = DateTime.UtcNow;

        var relativePath = await _fileStorageService.SaveAsync(fileStream, document.FileName, "documents");
        document.StoredFileName = Path.GetFileName(relativePath);
        document.FilePath = relativePath;

        if (string.IsNullOrWhiteSpace(document.FileType))
        {
            document.FileType = "application/octet-stream";
        }

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        return document;
    }

    public async Task<bool> DeleteAsync(int documentId, int requestingUserId)
    {
        var document = await _context.Documents
            .Include(d => d.Project)
            .FirstOrDefaultAsync(d => d.DocumentId == documentId && !d.IsDeleted);

        if (document == null)
        {
            return false;
        }

        var isOwner = document.UploadedByUserId == requestingUserId;
        var isProjectManager = document.Project != null && document.Project.ProjectManagerId == requestingUserId;
        var isAdmin = await _context.Users.AnyAsync(u => u.UserId == requestingUserId && u.Role == UserRole.Administrator);

        if (!isOwner && !isProjectManager && !isAdmin)
        {
            return false;
        }

        document.IsDeleted = true;
        document.UpdatedDateUtc = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(document.FilePath))
        {
            await _fileStorageService.DeleteAsync(document.FilePath);
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
