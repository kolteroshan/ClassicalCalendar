namespace NseApiStaticModel;

public static class NseApiService
{
    public static async Task<HttpClient> IndexApiAsync()
    {
        var handler = new HttpClientHandler
        {
            UseCookies = true,
            AllowAutoRedirect = true
        };

        var client = new HttpClient(handler);

        client.DefaultRequestHeaders.UserAgent.ParseAdd(NseApiStaticData.UserAgent);
        client.DefaultRequestHeaders.Accept.ParseAdd(NseApiStaticData.Accept);

        await client.GetAsync(NseApiStaticData.Refere);
        await client.GetAsync(NseApiStaticData.OptionChain);

        return client;
    }
}