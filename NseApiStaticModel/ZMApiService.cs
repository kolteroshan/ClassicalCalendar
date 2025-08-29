using System.Text;

namespace NseApiStaticModel;

public static class ZMApiService
{
    public static async Task<HttpClient> ZerodhaMarginCalApiAsync()
    {
        var handler = new HttpClientHandler
        {
            UseCookies = false
        };

        var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Add(ZMCApiStaticData.Cookie, ZMCApiStaticData.CfBmCookie_CfUvidCookie);

        return client;
    }
}