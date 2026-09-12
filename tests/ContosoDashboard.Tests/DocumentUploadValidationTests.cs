using ContosoDashboard.Services;

namespace ContosoDashboard.Tests;

public class DocumentUploadValidationTests
{
    [Fact]
    public void ValidateUpload_RejectsUnsupportedExtension()
    {
        var result = DocumentUploadValidator.Validate(
            "notes.exe",
            1024,
            "Quarterly update",
            "Project Documents");

        Assert.False(result.IsValid);
        Assert.Contains("supported", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateUpload_RejectsFilesOver25Mb()
    {
        var result = DocumentUploadValidator.Validate(
            "budget.pdf",
            26 * 1024 * 1024,
            "Budget",
            "Reports");

        Assert.False(result.IsValid);
        Assert.Contains("25 MB", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateUpload_AcceptsSupportedFiles()
    {
        var result = DocumentUploadValidator.Validate(
            "project-plan.pdf",
            512 * 1024,
            "Project plan",
            "Project Documents");

        Assert.True(result.IsValid);
        Assert.Equal(string.Empty, result.ErrorMessage);
    }
}
