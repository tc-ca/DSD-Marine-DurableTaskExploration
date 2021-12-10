using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayApi.Configuration
{
    public static class HubConfig
    {
        // Using .NET configuration is a certainly better way to do this...
        public const string ServiceBusConnectionString = "Endpoint=sb://vesselregistrydtfsagas.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=V+AHptBQkYzJzQQzMSRPk7iZaET2naeaiPobmQESMmo=";
        public const string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=vesselregistrydtfsagas;AccountKey=iAMkXKUUdiDCE1HkdrxMmnkFJ45SiKDuVZbcBOOsrZXqKxWEg0yyyTRIEHC5MBoT4nmkkm6aQJpUOZNL1q6otA==;EndpointSuffix=core.windows.net";
        public const string HubName = "firstregistry";
    }
}
