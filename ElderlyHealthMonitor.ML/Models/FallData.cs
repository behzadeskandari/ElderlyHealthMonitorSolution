using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace ElderlyHealthMonitor.ML.Models
{
    public class FallData
    {
        [LoadColumn(0)]
        public bool PreviousFall { get; set; }

        [LoadColumn(1)]
        public bool FOG { get; set; }

        [LoadColumn(2)]
        public bool SlowGait { get; set; }

        [LoadColumn(3)]
        [ColumnName("Label")]
        public bool IsFall { get; set; }
    }
}
