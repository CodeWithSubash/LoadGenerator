using System;
using System.Threading.Tasks;

using mParticle.LoadGenerator.Models;

namespace mParticle.LoadGenerator
{
    public interface IService
    {
        public string SendRequest(RequestData requestData);
        public Task<string> SendRequestAsync(RequestData requestData);
    }
}
