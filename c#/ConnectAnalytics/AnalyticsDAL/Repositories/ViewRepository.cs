using AnalyticsDAL.Models;
using System;
using System.Threading.Tasks;

namespace AnalyticsDAL.Repositories
{
    public class ViewRepository : IViewRepository
    {
        private readonly AnalyticsContext _analyticsContext;
        public ViewRepository(AnalyticsContext analyticsContext)
        {
            _analyticsContext = analyticsContext;
        }
        public async Task SaveViewAsync(View view)
        {
            view.MeasuredDate = view.MeasuredDate != DateTime.MinValue? view.MeasuredDate: DateTime.UtcNow;
            await _analyticsContext.View.AddAsync(view);
            await _analyticsContext.SaveChangesAsync();
        }
    }
}
