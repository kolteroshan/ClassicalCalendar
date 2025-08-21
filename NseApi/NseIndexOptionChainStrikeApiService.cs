using ClassicalCalendarGenericModel;
using ClassicalCalendarJsonModel;
using DTO;
using Static;
using System;
using System.Net;

namespace NseApi;

public class NseIndexOptionChainStrikeApiService
{
    public async Task GetIndexOptionChainAsync()
    {
        try
        {
            var response = await NseOptionChainApiAsync("NIFTY");

            if (response != null)
            {
                var deserializeResponse = await Deserializer.DeserializationResponse<OptionChainData>(response);

                var expiryDateObjectList = new List<ExpiryOptionDataDTO>();

                var records = deserializeResponse.Data.Response.Records;

                foreach (var item in records.ExpiryDates)
                {
                    var datas = records.Data
                        .Where(d => d.ExpiryDate == item)
                        .OrderBy(s => s.StrikePrice)
                        .ToList();

                    var strikeDTOs = new List<StrikeDTO>();

                    foreach (var data in datas)
                    {
                        var d = data;

                        var strikeDTO = new StrikeDTO();
                        strikeDTO.Strike = d.StrikePrice;

                        if (d.CE is not null)
                        {
                            var callDTO = new CallDTO
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
                            var putDTO = new PutDTO
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

                    var expiryDateObject = new ExpiryOptionDataDTO();

                    expiryDateObject.ExpiryDate = item;
                    expiryDateObject.ExpiryOptionDataDate = records.Date;
                    expiryDateObject.LiveStrike = records.UnderlyingValue;
                    expiryDateObject.Strikes = strikeDTOs;

                    expiryDateObjectList.Add(expiryDateObject);
                }

                var optionChainDto = new OptionChainDto
                {
                    DateOfOptionChain = records.Date,
                    Expires = expiryDateObjectList
                };

                var dto = ExpiryDtoMapping(deserializeResponse.Data!.Response.Records);
            }
        }
        catch (Exception ex)
        {
            var message = ex.Message;
        }
    }

    public async Task<HttpResponseMessage> NseOptionChainApiAsync(string symbol)
    {
        var client = await NseApiService.IndexApiAsync();

        var response = await client.GetAsync($"{NseStaticData.IndexOptionChain}{symbol}");

        response.EnsureSuccessStatusCode();
        return response;
    }

    public ExpiryOptionDataDTO ExpiryDtoMapping(Records records)
    {
        return new ExpiryOptionDataDTO();
    }
}
