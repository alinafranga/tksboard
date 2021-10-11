using AnalyticsDAL.Models;
using AnalyticsDomain.Models;
using System.Threading.Tasks;

namespace AnalyticsDAL.Repositories
{
    public interface IImpresionsRepository
    {
        Task SaveImpresionsAsync(ImpressionExt impression);
    }
}
