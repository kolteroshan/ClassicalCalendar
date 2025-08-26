using System.Net;

namespace NseApiGenericModel;

public class NseApiResponses<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public string StatusMessage { get; set; }
    public T? Data { get; set; }

    public NseApiResponses() { }

    public NseApiResponses(HttpStatusCode statusCode, string statusMessage, T? data)
    {
        StatusCode = statusCode;
        StatusMessage = statusMessage;
        Data = data;
    }

    public static NseApiResponses<T> Success(T? data, string message = "Success")
    {
        return new NseApiResponses<T>(HttpStatusCode.OK, message, data);
    }

    public static NseApiResponses<T> Error(HttpStatusCode statusCode, string message)
    {
        return new NseApiResponses<T>(statusCode, message, default(T));
    }
}