
using System;

namespace mParticle.LoadGenerator.Models
{
    public class ResponseData
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }

        // Overriding toString to be able to print out the object in a readable way
        public override string ToString()
        {
            return $"Successful: {Successful}, Error: {Error}, Message: {Message}";
        }
    }
}
