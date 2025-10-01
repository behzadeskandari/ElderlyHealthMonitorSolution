using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyHealthMonitor.Domain.Entities
{
    public class User
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();
        [Required][MaxLength(100)] public string Name { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = "Caregiver"; // Caregiver/Admin
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ElderlyProfile> ElderlyProfiles { get; set; } = new List<ElderlyProfile>();
        public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
    }
}
