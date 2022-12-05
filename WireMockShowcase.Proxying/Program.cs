using WireMock.Server;
using WireMock.Settings;

var settings = new WireMockServerSettings
{
    Urls = new[] { "https://localhost:9095/" },
    StartAdminInterface = true,
    ProxyAndRecordSettings = new ProxyAndRecordSettings
    {
        Url = "https://dvlm-identity.api.sundiogroup.com",
        SaveMapping = true,
        SaveMappingToFile = true,
        SaveMappingForStatusCodePattern = "2xx"
    }
};

var server = WireMockServer.Start(settings);
Console.WriteLine("Started.. press key to stop");
Console.ReadKey(true);