using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProviderTests.Middleware
{


public class ProviderStateMiddleware{
    private const string ConsumerName = "Consumer";
    private readonly RequestDelegate _next;
    private readonly IDictionary<string, Action> _providerStates;

     public ProviderStateMiddleware(RequestDelegate next)
        {
            _next = next;

            _providerStates = new Dictionary<string,Action>{
                {
                    "There is no data",
                    RemoveAllData
                },
                {
                    "There is data",
                    AddData
                }
            };
        }

    private void RemoveAllData(){
        string path = Path.Combine(Directory.GetCurrentDirectory(), @"../../../../data");
        var deletePath = Path.Combine(path, "somedata.txt");

        if (File.Exists(deletePath))
        {
            File.Delete(deletePath);
        }
    }

    private void AddData()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), @"../../../../data");
        var writePath = Path.Combine(path, "somedata.txt");

        if (!File.Exists(writePath))
        {
            File.Create(writePath);
        }
    }
    
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.Value == "/provider-states")
            {
                await this.HandleProviderStatesRequest(context);
                await context.Response.WriteAsync(String.Empty);
            }
            else
            {
                await this._next(context);
            }
        }


        private async Task HandleProviderStatesRequest(HttpContext context)
        {

            
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            if (context.Request.Method.ToUpper() == "Post".ToUpper() &&
                context.Request.Body != null)
            {
                
                var req = context.Request;
               
                string jsonRequestBody = String.Empty;
                using (var reader = new StreamReader(req.Body, Encoding.UTF8))
                {
                    jsonRequestBody = await reader.ReadToEndAsync();
                }
                //req.Body.Position = 0;

                var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

                //A null or empty provider state key must be handled
                if (providerState != null && !String.IsNullOrEmpty(providerState.State) &&
                    providerState.Consumer == ConsumerName)
                {
                    _providerStates[providerState.State].Invoke();
                }
            }
        }
 
    

}

}