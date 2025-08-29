using NseApiStaticModel;
using System.Text;
using NseApiJsonModel;
using NseApiDTO;
using NseApiGenericModel;

namespace NseApi;

public class ZerodhaMarginCalculatorApiService
{
    public async Task<NseApiResponses<MarginDTO>> GetMargin(
        string exchange, 
        string product,
        string market,
        DateOnly expiry, 
        string optionType, 
        double strike, 
        int qty, 
        string trade)
    {
        var marginClient = await ZMApiService.ZerodhaMarginCalApiAsync();

        var body = $"action=calculate&exchange%5B%5D={exchange}" +
            $"&product%5B%5D={product}" +
            $"&scrip%5B%5D={market}{expiry:yy}{expiry.Month}{expiry:dd}" +
            $"&option_type%5B%5D={optionType}" +
            $"&strike_price%5B%5D={strike}" +
            $"&qty%5B%5D={qty}" +
            $"&trade%5B%5D={trade}";

        var content = new StringContent(body, Encoding.UTF8, ZMCApiStaticData.ContentType);
        var response = await marginClient.PostAsync(ZMCApiStaticData.MarginApi, content);
        var deserializeResponse = await Deserializer.DeserializationResponse<MarginData>(response);

        return NseApiResponses<MarginDTO>
            .Success(new MarginDTO
            {
                Span = deserializeResponse.Data.Response.Last.Span,
                Spread = deserializeResponse.Data.Response.Last.Spread,
                Exposure = deserializeResponse.Data.Response.Last.Exposure,
                NetOptionValue = deserializeResponse.Data.Response.Last.NetOptionValue,
                Total = deserializeResponse.Data.Response.Last.Total
            });
    }
}