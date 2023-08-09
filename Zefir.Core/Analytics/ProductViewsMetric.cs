using Zefir.Core.Entity;

namespace Zefir.Core.Analytics.Metrics;

public class Metric
{
    public int Id { get; set; }
    public string ActionCode { get; set; }
    public User User { get; set; }
    public string Result { get; set; }
}
