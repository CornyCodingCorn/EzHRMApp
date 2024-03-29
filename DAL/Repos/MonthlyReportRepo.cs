﻿using System;
using System.Collections.Generic;
using System.Text;
using DAL.Rows;
using SqlKata.Execution;

namespace DAL.Repos
{
    public class MonthlyReportRepo : Repo<MonthlyReport>
    {
        public static MonthlyReportRepo Instance { get; set; } = new MonthlyReportRepo();

        private MonthlyReportRepo()
        {
            TableName = "baocaonhansu";
            PKColsName = new string[]
            {
                "Thang",
                "Nam"
            };
        }

        public MonthlyReport FindByDate(DateTime date)
        {
            return FindByID(new object[] { date.Month, date.Year });
        }

        public MonthlyReport FindCurrentMonthlyReport()
        {
            return FindByID(new object[] { DateTime.Now.Month, DateTime.Now.Year });
        }

        public IEnumerable<MonthlyReport> GetAllInYear(int year)
        {
            try
            {
                var db = DatabaseConnector.Database;
                return db.Query(TableName).Where(PKColsName[1], "=", year).Get<MonthlyReport>();
            }
            catch { return null; }
        }

        public IEnumerable<MonthlyReport> GetBetween(DateTime start, DateTime end)
        {
            try
            {
                var db = DatabaseConnector.Database;
                if (start.Year < end.Year)
                    return db.Query(TableName).WhereBetween(PKColsName[1], start.Year, end.Year).Get<MonthlyReport>();
                if (start.Year == end.Year)
                    return db.Query(TableName).Where(PKColsName[1], start.Year)
                                            .WhereBetween(PKColsName[0], start.Month, end.Month).Get<MonthlyReport>();
                else
                    return null;
            }
            catch { return null; }
        }
    }
}
