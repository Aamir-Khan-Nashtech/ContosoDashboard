using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoDashboard.Models;

public class Document
{
    public static readonly string[] AllowedCategories =
    {
        "Project Documents",
        "Team Resources",
        "Personal Files",
        "Reports",
        "Presentations",
        "Other"
    };

    public static bool IsValidCategory(string? category)
    {
        return !string.IsNullOrWhiteSpace(category) && AllowedCategories.Contains(category.Trim(), StringComparer.OrdinalIgnoreCase);
    }

    [Key]
    public int DocumentId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string StoredFileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string FileType { get; set; } = string.Empty;

    [Required]
    public long FileSizeBytes { get; set; }

    [Required]
    public int UploadedByUserId { get; set; }

    public int? ProjectId { get; set; }

    public int? TaskId { get; set; }

    [Required]
    public DateTime UploadDateUtc { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedDateUtc { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; }

    [MaxLength(500)]
    public string? Tags { get; set; }

    [ForeignKey("UploadedByUserId")]
    public virtual User UploadedByUser { get; set; } = null!;

    [ForeignKey("ProjectId")]
    public virtual Project? Project { get; set; }

    [ForeignKey("TaskId")]
    public virtual TaskItem? Task { get; set; }

    public virtual ICollection<DocumentShare> Shares { get; set; } = new List<DocumentShare>();
    public virtual ICollection<DocumentActivity> Activities { get; set; } = new List<DocumentActivity>();
}
