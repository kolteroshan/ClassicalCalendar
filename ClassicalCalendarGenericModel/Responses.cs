using System.Net;

namespace ClassicalCalendarGenericModel;

public class Responses<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public string StatusMessage { get; set; }
    public T? Data { get; set; }

    public Responses() { }

    public Responses(HttpStatusCode statusCode, string statusMessage, T? data)
    {
        StatusCode = statusCode;
        StatusMessage = statusMessage;
        Data = data;
    }

    public static Responses<T> Success(T? data, string message = "Success")
    {
        return new Responses<T>(HttpStatusCode.OK, message, data);
    }

    public static Responses<T> Error(HttpStatusCode statusCode, string message)
    {
        return new Responses<T>(statusCode, message, default(T));
    }
}