using AnalyticsDAL.Models;
using System.Threading.Tasks;

namespace AnalyticsDAL.Repositories
{
    public interface IListRepository
    {
        Task SaveListAsync(List view);
    }
}
