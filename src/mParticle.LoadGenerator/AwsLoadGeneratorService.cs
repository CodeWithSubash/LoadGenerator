using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using mParticle.Core;
using mParticle.LoadGenerator.Models;
using System.Net;
using System.Threading;

namespace mParticle.LoadGenerator
{
    using HttpHeaders = System.Net.Http.Headers;
    public class AwsLoadGeneratorService : IService
    {
        static HttpClient httpClient = new HttpClient();

        public AwsLoadGeneratorService(Config config)
        {
            httpClient.BaseAddress = new Uri(config.ServerURL.TrimEnd('/'));
            httpClient.DefaultRequestHeaders.Authorization = new HttpHeaders.AuthenticationHeaderValue(GlobalConstants.X_API_KEY, config.AuthKey);
            httpClient.DefaultRequestHeaders.Add(GlobalConstants.X_API_KEY, config.AuthKey);
            // ACCEPT header
            httpClient.DefaultRequestHeaders.Accept.Add(new HttpHeaders.MediaTypeWithQualityHeaderValue(GlobalConstants.MEDIATYPE_JSON));
            // Override the default 100 secs to 30 secs
            httpClient.Timeout = TimeSpan.FromSeconds(GlobalConstants.DEFAULT_TIMEOUT);
        }


        public async Task<string> SendRequestAsync(RequestData requestData)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(httpClient.BaseAddress, GetSerializedContent(requestData));
                HttpStatusCode statusCode = response.StatusCode;
                Logger.LogInfo($"{statusCode} ({(int)statusCode})");
                if (response.IsSuccessStatusCode)
                {
                    Logger.Debug($"Response Success from API: '{httpClient.BaseAddress}'. Status: {statusCode} ({(int)statusCode})");
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Control the server overwhelming from newer requests
                    if(statusCode == HttpStatusCode.TooManyRequests)
                    {
                        PerformExponentialBackoff();
                    }
                    Logger.Debug($"Response Failed from API: '{httpClient.BaseAddress}'. Reason: {statusCode} ({(int)statusCode})");
                }
            }
            catch(HttpRequestException requestException)
            {
                Logger.LogError("HttpClient Error", requestException);
                PerformExponentialBackoff();
            }
            catch (Exception exception)
            {
                Logger.LogError("Error while sending the http request.", exception);
            }
            return null;
        }


        public string SendRequest(RequestData requestData)
        {
            try
            {
                var response = httpClient.PostAsync(httpClient.BaseAddress, GetSerializedContent(requestData));
                if (response != null && response.Result.IsSuccessStatusCode)
                {
                    Logger.Debug($"Successful response received for request to AWS API: '{httpClient.BaseAddress}'");
                    var content = response.Result.Content.ReadAsStringAsync().Result;
                    return content;
                }
                else
                {
                    Logger.LogWarning($"Unable to receive a success response from AWS API. StatusCode: '{(int)response?.Status}'");
                }
            }
            catch (Exception exception)
            {
                Logger.LogError("Error while sending the http request.", exception);
            }
            return null;
        }

        #region Private Helper methods
        private StringContent GetSerializedContent(RequestData requestData)
        {
            return new StringContent(
                    JsonConvert.SerializeObject(requestData),
                    Encoding.UTF8,
                    GlobalConstants.MEDIATYPE_JSON);
        }

        // TODO: Modify this method to use exponential backoff technique
        // Ref: https://docs.aws.amazon.com/general/latest/gr/api-retries.html
        private void PerformExponentialBackoff()
        {
            Thread.Sleep(10);
        }
        #endregion
    }
}
