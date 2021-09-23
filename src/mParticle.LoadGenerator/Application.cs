using System;

using Newtonsoft.Json;

using mParticle.Core;
using mParticle.LoadGenerator.Models;

namespace mParticle.LoadGenerator
{
    public static class Application
    {
        public static void Start(Config config)
        {
            try
            {
                // 1. Create the service based on the config
                var awsService = new AwsLoadGeneratorService(config);

                // Use asynchronous API call
                //PerformASyncOperation(awsService);

                // PoC: Enable following code block to use synchronous API call
                PerformSyncOperation(awsService);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error while sending the http request", ex);
            }
        }

        #region Private Methods
        private static void PerformASyncOperation(AwsLoadGeneratorService awsService)
        {
            RequestData requestData2 = DataFactory.GenerateMockData();
            Logger.Debug($"REQUEST: {requestData2}");
            var result2 = awsService.SendRequestAsync(requestData2);
            var content2 = result2.Result;
            Logger.Debug($"RESULT: [{content2}]");
            ResponseData responseData2 = JsonConvert.DeserializeObject<ResponseData>(content2);
        }


        private static void PerformSyncOperation(AwsLoadGeneratorService awsService)
        {
            RequestData requestData1 = DataFactory.GenerateMockData();
            Logger.Debug($"REQUEST: {requestData1}");
            var content1 = awsService.SendRequest(requestData1);
            Logger.Debug($"RESPONSE: [{content1}]");
            ResponseData responseData1 = JsonConvert.DeserializeObject<ResponseData>(content1);
        }
        #endregion
    }
}
