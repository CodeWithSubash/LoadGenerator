using mParticle.Core;

namespace mParticle.LoadGenerator
{
    public static class ConfigParser
    {
        const string CONFIG_FILE = GlobalConstants.DEFAULT_CONFIG_FILE;

        public static bool TryParse(string[] configArgs, out Config config)
        {
            string configFile = CONFIG_FILE;
            if (configArgs.Length > 0)
            {
                configFile = configArgs[0];
            }

            config = Config.GetArguments(configFile);
            return !(config == null);
        }
    }
}
