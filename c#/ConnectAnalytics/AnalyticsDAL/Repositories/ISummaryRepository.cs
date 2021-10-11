using System.Collections.Generic;

namespace AnalyticsDAL.Repositories
{
    public interface ISummaryRepository
    {
        int SaveDay(List<int> producerIds, int day, int month, int year);
        int SaveMonth(List<int> producerIds, int month, int year);
        int SaveYear(List<int> producerIds, int year);
    }
}
