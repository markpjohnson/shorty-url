#r "Microsoft.WindowsAzure.Storage"
using Microsoft.WindowsAzure.Storage.Table;
using System.Net;
using HashidsNet;

public static readonly Hashids hasher = new Hashids("this is my salt", 7);
public static readonly Random random = new Random();

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log, IQueryable<UrlShorty> table, ICollector<UrlShorty> shorties)
{
    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    string url = data?.url;
    
    if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
    {
        return req.CreateResponse(HttpStatusCode.BadRequest, "URL is not well-formed");
    }

    string hash = null;
    do {
        hash = hasher.Encode(random.Next(100000000, 1000000000));
    } while (table.Where(us => us.RowKey == hash).ToList().Any());
    
    log.Info("hashid = " + hash);

    if (url != null) 
    {
        var shorty = new UrlShorty { PartitionKey = "prod", RowKey = hash, Url = url };
        shorties.Add(shorty);
        return req.CreateResponse(HttpStatusCode.Created, shorty);
    }

    return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a url in the request body");
}

public class UrlShorty : TableEntity
{
    public string Url { get; set; }
}