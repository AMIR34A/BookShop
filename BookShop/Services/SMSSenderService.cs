namespace BookShop.Services;

public class SMSSenderService : ISMSSenderService
{
    public async Task<bool> SendSMS(string token, string phone)
    {
        using HttpClient httpClient = new HttpClient();
        var httpResponse = await httpClient.GetAsync($"https://api.kavenegar.com/v1/78444F304653344B31365145675871637162305979596E68477732484879464F773574626755616E2B77673D/verify/lookup.json?receptor={phone}&token={token}&template=AuthVerify");
        return httpResponse.IsSuccessStatusCode;
    }
}
