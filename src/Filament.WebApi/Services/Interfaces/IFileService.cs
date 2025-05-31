using Filament.WebApi.Models;
using Filament.WebApi.Models.DTOs;

namespace Filament.WebApi.Services.Interfaces;


public interface IFileService
{
    Task<FileResponseDto> UploadFileAsync(IFormFile file, string userId);
    Task<List<FileResponseDto>> GetUserFilesAsync(string userId);
    Task<List<FileResponseDto>> GetAllFilesAsync();
    Task<FileModel> GetFileByIdAsync(int id);
    Task<bool> DeleteFileAsync(int id, string userId, bool isAdmin = false);
    Task<(byte[] content, string contentType, string fileName)?> DownloadFileAsync(int id, string userId, bool isAdmin = false);
}
