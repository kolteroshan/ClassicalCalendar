using Static;
using System.Net;
using ClassicalCalendarGenericModel;
using NseApiJsonModel;

namespace NseApi;

public class NseIndexStrikeLtpHistoryApiService
{
    public async Task<Responses<StrikeSnapshotDTO>> GetIndexOptionChainStrikeLtpHistoryAsync(string index)
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
                    LtpDtos = deserializeResponse.Data.Response.GraphPoint
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
        var client = await NseApiService.IndexApiAsync();

        var response = await client.GetAsync($"{NseStaticData.LtpHistoryUrl}{symbol}");

        response.EnsureSuccessStatusCode();
        return response;
    }
}
