using System;

using mParticle.Core;

namespace mParticle.LoadGenerator
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            // Application Started 
            Logger.LogInfo($"APP START: LoadGenerator started at {DateTime.UtcNow}");

            // Parse config file, if provided, else read the default one
            if (!ConfigParser.TryParse(args, out Config config))
            {
                Logger.LogError("Failed to parse configuration.");
                return;
            }
            // Log the loaded configuration
            Logger.Debug($"CONFIG LOADED: {config}");

            // Invoke Main Application Block
            Application.Start(config);

            // Application Ended
            Logger.LogInfo($"APP END: LoadGenerator ended at {DateTime.UtcNow}");
        }
    }
}
