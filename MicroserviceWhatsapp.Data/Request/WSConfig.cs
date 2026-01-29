
namespace MicroserviceWhatsapp.Data.Request
{
    public class WSConfig
    {
        public string Business_ID { get; set; } = string.Empty;
        public string Phone_Number_ID { get; set; } = string.Empty;
        public string User_Access_Token { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public string Metodo { get; set; } = string.Empty;

        public string ServiceIdentity { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string APICV { get;set; } = string.Empty;
        public string SocialMinerApi { get; set; } = string.Empty;
        public string SocialMinerFeedId { get; set; } = string.Empty;
        public string SocialMinerQueueId { get; set; } = string.Empty;
        public int MaxConcurrentChat { get; set; }
    }
}
