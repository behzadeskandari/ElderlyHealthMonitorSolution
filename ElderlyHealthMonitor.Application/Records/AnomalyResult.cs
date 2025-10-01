using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyHealthMonitor.Application.Records
{
    public record AnomalyResult(bool IsAnomaly, double Score);
}
