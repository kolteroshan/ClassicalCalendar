using ClassicalCalendarGenericModel;
using DTO;
using System.Net;

namespace OptiChainScheduler.NseApiService.NseIndexApiService;

public class NseIndexApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public NseIndexApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            UseCookies = true,
            CookieContainer = new CookieContainer()
        };

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", _configuration["NseApiService:UserAgent"]);
        _httpClient.DefaultRequestHeaders.Add("Accept", _configuration["NseApiService:Accept"]);
        _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", _configuration["NseApiService:AcceptEncoding"]);
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", _configuration["NseApiService:AcceptLanguage"]);
        _httpClient.DefaultRequestHeaders.Add("Referer", _configuration["NseApiService:Referer"]);
    }

    public async Task InitialResponseAsync()
        => await _httpClient.GetAsync(_configuration["NseApiService:Referer"]);

    public async Task<HttpResponseMessage> IndexApiAsync(string index)
        => await _httpClient.GetAsync($"{_configuration["OptionChainUrl:IndexUrl"]}{index}");

    public async Task<Responses<StrikeSnapshotDTO>> GetIndexOptionChainAsync(string index)
    {
        try
        {
            await InitialResponseAsync();
            var response = await IndexApiAsync(index);

            if (response != null)
            {
                var dto = await Deserializer.DeserializationResponse<StrikeSnapshotDTO>(response);

                return Responses<StrikeSnapshotDTO>.Success(dto.Data.Response);
            }
        }
        catch (Exception ex)
        {
            var message = ex.Message;
        }

        return Responses<StrikeSnapshotDTO>.Error(HttpStatusCode.NotFound, index);
    }
}