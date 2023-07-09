using System.IO;
using Microsoft.Extensions.Configuration;

namespace Sorter.Infra
{
    public static class DefaultConfigurationBuilder
    {
        public static IConfigurationRoot Build()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }
    }
}