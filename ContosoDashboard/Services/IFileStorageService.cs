namespace ContosoDashboard.Services;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream content, string fileName, string? subFolder = null, CancellationToken cancellationToken = default);
    Task<Stream> OpenReadStreamAsync(string relativePath, CancellationToken cancellationToken = default);
    Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default);
    bool FileExists(string relativePath);
}
