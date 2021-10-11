using AnalyticsDAL.Models;
using EFCore.BulkExtensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnalyticsDAL.Repositories
{
    public class FollowRepository : IFollowRepository
    {
        private readonly AnalyticsContext _analyticsContext;
        public FollowRepository(AnalyticsContext analyticsContext)
        {
            _analyticsContext = analyticsContext;
        }
        public async Task SaveFollowAsync(Follow follow)
        {
            follow.MeasuredDate = follow.MeasuredDate != DateTime.MinValue ? follow.MeasuredDate : DateTime.UtcNow;
            await _analyticsContext.Follow.AddAsync(follow);
            await _analyticsContext.SaveChangesAsync();
        }

        public void SaveFollows(List<Follow> follows)
        {
            _analyticsContext.BulkInsert(follows);
        }
    }
}
