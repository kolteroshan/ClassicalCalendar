using NseApiDTO;
using NseApiGenericModel;
using NseApiJsonModel;
using NseApiStaticModel;
using System.Net;

namespace NseApi;

public class NseIndexOptionChainStrikeApiService
{
    public async Task<NseApiResponses<NseOptionChainDTO>> GetIndexOptionChainAsync(string index)
    {
        try
        {
            var response = await NseOptionChainApiAsync(index);

            if (response != null)
            {
                var deserializeResponse = await Deserializer.DeserializationResponse<OptionChainData>(response);

                var records = deserializeResponse.Data.Response.Records;


                return NseApiResponses<NseOptionChainDTO>
                    .Success(ExpiryDtoMapping(deserializeResponse.Data!.Response.Records));
            }
        }
        catch (Exception ex)
        {
            var message = ex.Message;
        }

        return NseApiResponses<NseOptionChainDTO>.Error(HttpStatusCode.NotFound, index);
    }

    public async Task<HttpResponseMessage> NseOptionChainApiAsync(string symbol)
    {
        var client = await NseApiService.IndexApiAsync();

        var response = await client.GetAsync($"{NseApiStaticData.IndexOptionChain}{symbol}");

        response.EnsureSuccessStatusCode();
        return response;
    }

    public NseOptionChainDTO ExpiryDtoMapping(Records records)
    {
        var expiryDateObjectList = new List<NseExpiryOptionDataDTO>();

        foreach (var item in records.ExpiryDates)
        {
            var datas = records.Data
                .Where(d => d.ExpiryDate == item)
                .OrderBy(s => s.StrikePrice)
                .ToList();

            var strikeDTOs = new List<NseStrikeDTO>();

            foreach (var data in datas)
            {
                var d = data;

                var strikeDTO = new NseStrikeDTO();
                strikeDTO.Strike = d.StrikePrice;

                if (d.CE is not null)
                {
                    var callDTO = new NseCallDTO
                    {

                        ExpiryDate = d.CE.ExpiryDate,
                        StrikePrice = d.CE.StrikePrice,
                        Underlying = d.CE.Underlying,
                        OpenInterest = d.CE.OpenInterest,
                        ChangeinOpenInterest = d.CE.ChangeinOpenInterest,
                        PchangeinOpenInterest = d.CE.PchangeinOpenInterest,
                        TotalTradedVolume = d.CE.TotalTradedVolume,
                        LastPrice = d.CE.LastPrice,
                        Bidprice = d.CE.Bidprice,
                        AskPrice = d.CE.AskPrice,
                        UnderlyingValue = d.CE.UnderlyingValue
                    };

                    strikeDTO.CallDTO = callDTO;
                }

                if (d.PE is not null)
                {
                    var putDTO = new NsePutDTO
                    {

                        ExpiryDate = d.PE.ExpiryDate,
                        StrikePrice = d.PE.StrikePrice,
                        Underlying = d.PE.Underlying,
                        OpenInterest = d.PE.OpenInterest,
                        ChangeinOpenInterest = d.PE.ChangeinOpenInterest,
                        PchangeinOpenInterest = d.PE.PchangeinOpenInterest,
                        TotalTradedVolume = d.PE.TotalTradedVolume,
                        LastPrice = d.PE.LastPrice,
                        Bidprice = d.PE.Bidprice,
                        AskPrice = d.PE.AskPrice,
                        UnderlyingValue = d.PE.UnderlyingValue
                    };

                    strikeDTO.PutData = putDTO;
                }

                strikeDTOs.Add(strikeDTO);
            }

            var expiryDateObject = new NseExpiryOptionDataDTO();

            expiryDateObject.ExpiryDate = item;
            expiryDateObject.ExpiryOptionDataDate = records.Date;
            expiryDateObject.LiveStrike = records.UnderlyingValue;
            expiryDateObject.Strikes = strikeDTOs;

            expiryDateObjectList.Add(expiryDateObject);
        }

        return new NseOptionChainDTO
        {
            DateOfOptionChain = records.Date,
            Expires = expiryDateObjectList
        };
    }
}
