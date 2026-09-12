using Microsoft.EntityFrameworkCore;
using ContosoDashboard.Data;
using ContosoDashboard.Models;

namespace ContosoDashboard.Services;

public interface IDashboardService
{
    Task<DashboardSummary> GetDashboardSummaryAsync(int userId);
    Task<List<Announcement>> GetActiveAnnouncementsAsync();
    Task<List<Document>> GetRecentDocumentsAsync(int userId, int count = 5);
}

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummary> GetDashboardSummaryAsync(int userId)
    {
        var now = DateTime.UtcNow;
        var visibleProjectIds = await _context.ProjectMembers
            .Where(pm => pm.UserId == userId)
            .Select(pm => pm.ProjectId)
            .ToListAsync();

        var summary = new DashboardSummary
        {
            TotalActiveTasks = await _context.Tasks
                .CountAsync(t => t.AssignedUserId == userId && t.Status != Models.TaskStatus.Completed),

            TasksDueToday = await _context.Tasks
                .CountAsync(t => t.AssignedUserId == userId 
                    && t.DueDate.HasValue 
                    && t.DueDate.Value.Date == now.Date
                    && t.Status != Models.TaskStatus.Completed),

            ActiveProjects = await _context.Projects
                .Where(p => p.ProjectManagerId == userId || p.ProjectMembers.Any(pm => pm.UserId == userId))
                .Where(p => p.Status == ProjectStatus.Active)
                .CountAsync(),

            UnreadNotifications = await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead),

            RecentDocumentsCount = await _context.Documents
                .CountAsync(d => !d.IsDeleted && (
                    d.UploadedByUserId == userId ||
                    (d.ProjectId != null && (d.Project != null && (d.Project.ProjectManagerId == userId || visibleProjectIds.Contains(d.ProjectId.Value)))) ||
                    d.Shares.Any(s => s.IsActive && s.SharedWithUserId == userId) ||
                    d.Shares.Any(s => s.IsActive && s.SharedWithProjectId != null && visibleProjectIds.Contains(s.SharedWithProjectId.Value))
                ))
        };

        return summary;
    }

    public async Task<List<Announcement>> GetActiveAnnouncementsAsync()
    {
        var now = DateTime.UtcNow;

        return await _context.Announcements
            .Include(a => a.CreatedByUser)
            .Where(a => a.IsActive 
                && a.PublishDate <= now 
                && (!a.ExpiryDate.HasValue || a.ExpiryDate.Value > now))
            .OrderByDescending(a => a.PublishDate)
            .Take(5)
            .ToListAsync();
    }

    public async Task<List<Document>> GetRecentDocumentsAsync(int userId, int count = 5)
    {
        var visibleProjectIds = await _context.ProjectMembers
            .Where(pm => pm.UserId == userId)
            .Select(pm => pm.ProjectId)
            .ToListAsync();

        return await _context.Documents
            .Include(d => d.Project)
            .Include(d => d.UploadedByUser)
            .Where(d => !d.IsDeleted && (
                d.UploadedByUserId == userId ||
                (d.ProjectId != null && (d.Project != null && (d.Project.ProjectManagerId == userId || visibleProjectIds.Contains(d.ProjectId.Value)))) ||
                d.Shares.Any(s => s.IsActive && s.SharedWithUserId == userId) ||
                d.Shares.Any(s => s.IsActive && s.SharedWithProjectId != null && visibleProjectIds.Contains(s.SharedWithProjectId.Value))
            ))
            .OrderByDescending(d => d.UploadDateUtc)
            .Take(count)
            .ToListAsync();
    }
}

public class DashboardSummary
{
    public int TotalActiveTasks { get; set; }
    public int TasksDueToday { get; set; }
    public int ActiveProjects { get; set; }
    public int UnreadNotifications { get; set; }
    public int RecentDocumentsCount { get; set; }
}
