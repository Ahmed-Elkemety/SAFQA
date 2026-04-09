using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Dtos.AccountDto.Forget_password;
using SAFQA.BLL.Dtos.AccountDto.User;
using static SAFQA.BLL.Dtos.AccountDto.User.LocationDto;
namespace SAFQA.BLL.Managers.AccountManager.Auth
{
    public interface IAuthUser
    {
        Task<AuthResult> RegisterAsync(RegisterDto dto, string deciceId);
        Task<List<CountryDto>> GetCountriesAsync();
        Task<List<CityDto>> GetCitiesByCountryIdAsync(int countryId);
        Task<AuthResult> LoginAsync(LoginDto dto , string deciceId ,string role);
        Task<AuthResult> RefreshTokenAsync(string userId);
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        Task<AuthResult> ConfirmEmailAsync(ConfirmEmailDto dto);
        Task<AuthResult> ResendRegistrationOtpAsync(string email);
        Task<AuthResult> RequestPasswordResetAsync(string email);
        Task<AuthResult> VerifyOtpAsync(VerifyOtpDto dto);
        Task<AuthResult> ResetPasswordAsync(ResetPasswordDto dto);
        Task<AuthResult> ResendOtpAsync(string email);
        Task<AuthResult> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<AuthResult> SignOutAllDevicesAsync(string userId);


    }
}
