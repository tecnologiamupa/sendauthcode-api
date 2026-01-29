using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MicroserviceWhatsapp.Data.Request
{
    public class RequestUserLogin
    {
        [MaxLength(150)]
        public string? FullName { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        public string? Email { get; set; }

        public string? IdentityId { get; set; }

        [MaxLength(20)]
        [JsonIgnore]
        public string? PhoneNumber { get; set; }

        [MaxLength(10)]
        [JsonIgnore]
        public string? WhatsAppCode { get; set; }

        [MaxLength(50)]
        [JsonIgnore]
        public string? SocialProvider { get; set; }

        [MaxLength(255)]
        [JsonIgnore]
        public string? SocialId { get; set; }
    }
}
