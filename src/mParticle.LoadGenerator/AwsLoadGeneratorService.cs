using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using mParticle.Core;
using mParticle.LoadGenerator.Models;
using Newtonsoft.Json;

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
            // Override the default 100 secs to 60
            httpClient.Timeout = TimeSpan.FromSeconds(GlobalConstants.DEFAULT_TIMEOUT);
        }


        public async Task<string> SendRequestAsync(RequestData request)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(httpClient.BaseAddress, GetSerializedContent(request));
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Successful response received for request to AWS API: '{httpClient.BaseAddress}'");
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine($"Unable to receive a success response from AWS API. StatusCode: '{(int)response?.StatusCode}'");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending the http request. Message: {ex.Message}");
            }
            return null;
        }


        public ResponseData SendRequest(RequestData request)
        {
            ResponseData responseData = null;
            try
            {
                var response = httpClient.PostAsync(httpClient.BaseAddress, GetSerializedContent(request));
                if (response != null && response.Result.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Successful response received for request to AWS API: '{httpClient.BaseAddress}'");
                    var content = response.Result.Content.ReadAsStringAsync().Result;
                    responseData = JsonConvert.DeserializeObject<ResponseData>(content);
                }
                else
                {
                    Console.WriteLine($"Unable to receive a success response from AWS API. StatusCode: '{(int)response?.Status}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending the http request. Message: {ex.Message}");
            }
            return responseData;
        }

        #region Private Helper methods
        private StringContent GetSerializedContent(RequestData requestData)
        {
            return new StringContent(
                    JsonConvert.SerializeObject(requestData),
                    Encoding.UTF8,
                    GlobalConstants.MEDIATYPE_JSON);
        }
        #endregion
    }
}
