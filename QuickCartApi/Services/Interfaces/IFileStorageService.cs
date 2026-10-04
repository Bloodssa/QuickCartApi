namespace QuickCartApi.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<(bool Success, string? FileName, string? Error)> SaveImageAsync(IFormFile file);
        void DeleteImage(string fileName);
    }
}
