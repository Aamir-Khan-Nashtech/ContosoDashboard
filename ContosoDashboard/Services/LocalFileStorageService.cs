namespace ContosoDashboard.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _rootPath = Path.Combine(environment.ContentRootPath, "AppData", "uploads");
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<string> SaveAsync(Stream content, string fileName, string? subFolder = null, CancellationToken cancellationToken = default)
    {
        var safeFileName = Path.GetFileName(fileName);
        var folderPath = string.IsNullOrWhiteSpace(subFolder)
            ? _rootPath
            : Path.Combine(_rootPath, subFolder);

        Directory.CreateDirectory(folderPath);

        var uniqueFileName = CreateUniqueFileName(safeFileName);
        var fullPath = Path.Combine(folderPath, uniqueFileName);

        await using var stream = File.Create(fullPath);
        await content.CopyToAsync(stream, cancellationToken);

        var relativePath = Path.Combine(subFolder ?? string.Empty, uniqueFileName).Replace('\\', '/');
        return relativePath.TrimStart('/');
    }

    public Task<Stream> OpenReadStreamAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_rootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
        return Task.FromResult<Stream>(File.OpenRead(fullPath));
    }

    public Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_rootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public bool FileExists(string relativePath)
    {
        var fullPath = Path.Combine(_rootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
        return File.Exists(fullPath);
    }

    private static string CreateUniqueFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var name = Path.GetFileNameWithoutExtension(fileName);
        var safeName = string.IsNullOrWhiteSpace(name) ? "document" : name;
        return $"{safeName}-{Guid.NewGuid():N}{extension}";
    }
}
