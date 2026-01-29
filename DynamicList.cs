using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroserviceWhatsapp.Domain.Models
{
    public enum DynamicListType
    {
        List,
        Button
    }

    public class DynamicList
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FlowStepId { get; set; }

        [MaxLength(255)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public DynamicListType Type { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
