using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using netly;
using netly.Models;

namespace netly.Helpers
{
    public interface IRepository
    {
        IQueryable<UrlMap> FindUrlMaps();
        IQueryable<UrlMapDetailView> FindUrlMapDetails();
        IQueryable<UrlAggregate> FindUrlAggregates();
        IQueryable<UrlHistory> FindUrlHistories();
        IQueryable<UrlHistoryDetailView> FindUrlHistoryDetails();
        IQueryable<Country> FindCountries();
        IQueryable<ApiKey> FindApiKeys();
        //IQueryable<SQLDatesTable> FindSQLDates();

        void DeleteObject(UrlMap obj);
        void AddObject(UrlMap obj);
        void Attach(UrlMap obj);

        void DeleteObject(UrlAggregate obj);
        void AddObject(UrlAggregate obj);
        void Attach(UrlAggregate obj);

        void DeleteObject(UrlHistory obj);
        void AddObject(UrlHistory obj);
        void Attach(UrlHistory obj);

        void DeleteObject(Country obj);
        void AddObject(Country obj);
        void Attach(Country obj);

        void Dispose();
        void SaveChanges();
    }
}
