using SAFQA.BLL.Dtos.SellerAppDto.PaymentDto;
using SAFQA.BLL.Managers.SellerAppManager.WalletServeice;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Wallet;

public class CardService : ICardService
{
    private readonly ICardRepo _cardRepo;
    private readonly IWalletRepo _walletRepo;

    public CardService(ICardRepo cardRepo, IWalletRepo walletRepo)
    {
        _cardRepo = cardRepo;
        _walletRepo = walletRepo;
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

            // جلب المحفظة الخاصة بالمستخدم من التوكن
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

            // التأكد من عدم وجود الكارت مسبقًا
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

            string brand = dto.CardNumber.StartsWith("4") ? "Visa" :
                           dto.CardNumber.StartsWith("5") ? "MasterCard" : "Unknown";

            string token = Guid.NewGuid().ToString();

            var card = new SavedCard
            {
                Last4Digits = last4,
                CardBrand = brand,
                ExpiryMonth = month,
                ExpiryYear = year,
                PaymentToken = token,
                WalletId = wallet.Id,
                IsDefault = false
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
}