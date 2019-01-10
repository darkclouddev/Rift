namespace Rift.Configuration
{
    public class App
    {
        public bool MaintenanceMode { get; set; } = true;
        public ulong MainGuildId { get; set; } = 0ul;
        public ulong TechGuildId { get; set; } = 0ul;
        public ulong EmoteGuildId { get; set; } = 0ul;
        public string RiotApiKey { get; set; } = "";
        public string LolVersion { get; set; } = "";
    }
}
