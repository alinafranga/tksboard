using AnalyticsDAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnalyticsDAL.Repositories
{
    public interface IFollowRepository
    {
        Task SaveFollowAsync(Follow follow);
        void SaveFollows(List<Follow> follows);
    }
}
