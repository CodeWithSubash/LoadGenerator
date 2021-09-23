using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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

                // Limiting the program to run for an Hour
                // TODO: This can be easily configurable as the argument option to the application 
                DateTime appEndTime = DateTime.Now.AddHours(1);
                while (DateTime.Now < appEndTime)
                {
                    ExecuteOperationPerSecond(awsService, config.TargetRPS);
                }
            }
            catch (Exception exception)
            {
                Logger.LogError("Error while sending the http request", exception);
            }
        }

        #region Private Methods

        private static void ExecuteOperationPerSecond(AwsLoadGeneratorService awsService, uint targetRPS)
        {
            int currentRPS = 0;
            DateTime oneSecond = DateTime.Now.AddSeconds(1);
            // Dictionary<string, int> statusCodeDictionary = new Dictionary<string, int>();
            while (DateTime.Now < oneSecond)
            {
                currentRPS++;
                PerformASyncOperation(awsService);
                // Disabling for now as it introduced lots of lagging 
                //if(!statusCodeDictionary.ContainsKey(responseCode))
                //{
                //    statusCodeDictionary.Add(responseCode, 0);
                //}
                //statusCodeDictionary[responseCode] = 1 + statusCodeDictionary[responseCode];
                
                // NOTE: The good tradeoff can be done by making some milliseconds of delay between each requests
                // Tried  different options from 10ms to 0/1/2 ms; the optimal solution was around 4-5ms
                Thread.Sleep(4);
            }
            Logger.LogInfo($"Target RPS: {targetRPS}, Current RPS: {currentRPS}");
            //ShowStatusCodeMetrices(statusCodeDictionary);
        }

        private static void ShowStatusCodeMetrices(Dictionary<string, int> statusCodeDictionary)
        {
            StringBuilder str = new StringBuilder("METRICES: ");
            foreach (KeyValuePair<string, int> kvp in statusCodeDictionary)
            {
                str.Append(kvp.Key + ":" + kvp.Value + " ");
            }
            Logger.LogInfo(str.ToString());
        }

        private static void PerformASyncOperation(AwsLoadGeneratorService awsService)
        {
            RequestData requestData = DataFactory.GenerateMockData();
            Logger.Debug($"REQUEST: {requestData}");
            var result = awsService.SendRequestAsync(requestData);

            // INFO: No requirement to return the content results
            //var result = awsService.SendRequestAsync(requestData);
            //if (result != null)
            //{
            //    var content = result.Result;
            //    Logger.Debug($"RESULT: [{content}]");
            //    ResponseData responseData = JsonConvert.DeserializeObject<ResponseData>(content);
            //}
        }

        // WIP: Store the all metrices for the response status codes
        // Currently disabled, as it introduced lots of lagging
        private static Dictionary<int, int> GetEmptyHttpStatusCodeDictionary()
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            dict.Add(200, 0);
            dict.Add(429, 0);
            dict.Add(500, 0);
            dict.Add(301, 0);
            dict.Add(301, 0);
            return dict;
        }

        // This is just provided as a Proof of Concept to observe the synchronous call behavior
        // Need to call this method 'PerformSyncOperation(awsService);' from Start() method
        private static void PerformSyncOperation(AwsLoadGeneratorService awsService)
        {
            RequestData requestData = DataFactory.GenerateMockData();
            Logger.Debug($"REQUEST: {requestData}");
            var content = awsService.SendRequest(requestData);
            if (content != null)
            {
                Logger.Debug($"RESPONSE: [{content}]");
                // ResponseData responseData = JsonConvert.DeserializeObject<ResponseData>(content);
            }
        }
        #endregion
    }
}
