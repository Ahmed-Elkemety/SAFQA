using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SAFQA.BLL.Dtos.AccountDto.Forget_password;
using SAFQA.BLL.Dtos.DeliveryDto;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.DAL.Models;

namespace SAFQA.BLL.Managers.DeliveryAppManager
{
    public interface IDeliveryService
    {
        Task<(AuthResult, List<DeliveryDto>)> GetSellerDeliveries(string sellerId);
        Task<AuthResult> RequestLoginOtpAsync(string email);
        Task<AuthResult> VerifyLoginOtpAsync(VerifyOtpDto dto);

        Task<AuthResult> Step2Async(int auctionId);
        Task<AuthResult> Step3Async(int auctionId, string contact);
        Task<AuthResult> Step4Async(int auctionId, IFormFile image);
        Task<AuthResult> Step5Async(int auctionId);
    }
}
