using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using mParticle.LoadGenerator.Models;
using Newtonsoft.Json;

namespace mParticle.LoadGenerator
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            // Parse config file, if provided, else read the default one
            if (!ConfigParser.TryParse(args, out Config config))
            {
                Console.WriteLine("Failed to parse configuration.");
                return;
            }

            try
            {
                //Get the Load Generator service created based on the config
                var awsService = new AwsLoadGeneratorService(config);

                // Initial Work: Using synchronous call
                // ResponseData responseData = awsService.SendRequest(DataFactory.GenerateMockData());

                // Improvised Work: Using Asynchronous call
                var result = awsService.SendRequestAsync(DataFactory.GenerateMockData());
                ResponseData responseData = JsonConvert.DeserializeObject<ResponseData>(result.Result);

                Console.WriteLine(responseData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending the http request. Message: {ex.Message}");
            }
            Console.ReadLine();
        }
    }
}
