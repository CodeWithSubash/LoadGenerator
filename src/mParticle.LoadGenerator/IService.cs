using mParticle.LoadGenerator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace mParticle.LoadGenerator
{
    public interface IService
    {
        public ResponseData SendRequest(RequestData requestData);
    }
}
