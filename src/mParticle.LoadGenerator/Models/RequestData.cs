
using System;

namespace mParticle.LoadGenerator.Models
{
    public class RequestData
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int Requests_Sent { get; set; }

        // Overriding toString to be able to print out the object in a readable way
        public override string ToString()
        {
            return $"[Name: {Name}, Date: {Date.ToString("yyyy-MM-dd hh:mm:ss")}, Requests Sent: {Requests_Sent}]";
        }
    }
}
