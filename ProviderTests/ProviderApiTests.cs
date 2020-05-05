using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using Xunit;
using Xunit.Abstractions;


namespace ProviderTests
{
    public class ProviderApiTests: IDisposable {
         private string _providerUri { get; }
        private string _pactServiceUri { get; }
        private IHost _host { get; }
        private ITestOutputHelper _outputHelper { get; }

        public ProviderApiTests(ITestOutputHelper output)
        {
            _outputHelper = output;
            _providerUri = "https://localhost:5001";
            _pactServiceUri = "https://localhost:9001";

            _host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TestStartup>()
                    .UseUrls(_pactServiceUri);
                    
                }).Build();
          

            _host.Start();
        }


        

        [Fact]
        public void EnsureProviderApiHonoursPactWithConsumer()
        {
            // Arrange
    var config = new PactVerifierConfig
    {

        // NOTE: We default to using a ConsoleOutput,
        // however xUnit 2 does not capture the console output,
        // so a custom outputter is required.
        Outputters = new List<IOutput>
                        {
                            new XUnitOutput(_outputHelper)
                        },

        // Output verbose verification logs to the test output
        Verbose = true
    };

    //Act / Assert
    IPactVerifier pactVerifier = new PactVerifier(config);
    pactVerifier.ProviderState($"{_pactServiceUri}/provider-states")
        .ServiceProvider("Provider", _providerUri)
        .HonoursPactWith("Consumer")
        .PactUri(@"..\..\..\..\pacts\consumer-provider.json")
        .Verify();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _host.StopAsync(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();
                    _host.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }

}