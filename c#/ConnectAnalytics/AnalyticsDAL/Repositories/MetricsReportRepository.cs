using AnalyticsDAL.Helpers;
using AnalyticsDAL.Helpers.Enums;
using AnalyticsDAL.Models;
using AnalyticsDomain.Models;
using AnalyticsDomain.Models.Graphics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsDAL.Repositories
{
    public class MetricsReportRepository : IMetricsReportRepository
    {
        private readonly AnalyticsContext _analyticsContext;
        public MetricsReportRepository(AnalyticsContext analyticsContext)
        {
            _analyticsContext = analyticsContext;
        }

        public List<GraphicData> GetAvailableMonths(int userId, int provinceId)
        {
            var currentYear = DateTime.Now.Year;
            var availableMonths = _analyticsContext.SummaryMonth.Where(x => x.ProducerId == userId && (provinceId == -1 || x.ProvinceId == provinceId) && x.Year == currentYear)
                .Select(x=>x.Month).Distinct().OrderByDescending(x=>x).ToList();
            var graphics = new List<GraphicData>();
            foreach (var month in availableMonths)
            {
                var date = new DateTime(currentYear, month, 1);
                graphics.Add(new GraphicData
                {
                    Label = date.ToString("MMM yyyy"),
                    Attribute = date.Month + "/" + date.Year,
                    Date = date
                });
            }
            return graphics;
        }

        public OverviewMetricsInfo GetOverviewByProductAnalytics(int weedId, int provinceId, int month, int year)
        {
            OverviewMetricData impressions = null;
            OverviewMetricData views = null;
            OverviewMetricData lists = null;
            OverviewMetricData follows = null;
            if (month != 0 && year != 0)
            {
                var currenDate = DateTime.Now;
                var startDate = new DateTime(year, month, 1);
                var endDate = new DateTime(year, month, 1).AddMonths(1).AddMilliseconds(-1);

                var startDatePrevious = new DateTime(year, month, 1).AddMonths(-1);
                var endDatePrevious = endDate.AddMonths(-1);

                if (currenDate.Month == month && currenDate.Year == year)
                {
                    startDate = new DateTime(year, month, 1);
                    endDate = new DateTime(year, month, currenDate.Day);
                }

                var summaryDayList = _analyticsContext.SummaryProductDay.Where(x => x.WeedId == weedId && x.Date>= startDate && x.Date <= endDate && (provinceId == -1 || x.ProvinceId == provinceId))
               .AsNoTracking().ToList();
                var summaryDayPreviousList = _analyticsContext.SummaryProductDay.Where(x => x.WeedId == weedId && x.Date >= startDatePrevious && x.Date <= endDatePrevious && (provinceId == -1 || x.ProvinceId == provinceId))
                    .AsNoTracking().ToList();

                var totalSummaryMonth = new SummaryMonth();
                foreach (var summaryMonth in summaryDayList)
                {
                    totalSummaryMonth.ImpressionNumber = totalSummaryMonth.ImpressionNumber + summaryMonth.ImpressionNumber;
                    totalSummaryMonth.ViewsNumber = totalSummaryMonth.ViewsNumber + summaryMonth.ViewsNumber;
                    totalSummaryMonth.ListNumber = totalSummaryMonth.ListNumber + summaryMonth.ListNumber;
                    totalSummaryMonth.FollowsNumber = totalSummaryMonth.FollowsNumber.GetValueOrDefault(0) + summaryMonth.FollowsNumber.GetValueOrDefault(0);
                }

                var totalSummaryMonthPrevious = new SummaryMonth();
                foreach (var summaryMonth in summaryDayPreviousList)
                {
                    totalSummaryMonthPrevious.ImpressionNumber = totalSummaryMonthPrevious.ImpressionNumber + summaryMonth.ImpressionNumber;
                    totalSummaryMonthPrevious.ViewsNumber = totalSummaryMonthPrevious.ViewsNumber + summaryMonth.ViewsNumber;
                    totalSummaryMonthPrevious.ListNumber = totalSummaryMonthPrevious.ListNumber + summaryMonth.ListNumber;
                    totalSummaryMonthPrevious.FollowsNumber = totalSummaryMonthPrevious.FollowsNumber.GetValueOrDefault(0) + summaryMonth.FollowsNumber.GetValueOrDefault(0);
                }

                impressions = new OverviewMetricData() { TotalMetric = totalSummaryMonth.ImpressionNumber };
                if (totalSummaryMonthPrevious.ImpressionNumber > 0)
                {
                    decimal percentageDifferenceImp = ((decimal)totalSummaryMonth.ImpressionNumber - (decimal)totalSummaryMonthPrevious.ImpressionNumber) / (decimal)totalSummaryMonthPrevious.ImpressionNumber * 100;
                    impressions = new OverviewMetricData() { TotalMetric = totalSummaryMonth.ImpressionNumber, TotalPreviousPeriodPercentage = percentageDifferenceImp };
                }

                views = new OverviewMetricData() { TotalMetric = totalSummaryMonth.ViewsNumber };
                if (totalSummaryMonthPrevious.ViewsNumber > 0)
                {
                    var percentageDifferenceView = ((decimal)totalSummaryMonth.ViewsNumber - (decimal)totalSummaryMonthPrevious.ViewsNumber) / (decimal)totalSummaryMonthPrevious.ViewsNumber * 100;
                    views = new OverviewMetricData() { TotalMetric = totalSummaryMonth.ViewsNumber, TotalPreviousPeriodPercentage = percentageDifferenceView };
                }

                lists = new OverviewMetricData() { TotalMetric = totalSummaryMonth.ListNumber };
                if (totalSummaryMonthPrevious.ListNumber > 0)
                {
                    var percentageDifferenceList = ((decimal)totalSummaryMonth.ListNumber - (decimal)totalSummaryMonthPrevious.ListNumber) / (decimal)totalSummaryMonthPrevious.ListNumber * 100;
                    lists = new OverviewMetricData() { TotalMetric = totalSummaryMonth.ListNumber, TotalPreviousPeriodPercentage = percentageDifferenceList };
                }

                var lastDayWithData = _analyticsContext.SummaryProductDay.OrderByDescending(x => x.Date).FirstOrDefault(x => x.WeedId == weedId && x.Date.Month == endDate.Month && x.Date.Year == endDate.Year);
                var lastDayWithDataPrevious = _analyticsContext.SummaryProductDay.OrderByDescending(x => x.Date).FirstOrDefault(x => x.WeedId == weedId && x.Date.Month == endDatePrevious.Month && x.Date.Year == endDatePrevious.Year);
                DateTime followDateToCheck = endDate;
                DateTime followDateToCheckPrevious = endDatePrevious;
                if (lastDayWithData != null)
                {
                    followDateToCheck = lastDayWithData.Date;
                }
                if (lastDayWithDataPrevious != null)
                {
                    followDateToCheckPrevious = lastDayWithDataPrevious.Date;
                }

                var summaryDayListFollow = _analyticsContext.SummaryProductDay.Where(x => x.WeedId == weedId && x.Date.Date == followDateToCheck.Date && (provinceId == -1 || x.ProvinceId == provinceId))
              .AsNoTracking().ToList();
                var summaryDayPreviousListFollow = _analyticsContext.SummaryProductDay.Where(x => x.WeedId == weedId && x.Date.Date == followDateToCheckPrevious.Date && (provinceId == -1 || x.ProvinceId == provinceId))
                    .AsNoTracking().ToList();

                totalSummaryMonth = new SummaryMonth();
                foreach (var summaryDay in summaryDayListFollow)
                {
                    totalSummaryMonth.FollowsNumber = totalSummaryMonth.FollowsNumber.GetValueOrDefault(0) + summaryDay.FollowsNumber.GetValueOrDefault(0);
                }

                totalSummaryMonthPrevious = new SummaryMonth();
                foreach (var summaryDay in summaryDayPreviousListFollow)
                {
                    totalSummaryMonthPrevious.FollowsNumber = totalSummaryMonthPrevious.FollowsNumber.GetValueOrDefault(0) + summaryDay.FollowsNumber.GetValueOrDefault(0);
                }

                follows = new OverviewMetricData() { TotalMetric = totalSummaryMonth.FollowsNumber.GetValueOrDefault(0) };
                if (totalSummaryMonthPrevious.FollowsNumber > 0)
                {
                    var percentageDifferenceFollow = ((decimal)totalSummaryMonth.FollowsNumber - (decimal)totalSummaryMonthPrevious.FollowsNumber) / (decimal)totalSummaryMonthPrevious.FollowsNumber * 100;
                    follows = new OverviewMetricData() { TotalMetric = totalSummaryMonth.FollowsNumber.GetValueOrDefault(0), TotalPreviousPeriodPercentage = percentageDifferenceFollow };
                }
            }
            else
            {
                var summaryYearList = _analyticsContext.SummaryProductYear.Where(x => x.WeedId == weedId && x.Year == DateTime.Now.Year && (provinceId == -1 || x.ProvinceId == provinceId))
              .AsNoTracking().ToList();

                var totalSummaryYear = new SummaryYear();
                foreach (var summaryMonth in summaryYearList)
                {
                    totalSummaryYear.ImpressionNumber = totalSummaryYear.ImpressionNumber + summaryMonth.ImpressionNumber;
                    totalSummaryYear.ViewsNumber = totalSummaryYear.ViewsNumber + summaryMonth.ViewsNumber;
                    totalSummaryYear.ListNumber = totalSummaryYear.ListNumber + summaryMonth.ListNumber;
                    totalSummaryYear.FollowsNumber = totalSummaryYear.FollowsNumber.GetValueOrDefault(0) + summaryMonth.FollowsNumber.GetValueOrDefault(0);
                }
                impressions = new OverviewMetricData() { TotalMetric = totalSummaryYear.ImpressionNumber };
                views = new OverviewMetricData() { TotalMetric = totalSummaryYear.ViewsNumber };
                lists = new OverviewMetricData() { TotalMetric = totalSummaryYear.ListNumber };
                follows = new OverviewMetricData() { TotalMetric = totalSummaryYear.FollowsNumber.GetValueOrDefault(0) };
            }


            return new OverviewMetricsInfo() { Impressions = impressions, Lists = lists, Views = views, Follows = follows };
        }

        public OverviewMetricsInfo GetOverviewProducerAnalytics(int producerId, int provinceId, int month, int year)
        {
            OverviewMetricData impressions = null;
            OverviewMetricData views = null;
            OverviewMetricData lists = null;
            OverviewMetricData follows = null;
            if (month != 0 && year != 0)
            {
                var currenDate = DateTime.Now;
                var startDate = new DateTime(year, month, 1);
                var endDate = new DateTime(year, month, 1).AddMonths(1).AddMilliseconds(-1);

                var startDatePrevious = new DateTime(year, month, 1).AddMonths(-1);
                var endDatePrevious = endDate.AddMonths(-1);

                if (currenDate.Month == month && currenDate.Year == year)
                {
                    startDate = new DateTime(year, month, 1);
                    endDate = new DateTime(year, month, currenDate.Day);

                    startDatePrevious = new DateTime(year, month, 1).AddMonths(-1);
                    endDatePrevious = new DateTime(year, month, currenDate.Day).AddMonths(-1);
                }

                var summaryDayList = _analyticsContext.SummaryDay.Where(x => x.ProducerId == producerId && x.Date >= startDate && x.Date <= endDate && (provinceId == -1 || x.ProvinceId == provinceId))
               .AsNoTracking().ToList();
                var summaryDayPreviousList = _analyticsContext.SummaryDay.Where(x => x.ProducerId == producerId && x.Date >= startDatePrevious && x.Date <= endDatePrevious && (provinceId == -1 || x.ProvinceId == provinceId))
                    .AsNoTracking().ToList();

                var totalSummaryMonth = new SummaryMonth();
                foreach (var summaryMonth in summaryDayList)
                {
                    totalSummaryMonth.ImpressionNumber = totalSummaryMonth.ImpressionNumber + summaryMonth.ImpressionNumber;
                    totalSummaryMonth.ViewsNumber = totalSummaryMonth.ViewsNumber + summaryMonth.ViewsNumber;
                    totalSummaryMonth.ListNumber = totalSummaryMonth.ListNumber + summaryMonth.ListNumber;
                    totalSummaryMonth.FollowsNumber = totalSummaryMonth.FollowsNumber.GetValueOrDefault(0) + summaryMonth.FollowsNumber.GetValueOrDefault(0);
                }

                var totalSummaryMonthPrevious = new SummaryMonth();
                foreach (var summaryMonth in summaryDayPreviousList)
                {
                    totalSummaryMonthPrevious.ImpressionNumber = totalSummaryMonthPrevious.ImpressionNumber + summaryMonth.ImpressionNumber;
                    totalSummaryMonthPrevious.ViewsNumber = totalSummaryMonthPrevious.ViewsNumber + summaryMonth.ViewsNumber;
                    totalSummaryMonthPrevious.ListNumber = totalSummaryMonthPrevious.ListNumber + summaryMonth.ListNumber;
                    totalSummaryMonthPrevious.FollowsNumber = totalSummaryMonthPrevious.FollowsNumber.GetValueOrDefault(0) + summaryMonth.FollowsNumber.GetValueOrDefault(0);
                }

                impressions = new OverviewMetricData() { TotalMetric = totalSummaryMonth.ImpressionNumber };
                if (totalSummaryMonthPrevious.ImpressionNumber > 0)
                {
                    decimal percentageDifferenceImp = ((decimal)totalSummaryMonth.ImpressionNumber - (decimal)totalSummaryMonthPrevious.ImpressionNumber) / (decimal)totalSummaryMonthPrevious.ImpressionNumber * 100;
                    impressions = new OverviewMetricData() { TotalMetric = totalSummaryMonth.ImpressionNumber, TotalPreviousPeriodPercentage = percentageDifferenceImp };
                }

                views = new OverviewMetricData() { TotalMetric = totalSummaryMonth.ViewsNumber };
                if (totalSummaryMonthPrevious.ViewsNumber > 0)
                {
                    var percentageDifferenceView = ((decimal)totalSummaryMonth.ViewsNumber - (decimal)totalSummaryMonthPrevious.ViewsNumber) / (decimal)totalSummaryMonthPrevious.ViewsNumber * 100;
                    views = new OverviewMetricData() { TotalMetric = totalSummaryMonth.ViewsNumber, TotalPreviousPeriodPercentage = percentageDifferenceView };
                }

                lists = new OverviewMetricData() { TotalMetric = totalSummaryMonth.ListNumber };
                if (totalSummaryMonthPrevious.ListNumber > 0)
                {
                    var percentageDifferenceList = ((decimal)totalSummaryMonth.ListNumber - (decimal)totalSummaryMonthPrevious.ListNumber) / (decimal)totalSummaryMonthPrevious.ListNumber * 100;
                    lists = new OverviewMetricData() { TotalMetric = totalSummaryMonth.ListNumber, TotalPreviousPeriodPercentage = percentageDifferenceList };
                }

                var lastDayWithData = _analyticsContext.SummaryDay.OrderByDescending(x => x.Date).FirstOrDefault(x => x.ProducerId == producerId && x.Date.Month == endDate.Month && x.Date.Year == endDate.Year);
                var lastDayWithDataPrevious = _analyticsContext.SummaryDay.OrderByDescending(x => x.Date).FirstOrDefault(x => x.ProducerId == producerId && x.Date.Month == endDatePrevious.Month && x.Date.Year == endDatePrevious.Year);
                DateTime followDateToCheck = endDate;
                DateTime followDateToCheckPrevious = endDatePrevious;
                if (lastDayWithData != null)
                {
                    followDateToCheck = lastDayWithData.Date;
                    
                }
                if (lastDayWithDataPrevious != null)
                {
                    followDateToCheckPrevious = lastDayWithDataPrevious.Date;
                }

                var summaryDayListFollow = _analyticsContext.SummaryDay.Where(x => x.ProducerId == producerId && x.Date.Date == followDateToCheck.Date && (provinceId == -1 || x.ProvinceId == provinceId))
              .AsNoTracking().ToList();
                var summaryDayPreviousListFollow = _analyticsContext.SummaryDay.Where(x => x.ProducerId == producerId && x.Date.Date == followDateToCheckPrevious.Date && (provinceId == -1 || x.ProvinceId == provinceId))
                    .AsNoTracking().ToList();

                totalSummaryMonth = new SummaryMonth();
                foreach (var summaryMonth in summaryDayListFollow)
                {
                    totalSummaryMonth.FollowsNumber = totalSummaryMonth.FollowsNumber.GetValueOrDefault(0) + summaryMonth.FollowsNumber.GetValueOrDefault(0);
                }

                totalSummaryMonthPrevious = new SummaryMonth();
                foreach (var summaryMonth in summaryDayPreviousListFollow)
                {
                    totalSummaryMonthPrevious.FollowsNumber = totalSummaryMonthPrevious.FollowsNumber.GetValueOrDefault(0) + summaryMonth.FollowsNumber.GetValueOrDefault(0);
                }


                follows = new OverviewMetricData() { TotalMetric = totalSummaryMonth.FollowsNumber.GetValueOrDefault(0) };
                if (totalSummaryMonthPrevious.FollowsNumber > 0)
                {
                    var percentageDifferenceFollow = ((decimal)totalSummaryMonth.FollowsNumber - (decimal)totalSummaryMonthPrevious.FollowsNumber) / (decimal)totalSummaryMonthPrevious.FollowsNumber * 100;
                    follows = new OverviewMetricData() { TotalMetric = totalSummaryMonth.FollowsNumber.GetValueOrDefault(0), TotalPreviousPeriodPercentage = percentageDifferenceFollow };
                }
            }
            else
            {
                var summaryYearList = _analyticsContext.SummaryYear.Where(x => x.ProducerId == producerId && x.Year == DateTime.Now.Year && (provinceId == -1 || x.ProvinceId == provinceId))
              .AsNoTracking().ToList();

                var totalSummaryYear = new SummaryYear();
                foreach (var summaryMonth in summaryYearList)
                {
                    totalSummaryYear.ImpressionNumber = totalSummaryYear.ImpressionNumber + summaryMonth.ImpressionNumber;
                    totalSummaryYear.ViewsNumber = totalSummaryYear.ViewsNumber + summaryMonth.ViewsNumber;
                    totalSummaryYear.ListNumber = totalSummaryYear.ListNumber + summaryMonth.ListNumber;
                    totalSummaryYear.FollowsNumber = totalSummaryYear.FollowsNumber.GetValueOrDefault(0) + summaryMonth.FollowsNumber.GetValueOrDefault(0);
                }
                impressions = new OverviewMetricData() { TotalMetric = totalSummaryYear.ImpressionNumber };
                views = new OverviewMetricData() { TotalMetric = totalSummaryYear.ViewsNumber };
                lists = new OverviewMetricData() { TotalMetric = totalSummaryYear.ListNumber };
                follows = new OverviewMetricData() { TotalMetric = totalSummaryYear.FollowsNumber.GetValueOrDefault(0) };
            }
           

            return new OverviewMetricsInfo() { Impressions = impressions, Lists = lists, Views = views, Follows = follows };
        }

        public MultiAxisGraphicData GetProducerMetrics(int producerId, int provinceId, int month, int year, MetricTypeEnum metricTypeEnum)
        {
            var pstTime = PSTTime.GetPacificStandardTime();
            var hours = PSTTime.GetHours();

            var startDate = new DateTime(pstTime.Year, pstTime.Month, 1).AddYears(-1);
            var endDate = new DateTime(pstTime.Year, pstTime.Month, 1).AddMonths(1).AddMilliseconds(-1);

            if (year != 0 && month != 0)
            {
                startDate = new DateTime(year, month, 1);
                endDate = startDate.AddMonths(1).AddMilliseconds(-1);
            }

            var startDatePrevious = new DateTime(pstTime.Year, pstTime.Month, 1).AddYears(-2);
            var endDatePrevious = new DateTime(pstTime.Year - 1, pstTime.Month, 1).AddMonths(1).AddMilliseconds(-1);

            if (year != 0 && month != 0)
            {
                startDatePrevious = new DateTime(year, month, 1).AddMonths(-1);
                endDatePrevious = startDatePrevious.AddMonths(1).AddMilliseconds(-1);
            }

            var graphicsCurrent = new List<GraphicData>();
            var graphicsPrevious = new List<GraphicData>();
            if (year != 0 && month != 0)
            {
                List<SummaryDay> summaryDayList = null;
                List<SummaryDay> summaryDayPreviousList = null;

                
                    summaryDayList = _analyticsContext.SummaryDay.Where(x => x.ProducerId == producerId && x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date && (provinceId == -1 || x.ProvinceId == provinceId))
                .AsNoTracking().ToList();
                    summaryDayPreviousList = _analyticsContext.SummaryDay.Where(x => x.ProducerId == producerId && x.Date.Date >= startDatePrevious.Date && x.Date.Date <= endDatePrevious.Date && (provinceId == -1 || x.ProvinceId == provinceId))
                        .AsNoTracking().ToList();

                for (var d = startDate; d <= endDate; d = d.AddDays(1))
                {
                    if (d <= pstTime)
                    {
                        var dayMetrics = summaryDayList.Where(t => t.Date >= d
                            && t.Date <= d.AddDays(1).AddMilliseconds(-1)).ToList();

                        decimal totalDayMetric = 0;
                        switch (metricTypeEnum)
                        {
                            case MetricTypeEnum.Impressions:
                                    totalDayMetric = dayMetrics.Sum(x => x.ImpressionNumber);
                                break;
                            case MetricTypeEnum.Views:
                                totalDayMetric = dayMetrics.Sum(x => x.ViewsNumber);
                                break;
                            case MetricTypeEnum.Lists:
                                totalDayMetric = dayMetrics.Sum(x => x.ListNumber);
                                break;
                            case MetricTypeEnum.Follows:
                                totalDayMetric = dayMetrics.Sum(x => x.FollowsNumber.GetValueOrDefault(0));
                                break;
                        }
                        graphicsCurrent.Add(new GraphicData
                        {
                            Label = d.ToString("dd"),
                            Value = totalDayMetric,
                            Date = d
                        });
                    }
                    else { break; }
                }

                for (var d = startDatePrevious; d <= endDatePrevious; d = d.AddDays(1))
                {
                    if (d <= pstTime)
                    {
                        var dayMetrics = summaryDayPreviousList.Where(t => t.Date.AddHours(hours) >= d
                            && t.Date.AddHours(hours) <= d.AddDays(1).AddMilliseconds(-1)).ToList();

                        decimal totalDayMetric = 0;
                        switch (metricTypeEnum)
                        {
                            case MetricTypeEnum.Impressions:
                                totalDayMetric = dayMetrics.Sum(x => x.ImpressionNumber);
                                break;
                            case MetricTypeEnum.Views:
                                totalDayMetric = dayMetrics.Sum(x => x.ViewsNumber);
                                break;
                            case MetricTypeEnum.Lists:
                                totalDayMetric = dayMetrics.Sum(x => x.ListNumber);
                                break;
                            case MetricTypeEnum.Follows:
                                totalDayMetric = dayMetrics.Sum(x => x.FollowsNumber.GetValueOrDefault(0));
                                break;
                        }
                        graphicsPrevious.Add(new GraphicData
                        {
                            Label = d.ToString("dd"),
                            Value = totalDayMetric,
                            Date = d
                        });
                    }
                    else { break; }
                }

            }
            else
            {

                for (var d = startDate; d <= endDate; d = d.AddMonths(1))
                {
                    if (d <= pstTime)
                    {
                        var summaryMonthList = _analyticsContext.SummaryMonth.Where(x => x.ProducerId == producerId && x.Month == d.Month && x.Year == d.Year
                        && (provinceId == -1 || x.ProvinceId == provinceId)).AsNoTracking().ToList();

                        decimal totalMonthMetric = 0;
                        switch (metricTypeEnum)
                        {
                            case MetricTypeEnum.Impressions:
                                totalMonthMetric = summaryMonthList.Sum(x => x.ImpressionNumber);
                                break;
                            case MetricTypeEnum.Views:
                                totalMonthMetric = summaryMonthList.Sum(x => x.ViewsNumber);
                                break;
                            case MetricTypeEnum.Lists:
                                totalMonthMetric = summaryMonthList.Sum(x => x.ListNumber);
                                break;
                            case MetricTypeEnum.Follows:
                                totalMonthMetric = summaryMonthList.Sum(x => x.FollowsNumber.GetValueOrDefault(0));
                                break;
                        }
                        graphicsCurrent.Add(new GraphicData
                        {
                            Label = d.ToString("MMM"),
                            Value = totalMonthMetric,
                            Date = d
                        });
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return new MultiAxisGraphicData() { CurrentPeriod = graphicsCurrent, PreviousPeriod = graphicsPrevious };
        }

        public List<GraphicData> GetProductAvailableMonths(int weedId, int provinceId)
        {
            var currentYear = DateTime.Now.Year;
            var availableMonths = _analyticsContext.SummaryProductMonth.Where(x => x.WeedId == weedId && (provinceId == -1 || x.ProvinceId == provinceId) && x.Year == currentYear)
                .Select(x => x.Month).Distinct().OrderByDescending(x => x).ToList();
            var graphics = new List<GraphicData>();
            foreach (var month in availableMonths)
            {
                var date = new DateTime(currentYear, month, 1);
                graphics.Add(new GraphicData
                {
                    Label = date.ToString("MMM yyyy"),
                    Attribute = date.Month + "/" + date.Year,
                    Date = date
                });
            }
            return graphics;
        }

        public MultiAxisGraphicData GetProductMetrics(int weedId, int provinceId, int month, int year, MetricTypeEnum metricTypeEnum)
        {
            var pstTime = PSTTime.GetPacificStandardTime();
            var hours = PSTTime.GetHours();

            var startDate = new DateTime(pstTime.Year, pstTime.Month, 1).AddYears(-1);
            var endDate = new DateTime(pstTime.Year, pstTime.Month, 1).AddMonths(1).AddMilliseconds(-1);

            if (year != 0 && month != 0)
            {
                startDate = new DateTime(year, month, 1);
                endDate = startDate.AddMonths(1).AddMilliseconds(-1);
            }

            var startDatePrevious = new DateTime(pstTime.Year, pstTime.Month, 1).AddYears(-2);
            var endDatePrevioud = new DateTime(pstTime.Year - 1, pstTime.Month, 1).AddMonths(1).AddMilliseconds(-1);

            if (year != 0 && month != 0)
            {
                startDatePrevious = new DateTime(year, month, 1).AddMonths(-1);
                endDatePrevioud = startDatePrevious.AddMonths(1).AddMilliseconds(-1);
            }

            var graphicsCurrent = new List<GraphicData>();
            var graphicsPrevious = new List<GraphicData>();
            if (year != 0 && month != 0)
            {
                var summaryDayList = _analyticsContext.SummaryProductDay.Where(x => x.WeedId == weedId && x.Date >= startDate && x.Date <= endDate && (provinceId == -1 || x.ProvinceId == provinceId))
               .AsNoTracking().ToList();
                var summaryDayPreviousList = _analyticsContext.SummaryProductDay.Where(x => x.WeedId == weedId && x.Date >= startDatePrevious && x.Date <= endDatePrevioud && (provinceId == -1 || x.ProvinceId == provinceId))
                    .AsNoTracking().ToList();

                for (var d = startDate; d <= endDate; d = d.AddDays(1))
                {
                    if (d <= pstTime)
                    {
                        var dayMetrics = summaryDayList.Where(t => t.Date >= d
                            && t.Date <= d.AddDays(1).AddMilliseconds(-1)).ToList();

                        decimal totalDayMetric = 0;
                        switch (metricTypeEnum)
                        {
                            case MetricTypeEnum.Impressions:
                                totalDayMetric = dayMetrics.Sum(x => x.ImpressionNumber);
                                break;
                            case MetricTypeEnum.Views:
                                totalDayMetric = dayMetrics.Sum(x => x.ViewsNumber);
                                break;
                            case MetricTypeEnum.Lists:
                                totalDayMetric = dayMetrics.Sum(x => x.ListNumber);
                                break;
                            case MetricTypeEnum.Follows:
                                totalDayMetric = dayMetrics.Sum(x => x.FollowsNumber.GetValueOrDefault(0));
                                break;
                        }
                        graphicsCurrent.Add(new GraphicData
                        {
                            Label = d.ToString("dd"),
                            Value = totalDayMetric,
                            Date = d
                        });
                    }
                    else { break; }
                }

                for (var d = startDatePrevious; d <= endDatePrevioud; d = d.AddDays(1))
                {
                    if (d <= pstTime)
                    {
                        var dayMetrics = summaryDayPreviousList.Where(t => t.Date.AddHours(hours) >= d
                            && t.Date.AddHours(hours) <= d.AddDays(1).AddMilliseconds(-1)).ToList();

                        decimal totalDayMetric = 0;
                        switch (metricTypeEnum)
                        {
                            case MetricTypeEnum.Impressions:
                                totalDayMetric = dayMetrics.Sum(x => x.ImpressionNumber);
                                break;
                            case MetricTypeEnum.Views:
                                totalDayMetric = dayMetrics.Sum(x => x.ViewsNumber);
                                break;
                            case MetricTypeEnum.Lists:
                                totalDayMetric = dayMetrics.Sum(x => x.ListNumber);
                                break;
                            case MetricTypeEnum.Follows:
                                totalDayMetric = dayMetrics.Sum(x => x.FollowsNumber.GetValueOrDefault(0));
                                break;
                        }
                        graphicsPrevious.Add(new GraphicData
                        {
                            Label = d.ToString("dd"),
                            Value = totalDayMetric,
                            Date = d
                        });
                    }
                    else { break; }
                }

            }
            else
            {

                for (var d = startDate; d <= endDate; d = d.AddMonths(1))
                {
                    if (d <= pstTime)
                    {
                        var summaryMonthList = _analyticsContext.SummaryProductMonth.Where(x => x.WeedId == weedId && x.Month == d.Month && x.Year == d.Year
                        && (provinceId == -1 || x.ProvinceId == provinceId)).AsNoTracking().ToList();

                        decimal totalMonthMetric = 0;
                        switch (metricTypeEnum)
                        {
                            case MetricTypeEnum.Impressions:
                                totalMonthMetric = summaryMonthList.Sum(x => x.ImpressionNumber);
                                break;
                            case MetricTypeEnum.Views:
                                totalMonthMetric = summaryMonthList.Sum(x => x.ViewsNumber);
                                break;
                            case MetricTypeEnum.Lists:
                                totalMonthMetric = summaryMonthList.Sum(x => x.ListNumber);
                                break;
                            case MetricTypeEnum.Follows:
                                totalMonthMetric = summaryMonthList.Sum(x => x.FollowsNumber.GetValueOrDefault(0));
                                break;
                        }
                        graphicsCurrent.Add(new GraphicData
                        {
                            Label = d.ToString("MMM"),
                            Value = totalMonthMetric,
                            Date = d
                        });
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return new MultiAxisGraphicData() { CurrentPeriod = graphicsCurrent, PreviousPeriod = graphicsPrevious };
        }

        public List<ProductViewsReport> GetProductsViewsReport(ProductsMetricsQuery productsMetricsQuery)
        {
            List<ProductViewsReport> productMetricsReport = new List<ProductViewsReport>();
            ConcurrentBag<ProductViewsReport> cb = new ConcurrentBag<ProductViewsReport>();
            var weedIds = productsMetricsQuery.ProductList.Select(x => x.WeedId).ToList();

            var viewMetric = (from v in _analyticsContext.View
                              where v.MeasuredDate.Date >= productsMetricsQuery.StartDate.Date && v.MeasuredDate.Date <= productsMetricsQuery.EndDate.Date
                              && weedIds.Contains(v.WeedId)
                              group v by new { v.WeedId,v.MeasuredDate.Date, v.IsBasicProduct } into g
                              select new
                              {
                                  g.Key.WeedId,
                                  Views = g.Sum(x => x.MetricCount),
                                  WeightedViews = g.Sum(x => x.WeightedCount),
                                  IsBasicProduct = g.Key.IsBasicProduct,
                                  MeasuredDate = g.Key.Date
                              }).AsNoTracking().ToList();

            Parallel.ForEach(productsMetricsQuery.ProductList, (weed) =>
            {
                
                var view = viewMetric.Where(x => x.WeedId == weed.WeedId)
                           .Select(x=>new Metric() { Count = x.Views, MeasuredDate = x.MeasuredDate, WeightedCount = x.WeightedViews }).ToList();
                
                var productMetric = new ProductViewsReport() { WeedId = weed.WeedId, ProductName = weed.Name };
                if (view != null && view.Count > 0)
                {
                    productMetric.Views =  view;
                    var isBasicProduct = viewMetric.Where(x => x.WeedId == weed.WeedId).Select(x => x.IsBasicProduct).FirstOrDefault();
                    productMetric.IsBasicProduct = isBasicProduct;
                    cb.Add(productMetric);
                }
            });
            productMetricsReport = cb.ToList();
            return productMetricsReport;
        }
    }
}
