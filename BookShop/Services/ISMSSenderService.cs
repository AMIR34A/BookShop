namespace BookShop.Services
{
    public interface ISMSSenderService
    {
        Task<bool> SendSMS(string token, string phone);
    }
}
