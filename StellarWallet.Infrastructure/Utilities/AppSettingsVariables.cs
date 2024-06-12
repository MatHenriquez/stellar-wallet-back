using Microsoft.Extensions.Configuration;

namespace StellarWallet.Infrastructure.Utilities
{
    public static class AppSettingsVariables
    {
        public static IConfigurationRoot BuildConfig(string? environment)
        {
            string settedEnvironment = SetEnvironment(environment);

            string? directory;

            if (settedEnvironment == "test")
                directory = Directory.GetParent(Directory.GetCurrentDirectory()).ToString() + "\\..\\..\\..\\StellarWallet.WebApi";

            else
                directory = Directory.GetParent(Directory.GetCurrentDirectory()).ToString() + "\\StellarWallet.WebApi";
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(directory);

            if (File.Exists($"appsettings.{settedEnvironment}.json"))
                configurationBuilder.AddJsonFile($"appsettings.{settedEnvironment}.json", optional: false);
            else
                throw new Exception("Configuration file not found");

            return configurationBuilder.Build();
        }

        public static string GetConnectionString(string? environment)
        {
            return BuildConfig(SetEnvironment(environment)).GetConnectionString("StellarWallet") ?? throw new Exception("Undefined connection string");
        }

        public static string GetSettingVariable(string sectionName, string variableName, string? environment)
        {
            return BuildConfig(SetEnvironment(environment)).GetSection(sectionName).GetValue<string>(variableName) ?? throw new Exception("Undefined variable");
        }

        private readonly static Func<string?, string> SetEnvironment = (string? environment) =>
        {
            if (string.IsNullOrEmpty(environment))
                return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "test";
            else
                return environment;
        };
    }
}
