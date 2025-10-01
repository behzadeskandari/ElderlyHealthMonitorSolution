using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyHealthMonitor.Domain.Entities
{
    public class BehavioralPattern
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ElderlyProfileId { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;
        public string FeatureVectorJson { get; set; } = string.Empty; // ML features
        public string Summary { get; set; } = string.Empty;
        public double AnomalyScore { get; set; } = 0;
        public ElderlyProfile ElderlyProfile { get; set; } = null!;
    }
}
