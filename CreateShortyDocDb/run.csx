#r "Microsoft.Azure.Documents.Client"
#r "Newtonsoft.Json"
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log, DocumentClient client)
{
    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    string url = data?.url;
    
    if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
    {
        return req.CreateResponse(HttpStatusCode.BadRequest, "URL is not well-formed");
    }

    try 
    {
        var shorty = new UrlShorty { Url = url };
        
        ResourceResponse<Document> res = await client.CreateDocumentAsync(
                                            UriFactory.CreateDocumentCollectionUri("Shorty", "urlshorties"), 
                                            shorty, 
                                            new RequestOptions { PreTriggerInclude = new List<string> { "addShortId" } });
        
        shorty.Id = res.Resource.GetPropertyValue<string>("id");

        return req.CreateResponse(HttpStatusCode.Created, shorty);
    } 
    catch (Exception e)
    {
        log.Error("Exception occurred", e);
        throw;
    }

    return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a url in the request body");
}

public class UrlShorty 
{
    [JsonProperty("id")]
    public string Id { get; set; }
    
    [JsonProperty("url")]
    public string Url { get; set; }
}