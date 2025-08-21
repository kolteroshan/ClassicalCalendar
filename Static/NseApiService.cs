using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Static;

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

        client.DefaultRequestHeaders.UserAgent.ParseAdd(NseStaticData.UserAgent);
        client.DefaultRequestHeaders.Accept.ParseAdd(NseStaticData.Accept);

        await client.GetAsync(NseStaticData.Refere);
        await client.GetAsync(NseStaticData.OptionChain);

        return client;
    }
}
