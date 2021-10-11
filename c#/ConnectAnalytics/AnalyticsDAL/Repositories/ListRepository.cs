using AnalyticsDAL.Models;
using System;
using System.Threading.Tasks;

namespace AnalyticsDAL.Repositories
{
    public class ListRepository: IListRepository
    {
        private readonly AnalyticsContext _analyticsContext;
        public ListRepository(AnalyticsContext analyticsContext)
        {
            _analyticsContext = analyticsContext;
        }
        public async Task SaveListAsync(List list)
        {
            list.MeasuredDate = list.MeasuredDate != DateTime.MinValue ? list.MeasuredDate : DateTime.UtcNow;
            await _analyticsContext.List.AddAsync(list);
            await _analyticsContext.SaveChangesAsync();
        }
    }
}
