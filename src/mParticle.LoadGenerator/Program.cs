using System;
using System.Threading;

using mParticle.Core;
using mParticle.LoadGenerator.Models;
using Newtonsoft.Json;

namespace mParticle.LoadGenerator
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            Logger.LogInfo($"LoadGenerator App started at {DateTime.UtcNow}");

            // Parse config file, if provided, else read the default one
            if (!ConfigParser.TryParse(args, out Config config))
            {
                Logger.LogError("Failed to parse configuration.");
                return;
            }

            try
            {
                //Get the Load Generator service created based on the config
                var awsService = new AwsLoadGeneratorService(config);

                // Initial Work: PoC using synchronous call
                //var content1 = awsService.SendRequest(DataFactory.GenerateMockData());
                //ResponseData responseData1 = JsonConvert.DeserializeObject<ResponseData>(content1);

                var result2 = awsService.SendRequestAsync(DataFactory.GenerateMockData());
                var content2 = result2.Result;
                Logger.Debug($"RESULT: [{content2}]");
                ResponseData responseData2 = JsonConvert.DeserializeObject<ResponseData>(content2);

            }
            catch (Exception ex)
            {
                Logger.LogError("Error while sending the http request", ex);
            }
            Logger.LogInfo($"LoadGenerator App ended at {DateTime.UtcNow}");
        }
    }
}
