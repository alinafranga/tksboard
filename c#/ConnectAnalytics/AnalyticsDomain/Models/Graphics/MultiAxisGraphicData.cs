using System.Collections.Generic;
namespace AnalyticsDomain.Models.Graphics
{
    public class MultiAxisGraphicData
    {
        public List<GraphicData> CurrentPeriod { get; set; }
        public List<GraphicData> PreviousPeriod { get; set; }
    }
}
