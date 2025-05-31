using Filament.WebApi.Data;
using Filament.WebApi.Models;
using Filament.WebApi.Models.DTOs;
using Filament.WebApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Filament.WebApi.Services;

public class FileService(ApplicationDbContext context, IWebHostEnvironment environment) : IFileService
{
    public async Task<FileResponseDto> UploadFileAsync(IFormFile file, string userId)
    {
        if (file == null || file.Length == 0)
            return null;

        var uploadsFolder = Path.Combine(environment.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileModel = new FileModel
        {
            FileName = fileName,
            OriginalFileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            FilePath = filePath,
            UserId = userId
        };

        context.Files.Add(fileModel);
        await context.SaveChangesAsync();

        return new FileResponseDto
        {
            Id = fileModel.Id,
            FileName = fileModel.FileName,
            OriginalFileName = fileModel.OriginalFileName,
            ContentType = fileModel.ContentType,
            FileSize = fileModel.FileSize,
            UploadedAt = fileModel.UploadedAt,
            UserId = fileModel.UserId,
            UserEmail = fileModel.User?.Email ?? ""
        };
    }

    public async Task<List<FileResponseDto>> GetUserFilesAsync(string userId)
    {
        return await context.Files
            .Include(f => f.User)
            .Where(f => f.UserId == userId)
            .Select(f => new FileResponseDto
            {
                Id = f.Id,
                FileName = f.FileName,
                OriginalFileName = f.OriginalFileName,
                ContentType = f.ContentType,
                FileSize = f.FileSize,
                UploadedAt = f.UploadedAt,
                UserId = f.UserId,
                UserEmail = f.User.Email ?? ""
            })
            .ToListAsync();
    }

    public async Task<List<FileResponseDto>> GetAllFilesAsync()
    {
        return await context.Files
            .Include(f => f.User)
            .Select(f => new FileResponseDto
            {
                Id = f.Id,
                FileName = f.FileName,
                OriginalFileName = f.OriginalFileName,
                ContentType = f.ContentType,
                FileSize = f.FileSize,
                UploadedAt = f.UploadedAt,
                UserId = f.UserId,
                UserEmail = f.User.Email ?? ""
            })
            .ToListAsync();
    }

    public async Task<FileModel> GetFileByIdAsync(int id)
    {
        return await context.Files
            .Include(f => f.User)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<bool> DeleteFileAsync(int id, string userId, bool isAdmin = false)
    {
        var file = await context.Files.FindAsync(id);
        if (file == null)
            return false;

        if (!isAdmin && file.UserId != userId)
            return false;

        if (File.Exists(file.FilePath))
            File.Delete(file.FilePath);

        context.Files.Remove(file);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<(byte[] content, string contentType, string fileName)?> DownloadFileAsync(int id, string userId, bool isAdmin = false)
    {
        var file = await context.Files.FindAsync(id);
        if (file == null)
            return null;

        if (!isAdmin && file.UserId != userId)
            return null;

        if (!File.Exists(file.FilePath))
            return null;

        var content = await File.ReadAllBytesAsync(file.FilePath);
        return (content, file.ContentType, file.OriginalFileName);
    }
}
