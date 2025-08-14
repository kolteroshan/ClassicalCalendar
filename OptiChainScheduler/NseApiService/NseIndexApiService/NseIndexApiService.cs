using ClassicalCalendarGenericModel;
using ClassicalCalendarJsonModel;
using DTO;
using System.Net;

namespace OptiChainScheduler.NseApiService.NseIndexApiService;

public class NseIndexApiService
{
    private readonly IConfiguration _configuration;

    public NseIndexApiService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

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
            client.DefaultRequestHeaders.UserAgent.ParseAdd(_configuration["NseApiService:UserAgent"]);
            client.DefaultRequestHeaders.Accept.ParseAdd(_configuration["NseApiService:Accept"]);

            await client.GetAsync(_configuration["NseApiService:Referer"]);
            await client.GetAsync(_configuration["NseApiService:OptionChain"]);

            var response = await client.GetAsync($"{_configuration["OptionChainUrl:IndexUrl"]}{symbol}");

            response.EnsureSuccessStatusCode();
            return response;
        }
    }
}