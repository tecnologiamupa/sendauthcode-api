using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceWhatsapp.Domain.Models
{
    public class UserLogin
    {
        [Key]
        public int Id { get; set; }

        // Datos personales
        [MaxLength(150)]
        public string? FullName { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        public string? Email { get; set; }

        public string? IdentityId { get; set; }


        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        // Autenticación por WhatsApp
        [MaxLength(10)]
        public string? WhatsAppCode { get; set; }

        public DateTime? WhatsAppCodeSentAt { get; set; }

        public DateTime? WhatsAppCodeExpiresAt { get; set; }

        public bool WhatsAppVerified { get; set; } = false;

        // Login por redes sociales
        [MaxLength(50)]
        public string? SocialProvider { get; set; } // google, facebook, apple

        [MaxLength(255)]
        public string? SocialId { get; set; }

        // Control de acceso y auditoría
        public bool IsActive { get; set; } = true;

        public DateTime? LastLogin { get; set; }

        public int LoginAttempts { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
