using AnalyticsDAL.Helpers;
using AnalyticsDAL.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnalyticsDAL.Repositories
{
    public class SummaryRepository : ISummaryRepository
    {
        private readonly AnalyticsContext _analyticsContext;

        public SummaryRepository(AnalyticsContext analyticsContext)
        {
            _analyticsContext = analyticsContext;
        }

        private SummaryDay CreateDay(int producerId, List<Impression> impressions, List<View> views, List<List> list, List<Follow> follows, int provId, int day, int month, int year)
        {
            var startDate = new DateTime(year, month, day);
            var existingDayProducer = _analyticsContext.SummaryDay.FirstOrDefault(d => d.ProducerId == producerId && d.Date == startDate && d.ProvinceId == provId);

            var totalImpressions = 0;
            var totalViews = 0;
            var totalList = 0;
            var totalFollows = 0;

            impressions.ForEach(d =>
            {
                totalImpressions += d.WeightedCount;
            });
            views.ForEach(d =>
            {
                totalViews += d.WeightedCount;
            });
            list.ForEach(d =>
            {
                totalList += d.WeightedCount;
            });
            follows.ForEach(d =>
            {
                totalFollows += d.WeightedCount;
            });

            if (existingDayProducer == null)
            {
                var dayEntity = new SummaryDay
                {
                    ProducerId = producerId,
                    ImpressionNumber = totalImpressions,
                    ViewsNumber = totalViews,
                    ListNumber = totalList,
                    Date = startDate,
                    ProvinceId = provId,
                    FollowsNumber = totalFollows
                };

                return dayEntity;
            }
            else
            {
                existingDayProducer.ImpressionNumber = totalImpressions;
                existingDayProducer.ViewsNumber = totalViews;
                existingDayProducer.ListNumber = totalList;
                existingDayProducer.FollowsNumber = totalFollows;
            };
            return existingDayProducer;
        }

        private List<SummaryDay> SaveDayForProducerAllProvinces(int producerId, List<Province> provinces, int day, int month, int year)
        {
            var summaryDayByProvs = new List<SummaryDay>();
            var hours = PSTTime.GetHours();

            var startDate = new DateTime(year, month, day);
            var endDate = new DateTime(year, month, day).AddDays(1).AddMilliseconds(-1);

            var impressions = _analyticsContext.Impression.Where(d => d.ProducerId == producerId && d.MeasuredDate.AddHours(hours) >= startDate && d.MeasuredDate.AddHours(hours) < endDate).AsNoTracking().ToList();
            var views = _analyticsContext.View.Where(d => d.ProducerId == producerId && d.MeasuredDate.AddHours(hours) >= startDate && d.MeasuredDate.AddHours(hours) < endDate).AsNoTracking().ToList();
            var list = _analyticsContext.List.Where(d => d.ProducerId == producerId && d.MeasuredDate.AddHours(hours) >= startDate && d.MeasuredDate.AddHours(hours) < endDate).AsNoTracking().ToList();
            var follows = _analyticsContext.Follow.Where(d => d.ProducerId == producerId && d.MeasuredDate.AddHours(hours) >= startDate && d.MeasuredDate.AddHours(hours) < endDate).AsNoTracking().ToList();

            foreach (var prov in provinces)
            {
                var impProv = impressions.Where(d => d.ProvinceId == prov.Id).ToList();
                var viewProv = views.Where(d => d.ProvinceId == prov.Id).ToList();
                var listProv = list.Where(d => d.ProvinceId == prov.Id).ToList();
                var followProv = follows.Where(d => d.ProvinceId == prov.Id).ToList();

                var summaryDay = CreateDay(producerId, impProv, viewProv, listProv, followProv, prov.Id, day, month, year);
                if (summaryDay.ViewsNumber != 0 || summaryDay.ImpressionNumber != 0 || summaryDay.ListNumber != 0 || summaryDay.FollowsNumber != 0)
                    summaryDayByProvs.Add(summaryDay);
            }
            return summaryDayByProvs;
        }

        public int SaveDay(List<int> producerIds, int day, int month, int year)
        {
            var mainDataList = new List<SummaryDay>();

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

        private List<SummaryMonth> SaveMonthForProducerAllProvinces(int producerId, List<Province> provinces, int month, int year)
        {
            var summaryByProvs = new List<SummaryMonth>();
            foreach (var prov in provinces)
            {
                var summary = CreateMonth(producerId, prov.Id, month, year);
                if (summary.ViewsNumber != 0 || summary.ImpressionNumber != 0 || summary.ListNumber != 0 || summary.FollowsNumber != 0)
                    summaryByProvs.Add(summary);
            }
            return summaryByProvs;
        }

        private SummaryMonth CreateMonth(int producerId, int provinceId, int month, int year)
        {
            var days = _analyticsContext.SummaryDay.Where(d => d.ProducerId == producerId && d.Date.Month == month && d.Date.Year == year
                && d.ProvinceId == provinceId).AsNoTracking().ToList();

            var daysFollows = _analyticsContext.SummaryDay.Where(d => d.ProducerId == producerId && d.Date.Month == month && d.Date.Year == year
                && d.ProvinceId == provinceId).AsNoTracking().OrderByDescending(x=> x.Date).FirstOrDefault();

            var existingMonthProducer = _analyticsContext.SummaryMonth.FirstOrDefault(d => d.ProducerId == producerId && d.Month == month
                && d.Year == year && d.ProvinceId == provinceId);

            var totalImpressions = 0;
            var totalViews = 0;
            var totalList = 0;
            var totalFollows = 0;

            days.ForEach(d =>
            {
                totalImpressions += d.ImpressionNumber;
                totalViews += d.ViewsNumber;
                totalList += d.ListNumber;
               
            });

            if(daysFollows != null)
                totalFollows = daysFollows.FollowsNumber.GetValueOrDefault(0);

            if (existingMonthProducer == null)
            {
                var monthEntity = new SummaryMonth
                {
                    ProducerId = producerId,
                    ImpressionNumber = totalImpressions,
                    ViewsNumber = totalViews,
                    ListNumber = totalList,
                    Month = month,
                    Year = year,
                    ProvinceId = provinceId,
                    FollowsNumber = totalFollows
                };

                return monthEntity;
            }
            else
            {
                existingMonthProducer.ImpressionNumber = totalImpressions;
                existingMonthProducer.ViewsNumber = totalViews;
                existingMonthProducer.ListNumber = totalList;
                existingMonthProducer.FollowsNumber = totalFollows;

                return existingMonthProducer;
            }
        }

        public int SaveMonth(List<int> producerIds, int month, int year)
        {
            var mainDataList = new List<SummaryMonth>();
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

        public List<SummaryYear> SaveYear(int producerId, List<Province> provinces, int year)
        {
            var summaryByProvs = new List<SummaryYear>();
            foreach (var prov in provinces)
            {
                var summary = CreateYear(producerId, prov.Id, year);
                if (summary.ViewsNumber != 0 || summary.ImpressionNumber != 0 || summary.ListNumber != 0 || summary.FollowsNumber != 0)
                    summaryByProvs.Add(summary);
            }
            return summaryByProvs;
        }

        public SummaryYear CreateYear(int producerId, int provinceId, int year)
        {
            var months = _analyticsContext.SummaryMonth.Where(d => d.Year == year && d.ProducerId == producerId && d.ProvinceId == provinceId).AsNoTracking().AsNoTracking().ToList();

            var existingYearProducer = _analyticsContext.SummaryYear.FirstOrDefault(d => d.ProducerId == producerId && d.Year == year && d.ProvinceId == provinceId);

            var totalImpressions = 0;
            var totalViews = 0;
            var totalList = 0;
            var totalFollows = 0;

            months.ForEach(d =>
            {
                totalImpressions += d.ImpressionNumber;
                totalViews += d.ViewsNumber;
                totalList += d.ListNumber;
                totalFollows += d.FollowsNumber.GetValueOrDefault(0);
            });

            if (existingYearProducer == null)
            {
                var yearEntity = new SummaryYear
                {
                    ProducerId = producerId,
                    ImpressionNumber = totalImpressions,
                    ViewsNumber = totalViews,
                    ListNumber = totalList,
                    Year = year,
                    ProvinceId = provinceId,
                    FollowsNumber = totalFollows
                };

                return yearEntity;
            }
            else
            {
                existingYearProducer.ImpressionNumber = totalImpressions;
                existingYearProducer.ViewsNumber = totalViews;
                existingYearProducer.ListNumber = totalList;
                existingYearProducer.FollowsNumber = totalFollows;

                return existingYearProducer;
            }
        }

        public int SaveYear(List<int> producerIds, int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year + 1, 1, 1).AddMilliseconds(-1);

            var mainDataList = new List<SummaryYear>();
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
    }
}
