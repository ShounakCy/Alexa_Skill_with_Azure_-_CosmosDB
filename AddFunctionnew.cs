using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Linq;

namespace FunctionApp
{
    public static class AddFunction
    {
        [FunctionName("AddFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            // Simple Function
            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();
            log.Info($"Content={data}");
            if (data.request.type == "LaunchRequest")
            {
                // default launch request, let's just let them know what you can do
                log.Info($"Default LaunchRequest made");
                return DefaultRequest(req);
            }
            else if (data.request.type == "IntentRequest")
            {
                // Set name to query string or body data
                string intentName = data.request.intent.name;
                log.Info($"intentName={intentName}");
                switch (intentName)
                {
                    case "AddIntent":
                        DocumentClient client = new DocumentClient(new Uri("https://alexa.documents.azure.com:443/"),
                "xxx==");

                        IQueryable<Document> values = client.CreateDocumentQuery<Document>(UriFactory.CreateDocumentCollectionUri("myDb", "myCollection"), "SELECT * FROM c");

                        Document latest = new Document();
                        foreach (var y in values)
                        {
                            latest = y;
                            break;
                        }
                        foreach (var x in values)
                        {
                            if (x.GetPropertyValue<DateTime>("EventProcessedUtcTime") > latest.GetPropertyValue<DateTime>("EventProcessedUtcTime"))
                            {
                                latest = x;
                            }
                        }

                        var z = latest.GetPropertyValue<float>("ActiveHeatingTime").ToString();
                        var n1 = z;//Convert.ToDouble(data.request.intent.slots["firstnum"].value);
                        var n2 = Convert.ToDouble(data.request.intent.slots["secondnum"].value);
                        string result = n1 ;
                       // Console.WriteLine(result);
                        string subject = result.ToString();
                        return req.CreateResponse(HttpStatusCode.OK, new
                        {
                            version = "1.0",
                            sessionAttributes = new { },
                            response = new
                            {
                                outputSpeech = new
                                {
                                    type = "PlainText",
                                    text = $"The result is {result.ToString()}."
                                },
                                card = new
                                {
                                    type = "Simple",
                                    title = "Alexa-Azure Simple Calculator",
                                    content = $"The result is {result.ToString()}."
                                },
                                shouldEndSession = true
                            }
                        });
                    // Add more intents and default responses
                    default:
                        return DefaultRequest(req);
                }
            }
            else
            {
                return DefaultRequest(req);
            }
        }
        private static HttpResponseMessage DefaultRequest(HttpRequestMessage req)
        {
            return req.CreateResponse(HttpStatusCode.OK, new
            {
                version = "1.0",
                sessionAttributes = new { },
                response = new
                {
                    outputSpeech = new
                    {
                        type = "PlainText",
                        text = "Welcome to this calculator that only knows how to add two numbers.\n Ask add two plus three"
                    },
                    card = new
                    {
                        type = "Simple",
                        title = "Alexa-Azure Simple Calculator",
                        content = "Welcome to this calculator that only adds two number.\n Ask 2 + 3"
                    },
                    shouldEndSession = true
                }
            });
        }
    }
}