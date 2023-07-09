using System.IO;
using Microsoft.Extensions.Configuration;

namespace Generator.Infra
{
    public static class DefaultConfigurationBuilder
    {
        public static IConfigurationRoot Build()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            return builder.Build();
        }
    }
}