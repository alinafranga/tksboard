using AnalyticsDAL.Helpers;
using AnalyticsDAL.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnalyticsDAL.Repositories
{
    public class SummaryProductRepository : ISummaryProductRepository
    {
        private readonly AnalyticsContext _analyticsContext;

        public SummaryProductRepository(AnalyticsContext analyticsContext)
        {
            _analyticsContext = analyticsContext;
        }

        public int SaveDay(List<int> producerIds, int day, int month, int year)
        {
            var hours = PSTTime.GetHours();

            var mainDataList = new List<SummaryProductDay>();

            var provinces = _analyticsContext.Province.AsNoTracking().ToList();

            foreach (var producerId in producerIds)
            {
                var listDay = SaveDayForProducerAllProvinces(producerId, provinces, day, month, year);
                if (listDay != null && listDay.Count > 0)
                    mainDataList.AddRange(listDay);
            }

            _analyticsContext.BulkInsertOrUpdate(mainDataList);
            return 99999;
        }

        private List<SummaryProductDay> SaveDayForProducerAllProvinces(int producerId, List<Province> provinces, int day, int month, int year)
        {
            var summaryDayByProvs = new List<SummaryProductDay>();
            var hours = PSTTime.GetHours();

            var startDate = new DateTime(year, month, day);
            var endDate = new DateTime(year, month, day).AddDays(1).AddMilliseconds(-1);

            var impressions = _analyticsContext.Impression.Where(d => d.ProducerId == producerId && d.MeasuredDate.AddHours(hours) >= startDate && d.MeasuredDate.AddHours(hours) < endDate).AsNoTracking().ToList();
            var views = _analyticsContext.View.Where(d => d.ProducerId == producerId && d.MeasuredDate.AddHours(hours) >= startDate && d.MeasuredDate.AddHours(hours) < endDate).AsNoTracking().ToList();
            var list = _analyticsContext.List.Where(d => d.ProducerId == producerId && d.MeasuredDate.AddHours(hours) >= startDate && d.MeasuredDate.AddHours(hours) < endDate).AsNoTracking().ToList();
            var follows = _analyticsContext.Follow.Where(d => d.ProducerId == producerId && d.MeasuredDate.AddHours(hours) >= startDate && d.MeasuredDate.AddHours(hours) < endDate).AsNoTracking().ToList();

            var weedAdvancedIds = impressions.Where(d => !d.IsBasicProduct).Select(d => d.WeedId).Distinct().ToList();
            weedAdvancedIds.AddRange(views.Where(d => !d.IsBasicProduct).Select(d => d.WeedId).ToList());
            weedAdvancedIds.AddRange(list.Where(d => !d.IsBasicProduct).Select(d => d.WeedId).ToList());
            weedAdvancedIds.AddRange(follows.Where(d => !d.IsBasicProduct).Select(d => d.WeedId).ToList());

            var weedBasicIds = impressions.Where(d => d.IsBasicProduct).Select(d => d.WeedId).Distinct().ToList();
            weedBasicIds.AddRange(views.Where(d => d.IsBasicProduct).Select(d => d.WeedId).ToList());
            weedBasicIds.AddRange(list.Where(d => d.IsBasicProduct).Select(d => d.WeedId).ToList());
            weedBasicIds.AddRange(follows.Where(d => d.IsBasicProduct).Select(d => d.WeedId).ToList());

            var weedIds = weedAdvancedIds.Distinct().ToList();
            var summaryAdvanced = GenerateForWeeds(producerId, provinces, weedIds, impressions, views, list, follows, day, month, year, false);

            var weedIBasicds = weedBasicIds.Distinct().ToList();
            var summaryBasic = GenerateForWeeds(producerId, provinces, weedIBasicds, impressions, views, list, follows, day, month, year, true);

            summaryDayByProvs.AddRange(summaryAdvanced);
            summaryDayByProvs.AddRange(summaryBasic);

            return summaryDayByProvs;
        }

        private List<SummaryProductDay> GenerateForWeeds(int producerId, List<Province> provinces, List<int> weedIds, List<Impression> impressions,
            List<View> views, List<List> list, List<Follow> follows, int day, int month, int year, bool isBasic)
        {
            var summaryDayByProvs = new List<SummaryProductDay>();
            if (weedIds.Count > 0)
            {
                foreach (var prov in provinces)
                {
                    var impProv = impressions.Where(d => d.ProvinceId == prov.Id && d.IsBasicProduct == isBasic).ToList();
                    var viewProv = views.Where(d => d.ProvinceId == prov.Id && d.IsBasicProduct == isBasic).ToList();
                    var listProv = list.Where(d => d.ProvinceId == prov.Id && d.IsBasicProduct == isBasic).ToList();
                    var followProv = follows.Where(d => d.ProvinceId == prov.Id && d.IsBasicProduct == isBasic).ToList();

                    var summaryDayList = CreateDay(producerId, weedIds, impProv, viewProv, listProv, followProv, prov.Id, day, month, year, isBasic);
                    foreach (var summaryDay in summaryDayList)
                    {
                        if (summaryDay.ViewsNumber != 0 || summaryDay.ImpressionNumber != 0 || summaryDay.ListNumber != 0 || summaryDay.FollowsNumber != 0)
                            summaryDayByProvs.Add(summaryDay);
                    }
                }
            }
            return summaryDayByProvs;
        }

        private List<SummaryProductDay> CreateDay(int producerId, List<int> weedIds, List<Impression> impressions, List<View> views, List<List> list, List<Follow> follows,
            int provId, int day, int month, int year, bool isBasic)
        {
            List<SummaryProductDay> returnList = new List<SummaryProductDay>();
            foreach (var weedId in weedIds)
            {
                var startDate = new DateTime(year, month, day);
                var existingDayProducer = _analyticsContext.SummaryProductDay
                    .FirstOrDefault(d => d.ProducerId == producerId && d.Date == startDate && d.ProvinceId == provId && d.WeedId == weedId
                    && d.IsBasicProduct == isBasic);

                var impProv = impressions.Where(d => d.WeedId == weedId).ToList();
                var viewProv = views.Where(d => d.WeedId == weedId).ToList();
                var listProv = list.Where(d => d.WeedId == weedId).ToList();
                var followProv = follows.Where(d => d.WeedId == weedId).ToList();

                var totalImpressions = 0;
                var totalViews = 0;
                var totalList = 0;
                var totalFollows = 0;

                impProv.ForEach(d =>
                {
                    totalImpressions += d.WeightedCount;
                });
                viewProv.ForEach(d =>
                {
                    totalViews += d.WeightedCount;
                });
                listProv.ForEach(d =>
                {
                    totalList += d.WeightedCount;
                });
                followProv.ForEach(d =>
                {
                    totalFollows += d.WeightedCount;
                });

                if (existingDayProducer == null)
                {
                    var dayEntity = new SummaryProductDay
                    {
                        ProducerId = producerId,
                        ImpressionNumber = totalImpressions,
                        ViewsNumber = totalViews,
                        ListNumber = totalList,
                        Date = startDate,
                        ProvinceId = provId,
                        FollowsNumber = totalFollows,
                        WeedId = weedId,
                        IsBasicProduct = isBasic
                    };

                    returnList.Add(dayEntity);
                }
                else
                {
                    existingDayProducer.ImpressionNumber = totalImpressions;
                    existingDayProducer.ViewsNumber = totalViews;
                    existingDayProducer.ListNumber = totalList;
                    existingDayProducer.FollowsNumber = totalFollows;

                    returnList.Add(existingDayProducer);
                }
            }
            return returnList;
        }

        public int SaveMonth(List<int> producerIds, int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month + 1, 1).AddMilliseconds(-1);

            var mainDataList = new List<SummaryProductMonth>();
            var provinces = _analyticsContext.Province.AsNoTracking().ToList();

            foreach (var producerId in producerIds)
            {
                var monthSummary = SaveMonthForProducerAllProvinces(producerId, provinces, month, year);
                if (monthSummary != null && monthSummary.Count > 0)
                    mainDataList.AddRange(monthSummary);
            }
            _analyticsContext.BulkInsertOrUpdate(mainDataList);
            return 9999;
        }

        private List<SummaryProductMonth> SaveMonthForProducerAllProvinces(int producerId, List<Province> provinces, int month, int year)
        {
            var summaryByProvs = new List<SummaryProductMonth>();
            foreach (var prov in provinces)
            {
                var summaryList = CreateMonth(producerId, prov.Id, month, year);
                foreach (var summary in summaryList)
                {
                    if (summary.ViewsNumber != 0 || summary.ImpressionNumber != 0 || summary.ListNumber != 0 || summary.FollowsNumber != 0)
                        summaryByProvs.Add(summary);
                }
            }
            return summaryByProvs;
        }

        private List<SummaryProductMonth> CreateMonth(int producerId, int provinceId, int month, int year)
        {
            var days = _analyticsContext.SummaryProductDay.Where(d => d.ProducerId == producerId && d.Date.Month == month && d.Date.Year == year
                && d.ProvinceId == provinceId).AsNoTracking().ToList();

            var weedAdvancedIds = days.Where(d => !d.IsBasicProduct.GetValueOrDefault(false)).Select(d => d.WeedId).Distinct().ToList();

            var weedBasicIds = days.Where(d => d.IsBasicProduct.GetValueOrDefault(false)).Select(d => d.WeedId).Distinct().ToList();

            List<SummaryProductMonth> returnList = new List<SummaryProductMonth>();

            var monthsAdvanced = GenerateMonthsForWeeds(weedAdvancedIds, producerId, provinceId, days, month, year, false);
            returnList.AddRange(monthsAdvanced);

            var monthsBasic = GenerateMonthsForWeeds(weedBasicIds, producerId, provinceId, days, month, year, true);
            returnList.AddRange(monthsBasic);

            return returnList;
        }

        private List<SummaryProductMonth> GenerateMonthsForWeeds(List<int> weedIds, int producerId, int provinceId, List<SummaryProductDay> days, int month, int year, bool isBasic)
        {
            List<SummaryProductMonth> returnList = new List<SummaryProductMonth>();
            foreach (var weedId in weedIds)
            {
                var existingMonthProducer = _analyticsContext.SummaryProductMonth.FirstOrDefault(d => d.ProducerId == producerId && d.Month == month
                    && d.Year == year && d.ProvinceId == provinceId && d.WeedId == weedId && d.IsBasicProduct == isBasic);

                var totalImpressions = 0;
                var totalViews = 0;
                var totalList = 0;
                var totalFollows = 0;

                var daysWeed = days.Where(d => d.ProducerId == producerId && d.Date.Month == month && d.Date.Year == year
                    && d.ProvinceId == provinceId && d.WeedId == weedId && d.IsBasicProduct == isBasic).ToList();

                var daysFollows = days.Where(d => d.ProducerId == producerId && d.Date.Month == month && d.Date.Year == year
                                && d.ProvinceId == provinceId && d.WeedId == weedId && d.IsBasicProduct == isBasic).OrderByDescending(x => x.Date).FirstOrDefault();

                daysWeed.ForEach(d =>
                {
                    totalImpressions += d.ImpressionNumber;
                    totalViews += d.ViewsNumber;
                    totalList += d.ListNumber;
                });

                if (daysFollows != null)
                    totalFollows = daysFollows.FollowsNumber.GetValueOrDefault(0);

                if (existingMonthProducer == null)
                {
                    var monthEntity = new SummaryProductMonth
                    {
                        ProducerId = producerId,
                        ImpressionNumber = totalImpressions,
                        ViewsNumber = totalViews,
                        ListNumber = totalList,
                        Month = month,
                        Year = year,
                        ProvinceId = provinceId,
                        FollowsNumber = totalFollows,
                        WeedId = weedId,
                        IsBasicProduct = isBasic
                    };

                    returnList.Add(monthEntity);
                }
                else
                {
                    existingMonthProducer.ImpressionNumber = totalImpressions;
                    existingMonthProducer.ViewsNumber = totalViews;
                    existingMonthProducer.ListNumber = totalList;
                    existingMonthProducer.FollowsNumber = totalFollows;

                    returnList.Add(existingMonthProducer);
                }
            }

            return returnList;
        }

        public int SaveYear(List<int> producerIds, int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year + 1, 1, 1).AddMilliseconds(-1);

            var mainDataList = new List<SummaryProductYear>();
            var provinces = _analyticsContext.Province.AsNoTracking().ToList();

            foreach (var producerId in producerIds)
            {
                var yearSummary = SaveYear(producerId, provinces, year);
                if (yearSummary != null && yearSummary.Count > 0)
                    mainDataList.AddRange(yearSummary);
            }
            _analyticsContext.BulkInsertOrUpdate(mainDataList);
            return 9999;
        }

        private List<SummaryProductYear> SaveYear(int producerId, List<Province> provinces, int year)
        {
            var summaryByProvs = new List<SummaryProductYear>();
            foreach (var prov in provinces)
            {
                var summaryYear = CreateYear(producerId, prov.Id, year);
                foreach (var summary in summaryYear)
                {
                    if (summary.ViewsNumber != 0 || summary.ImpressionNumber != 0 || summary.ListNumber != 0 || summary.FollowsNumber != 0)
                        summaryByProvs.Add(summary);
                }
            }
            return summaryByProvs;
        }

        private List<SummaryProductYear> CreateYear(int producerId, int provinceId, int year)
        {
            var months = _analyticsContext.SummaryProductMonth.Where(d => d.Year == year && d.ProducerId == producerId && d.ProvinceId == provinceId).AsNoTracking().ToList();

            var weedAdvancedIds = months.Where(d => !d.IsBasicProduct.GetValueOrDefault(false)).Select(d => d.WeedId).Distinct().ToList();
            var weedBasicIds = months.Where(d => d.IsBasicProduct.GetValueOrDefault(false)).Select(d => d.WeedId).Distinct().ToList();

            List<SummaryProductYear> returnList = new List<SummaryProductYear>();

            var yearAdvanced = GenerateYearsForWeeds(weedAdvancedIds, producerId, provinceId, months, year, false);
            var yearBasic = GenerateYearsForWeeds(weedAdvancedIds, producerId, provinceId, months, year, true);

            returnList.AddRange(yearAdvanced);
            returnList.AddRange(yearBasic);

            return returnList;
        }

        private List<SummaryProductYear> GenerateYearsForWeeds(List<int> weedIds, int producerId, int provinceId, List<SummaryProductMonth> months, int year, bool isBasic)
        {
            List<SummaryProductYear> returnList = new List<SummaryProductYear>();
            foreach (var weedId in weedIds)
            {
                var existingYearProducer = _analyticsContext.SummaryProductYear.FirstOrDefault(d => d.ProducerId == producerId && d.Year == year
                    && d.ProvinceId == provinceId && d.WeedId == weedId && d.IsBasicProduct == isBasic);

                var totalImpressions = 0;
                var totalViews = 0;
                var totalList = 0;
                var totalFollows = 0;

                var monthsWeed = months.Where(d => d.WeedId == weedId).ToList();

                monthsWeed.ForEach(d =>
                {
                    totalImpressions += d.ImpressionNumber;
                    totalViews += d.ViewsNumber;
                    totalList += d.ListNumber;
                    totalFollows += d.FollowsNumber.GetValueOrDefault(0);
                });

                if (existingYearProducer == null)
                {
                    var yearEntity = new SummaryProductYear
                    {
                        ProducerId = producerId,
                        ImpressionNumber = totalImpressions,
                        ViewsNumber = totalViews,
                        ListNumber = totalList,
                        Year = year,
                        ProvinceId = provinceId,
                        FollowsNumber = totalFollows,
                        WeedId = weedId,
                        IsBasicProduct = isBasic
                    };

                    returnList.Add(yearEntity);
                }
                else
                {
                    existingYearProducer.ImpressionNumber = totalImpressions;
                    existingYearProducer.ViewsNumber = totalViews;
                    existingYearProducer.ListNumber = totalList;
                    existingYearProducer.FollowsNumber = totalFollows;

                    returnList.Add(existingYearProducer);
                }
            }

            return returnList;
        }
    }
}
