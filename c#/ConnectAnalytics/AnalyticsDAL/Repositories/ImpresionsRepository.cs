using AnalyticsDAL.Models;
using AnalyticsDomain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnalyticsDAL.Repositories
{
    public class ImpresionsRepository : IImpresionsRepository
    {
        private readonly AnalyticsContext _analyticsContext;
        public ImpresionsRepository(AnalyticsContext analyticsContext)
        {
            _analyticsContext = analyticsContext;
        }
        public async Task SaveImpresionsAsync(ImpressionExt impression)
        {
            var impressionList = new List<Impression>();
            foreach(var weed in impression.WeedIds)
            {
                impressionList.Add(new Impression()
                {
                    IpAddress = impression.IpAddress,
                    MeasuredDate = (impression.MeasuredDate == null ? DateTime.UtcNow : impression.MeasuredDate.Value),
                    MetricCount = impression.MetricCount,
                    ProducerConnectUserId = impression.ProducerConnectUserId,
                    WeedId = weed.WeedId,
                    WeightedCount = impression.WeightedCount,
                    IsBasicProduct = impression.IsBasicProduct,
                    ProvinceId = impression.ProvinceId,
                    ProducerId = weed.ProducerId
                });
            }
            await _analyticsContext.Impression.AddRangeAsync(impressionList);
            await _analyticsContext.SaveChangesAsync();
        }
    }
}
