using System;

using Newtonsoft.Json;

using mParticle.Core;
using mParticle.LoadGenerator.Models;
using System.Threading;
using System.Threading.Tasks;

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

                // Limiting the program to run for an Hour
                // TODO: This can be easily configurable as the argument option to the application 
                DateTime endTime = DateTime.Now.AddHours(1);
                while (DateTime.Now < endTime)
                {
                    int currentRPS = ExecuteOperationPerSecond(awsService);
                    Logger.LogInfo($"Target RPS: {config.TargetRPS}, Current RPS: {currentRPS}");
                }
            }
            catch (Exception exception)
            {
                Logger.LogError("Error while sending the http request", exception);
            }
        }

        #region Private Methods

        private static int ExecuteOperationPerSecond(AwsLoadGeneratorService awsService)
        {
            int currentRPS = 0;
            DateTime endTime = DateTime.Now.AddSeconds(1);
            while (DateTime.Now < endTime)
            {
                currentRPS++;
                PerformASyncOperation(awsService);
                // NOTE: The good tradeoff can be done by making some milliseconds of delay in the request calls
                // Tried  different options from 10ms to 0/1/2 ms; the optimal solution was around 4-5ms
                Thread.Sleep(4); 
            }
            return currentRPS;
        }
        private static void PerformASyncOperation(AwsLoadGeneratorService awsService)
        {
            RequestData requestData = DataFactory.GenerateMockData();
            Logger.Debug($"REQUEST: {requestData}");
            Task.Run(async () =>
            {
                await awsService.SendRequestAsync(requestData);
            }).GetAwaiter().GetResult();

            // INFO: No requirement to return the content results
            //var result = awsService.SendRequestAsync(requestData);
            //if (result != null)
            //{
            //    var content = result.Result;
            //    Logger.Debug($"RESULT: [{content}]");
            //    ResponseData responseData = JsonConvert.DeserializeObject<ResponseData>(content);
            //}
        }


        // This is just provided as a Proof of Concept to observe the synchronous call behavior
        // Need to call this method 'PerformSyncOperation(awsService);' from Start() method
        private static void PerformSyncOperation(AwsLoadGeneratorService awsService)
        {
            RequestData requestData = DataFactory.GenerateMockData();
            Logger.Debug($"REQUEST: {requestData}");
            var content = awsService.SendRequest(requestData);
            if(content != null)
            {
                Logger.Debug($"RESPONSE: [{content}]");
                // ResponseData responseData = JsonConvert.DeserializeObject<ResponseData>(content);
            }
        }
        #endregion
    }
}
