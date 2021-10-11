using AnalyticsDAL.Models;
using System.Threading.Tasks;

namespace AnalyticsDAL.Repositories
{
    public interface IViewRepository
    {
        Task SaveViewAsync(View view);
    }
}
