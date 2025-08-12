using ClassicalCalendarGenericModel;
using DTO;
using System.IO.Compression;
using System.Text.Json;

public static class Deserializer
{
    public static async Task<Responses<DeserializerDto<T>>> DeserializationResponse<T>(HttpResponseMessage responseMessage)
    {
        await using Stream responseStream = await GetDecompressedStream(responseMessage);

        T? deserialized = await JsonSerializer.DeserializeAsync<T>(
            responseStream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            });

        if (deserialized == null)
            throw new JsonException("Deserialization returned null.");

        return Responses<DeserializerDto<T>>.Success(new DeserializerDto<T>
        {
            Response = deserialized
        });
    }

    private static async Task<Stream> GetDecompressedStream(HttpResponseMessage responseMessage)
    {
        Stream rawStream = await responseMessage.Content.ReadAsStreamAsync();

        if (responseMessage.Content.Headers.ContentEncoding.Contains("gzip"))
            return new GZipStream(rawStream, CompressionMode.Decompress);

        if (responseMessage.Content.Headers.ContentEncoding.Contains("deflate"))
            return new DeflateStream(rawStream, CompressionMode.Decompress);

        if (responseMessage.Content.Headers.ContentEncoding.Contains("br"))
            return new BrotliStream(rawStream, CompressionMode.Decompress);

        return rawStream;
    }
}