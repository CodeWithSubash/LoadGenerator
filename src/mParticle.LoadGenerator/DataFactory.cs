using System;

using mParticle.LoadGenerator.Models;

namespace mParticle.LoadGenerator
{
    public static class DataFactory
    {
        const string NAME = "subashp";
        public static RequestData GenerateMockData()
        {
            return new RequestData()
            {
                Name = NAME,
                Date = DateTime.UtcNow,
            };
        }
    }
}
