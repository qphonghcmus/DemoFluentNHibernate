using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernateApp.Entities;
using FluentNHibernateApp.Repositories;
using NHibernate;
using NHibernate.Metadata;
using NHibernate.Tool.hbm2ddl;

namespace FluentNHibernateApp.Views
{
    public class ViewHelper
    {
        #region properties

        private static ISessionFactory _sessionFactory;
        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    try
                    {
                        _sessionFactory = Fluently.Configure()
                            .Database(MsSqlConfiguration.MsSql2012.ConnectionString(@"Data Source=DESKTOP-3FU2P3A\QUOC_PHONG;Initial Catalog=DemoFNH_Sub;Integrated Security=True"))
                            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Blog>())
                            //.ExposeConfiguration(cfg =>
                            //{
                            //    //cfg.SetProperty("adonet.batch_size", "100");
                            //    //cfg.SetProperty("connection.release_mode", "on_close");
                            //    new SchemaUpdate(cfg).Execute(false, true);
                            //})
                            .BuildSessionFactory();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                }

                return _sessionFactory;
            }
        }

        #endregion

        #region Methods

        public static IClassMetadata GetClassMetadata(Type type)
        {
            return SessionFactory.GetClassMetadata(type);
        }

        public static IViewAPI GetViewApi()
        {
            return new ViewAPI(SessionFactory.OpenSession(new Interceptor()));
        }

        public static ISession GetSession()
        {
            return SessionFactory.OpenSession();
        }

        public static IStatelessSession GetStatelessSession()
        {
            return SessionFactory.OpenStatelessSession();
        }


        #endregion
    }
}
