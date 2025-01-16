namespace IP_MVC.Models
{
    public class FlowAnalyticsDataModel
    {
        public int FlowId { get; set; }
        public IEnumerable<object> ChartData { get; set; }
    }
}