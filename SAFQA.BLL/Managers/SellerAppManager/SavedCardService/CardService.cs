using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Dtos.SellerAppDto.PaymentDto;
using SAFQA.BLL.Dtos.SellerAppDto.WalletDto;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.BLL.Managers.SellerAppManager.WalletServeice;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Wallet;

public class CardService : ICardService
{
    private readonly ICardRepo _cardRepo;
    private readonly IWalletRepo _walletRepo;
    private readonly SAFQA_Context _context;

    public CardService(ICardRepo cardRepo, IWalletRepo walletRepo , SAFQA_Context context)
    {
        _cardRepo = cardRepo;
        _walletRepo = walletRepo;
        _context = context;
    }
    public bool AddCard(string userId, AddCardDto dto, out string message)
    {
        try
        {
            message = "";

            if (string.IsNullOrWhiteSpace(dto.CardNumber) ||
                string.IsNullOrWhiteSpace(dto.ExpiryDate) ||
                string.IsNullOrWhiteSpace(dto.CVV))
            {
                message = "All fields are required.";
                return false;
            }

            if (dto.CardNumber.Length != 16)
            {
                message = "Card number must be 16 digits.";
                return false;
            }

            if (dto.CVV.Length < 3 || dto.CVV.Length > 4)
            {
                message = "CVV must be 3 or 4 digits.";
                return false;
            }

            var wallet = _walletRepo.GetByIdd(userId);
            if (wallet == null)
            {
                message = "Wallet not found for this user.";
                return false;
            }

            string last4 = dto.CardNumber.Substring(dto.CardNumber.Length - 4);
            var parts = dto.ExpiryDate.Split('/');
            if (parts.Length != 2)
            {
                message = "Invalid expiry date format.";
                return false;
            }

            int month = int.Parse(parts[0]);
            int year = int.Parse(parts[1]);

            var isExist = _cardRepo.GetAll()
                .Any(c =>
                    c.WalletId == wallet.Id &&
                    c.Last4Digits == last4 &&
                    c.ExpiryMonth == month &&
                    c.ExpiryYear == year
                );

            if (isExist)
            {
                message = "This card is already added.";
                return false;
            }

            string brand;

            if (dto.CardLabel != null) 
            {
                brand = dto.CardLabel;
            }
            else
            {
               brand = dto.CardNumber.StartsWith("4") ? "Visa" :
                       dto.CardNumber.StartsWith("5") ? "MasterCard" : "Unknown";

            }

            var card = new SavedCard
            {
                Last4Digits = last4,
                CardBrand = brand,
                ExpiryMonth = month,
                ExpiryYear = year,
                WalletId = wallet.Id,
                CardholderName = dto.CardholderName,

            };

            _cardRepo.Add(card);
            message = "Card added successfully.";
            return true;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return false;
        }
    }

    public IEnumerable<SavedCaredDto> GetCardsByUser(string userId)
    {
        var wallet = _walletRepo.GetAll()
            .FirstOrDefault(w => w.UserId == userId);

        if (wallet == null)
            return new List<SavedCaredDto>();


        var cards = _cardRepo.GetAll()
        .Where(c => c.WalletId == wallet.Id)
        .Select(c => new SavedCaredDto
        {
            CardId = c.Id,
            MaskedCardNumber = "•••• •••• •••• " + c.Last4Digits
        })
        .ToList();

        return cards;
    }

    public async Task<AuthResult> DeleteCardAsync(int cardId, string userId)
    {
        var card = await _context.savedCards
            .Include(c => c.Wallet)
            .FirstOrDefaultAsync(c => c.Id == cardId);

        if (card == null)
        {
            return new AuthResult
            {
                IsSuccess = false,
                Message = "Card not found"
            };
        }

        var userIdcard = card.Wallet.UserId;

        if (userIdcard != userId)
        {
            return new AuthResult
            {
                IsSuccess = false,
                Message = "Unauthorized"
            };
        }

        _cardRepo.Delete(card);

        return new AuthResult
        {
            IsSuccess = true,
            Message = "Card deleted successfully"
        };
    }
}