using Filament.WebApi.Models.DTOs;
using Filament.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Filament.WebApi.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController(IFileService fileService) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] FileUploadDto model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var result = await fileService.UploadFileAsync(model.File, userId);
        if (result == null)
            return BadRequest("File upload failed");

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetFiles()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (isAdmin)
        {
            var allFiles = await fileService.GetAllFilesAsync();
            return Ok(allFiles);
        }
        else
        {
            var userFiles = await fileService.GetUserFilesAsync(userId!);
            return Ok(userFiles);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFile(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var file = await fileService.GetFileByIdAsync(id);
        if (file == null)
            return NotFound();

        if (!isAdmin && file.UserId != userId)
            return Forbid();

        return Ok(new FileResponseDto
        {
            Id = file.Id,
            FileName = file.FileName,
            OriginalFileName = file.OriginalFileName,
            ContentType = file.ContentType,
            FileSize = file.FileSize,
            UploadedAt = file.UploadedAt,
            UserId = file.UserId,
            UserEmail = file.User?.Email ?? ""
        });
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadFile(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var result = await fileService.DownloadFileAsync(id, userId!, isAdmin);
        if (result == null)
            return NotFound();

        return File(result.Value.content, result.Value.contentType, result.Value.fileName);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var result = await fileService.DeleteFileAsync(id, userId!, isAdmin);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
