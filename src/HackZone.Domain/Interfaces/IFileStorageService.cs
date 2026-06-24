namespace HackZone.Domain.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);
    Task DeleteAsync(string objectName, CancellationToken ct = default);
    string GetUrl(string objectName);
}
