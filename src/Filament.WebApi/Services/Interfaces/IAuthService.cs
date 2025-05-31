using Filament.WebApi.Models.DTOs;

namespace Filament.WebApi.Services.Interfaces;


public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto model);
    Task<AuthResponseDto> LoginAsync(LoginDto model);
}
