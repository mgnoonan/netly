using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using netly;
using netly.Models;
using System.Data.Entity;

namespace netly.Helpers
{
    public class Repository : IRepository
    {
        private netlyEntities context = new netlyEntities();

        public IQueryable<UrlMap> FindUrlMaps()
        {
            var query = from g in context.UrlMaps
                        select g;

            return query;
        }

        public IQueryable<UrlMapDetailView> FindUrlMapDetails()
        {
            var query = from g in context.UrlMapDetailViews
                        select g;

            return query;
        }

        public IQueryable<UrlAggregate> FindUrlAggregates()
        {
            var query = from g in context.UrlAggregates
                        select g;

            return query;
        }

        public IQueryable<UrlHistory> FindUrlHistories()
        {
            var query = from g in context.UrlHistories
                        select g;

            return query;
        }

        public IQueryable<UrlHistoryDetailView> FindUrlHistoryDetails()
        {
            var query = from g in context.UrlHistoryDetailViews
                        select g;

            return query;
        }

        public IQueryable<Country> FindCountries()
        {
            var query = from g in context.Countries
                        select g;

            return query;
        }

        public IQueryable<ApiKey> FindApiKeys()
        {
            var query = from g in context.ApiKeys
                        select g;

            return query;
        }

        //public IQueryable<SQLDatesTable> FindSQLDates()
        //{
        //    var query = from g in context.SQLDatesTables
        //                select g;

        //    return query;
        //}

        public void DeleteObject(UrlMap obj)
        {
            context.UrlMaps.Remove(obj);
        }

        public void AddObject(UrlMap obj)
        {
            context.UrlMaps.Add(obj);
        }

        public void Attach(UrlMap obj)
        {
            context.UrlMaps.Attach(obj);
            context.Entry(obj).State = EntityState.Modified;
        }

        public void DeleteObject(UrlAggregate obj)
        {
            context.UrlAggregates.Remove(obj);
        }

        public void AddObject(UrlAggregate obj)
        {
            context.UrlAggregates.Add(obj);
        }

        public void Attach(UrlAggregate obj)
        {
            context.UrlAggregates.Attach(obj);
            context.Entry(obj).State = EntityState.Modified;
        }

        public void DeleteObject(UrlHistory obj)
        {
            context.UrlHistories.Remove(obj);
        }

        public void AddObject(UrlHistory obj)
        {
            context.UrlHistories.Add(obj);
        }

        public void Attach(UrlHistory obj)
        {
            context.UrlHistories.Attach(obj);
            context.Entry(obj).State = EntityState.Modified;
        }

        public void DeleteObject(Country obj)
        {
            context.Countries.Remove(obj);
        }

        public void AddObject(Country obj)
        {
            context.Countries.Add(obj);
        }

        public void Attach(Country obj)
        {
            context.Countries.Attach(obj);
            context.Entry(obj).State = EntityState.Modified;
        }

        public void Dispose()
        {
            context.Dispose();
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}