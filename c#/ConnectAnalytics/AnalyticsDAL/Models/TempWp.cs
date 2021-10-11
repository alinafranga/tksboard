using System;
using System.Collections.Generic;

#nullable disable

namespace AnalyticsDAL.Models
{
    public partial class TempWp
    {
        public int Id { get; set; }
        public int ProducerId { get; set; }
        public int WeedId { get; set; }
    }
}
