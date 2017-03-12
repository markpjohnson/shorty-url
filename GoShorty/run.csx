#r "Microsoft.WindowsAzure.Storage"
using Microsoft.WindowsAzure.Storage.Table;
using System.Net;
using System.Net.Http.Headers;

public static HttpResponseMessage Run(HttpRequestMessage req, TraceWriter log, UrlShorty shorty)
{
    if (shorty == null) 
    {
        return req.CreateResponse(HttpStatusCode.BadRequest, "Could not find this URL");
    }

    var response = req.CreateResponse(HttpStatusCode.Moved, "");
    response.Headers.Location = new Uri(shorty.Url);
    response.Content.Headers.LastModified = DateTime.UtcNow;
    response.Headers.CacheControl = new CacheControlHeaderValue { Private = true, MaxAge = TimeSpan.FromSeconds(90) };
    response.Headers.Add("X-Shorty-Timestamp", DateTime.Now.ToString());
    return response;
}

public class UrlShorty : TableEntity
{
    public string Url { get; set; }
}