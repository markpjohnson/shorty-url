#r "Newtonsoft.Json"
using System.Net;
using Newtonsoft.Json;

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

public class UrlShorty 
{
    [JsonProperty("id")]
    public string Id { get; set; }
    
    [JsonProperty("url")]
    public string Url { get; set; }
}