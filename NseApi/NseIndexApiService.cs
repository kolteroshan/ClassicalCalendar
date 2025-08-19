using DTO;
using Static;
using System.Net;
using ClassicalCalendarJsonModel;
using ClassicalCalendarGenericModel;

namespace NseApi;

public class NseIndexApiService
{
    public async Task<Responses<StrikeSnapshotDTO>> GetIndexOptionChainAsync(string index)
    {
        try
        {
            var response = await IndexApiAsync(index);

            if (response != null)
            {
                var deserializeResponse = await Deserializer.DeserializationResponse<Root>(response);

                return Responses<StrikeSnapshotDTO>.Success(new StrikeSnapshotDTO
                {
                    Strike = deserializeResponse.Data!.Response.Strike,
                    ClosePrice = deserializeResponse.Data.Response.ClosePrice,
                    LtpDtos = deserializeResponse.Data.Response.GrapthData
                        .Select(c => new LtpDto
                        {
                            Date = c.Date,
                            Time = c.Time,
                            Value = c.Value
                        }).ToList(),
                    Name = deserializeResponse.Data.Response.Name,
                    Type = deserializeResponse.Data.Response.Type,
                    Date = deserializeResponse.Data.Response.Date
                });
            }
        }
        catch (Exception ex)
        {
            var message = ex.Message;
        }

        return Responses<StrikeSnapshotDTO>.Error(HttpStatusCode.NotFound, index);
    }

    public async Task<HttpResponseMessage> IndexApiAsync(string symbol)
    {
        using (var handler = new HttpClientHandler
        {
            UseCookies = true,
            AllowAutoRedirect = true
        })

        using (var client = new HttpClient(handler))
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd(NseStaticData.UserAgent);
            client.DefaultRequestHeaders.Accept.ParseAdd(NseStaticData.Accept);

            await client.GetAsync(NseStaticData.Refere);
            await client.GetAsync(NseStaticData.OptionChain);

            var response = await client.GetAsync($"{NseStaticData.IndexUrl}{symbol}");

            response.EnsureSuccessStatusCode();
            return response;
        }
    }
}
