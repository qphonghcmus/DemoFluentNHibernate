using System;
using DemoNhibernateApp.Domain;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Metadata;

namespace DemoNhibernateApp.Repositories
{
    public class NHibernateHelper
    {

        private static ISessionFactory _sessionFactory;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    var configuration = new Configuration();
                    configuration.Configure();
                    configuration.AddAssembly(typeof(Product).Assembly);
                    _sessionFactory = configuration.BuildSessionFactory();
                }

                return _sessionFactory;
            }
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession(new Interceptor());
        }

        public static IStatelessSession OpenStatelessSession()
        {
            return SessionFactory.OpenStatelessSession();
        }

        public static IClassMetadata GetClassMetadata(Type type)
        {
            return SessionFactory.GetClassMetadata(type);
        }

        public static IRepositiory GetRepositiory()
        {
            return new Repository(SessionFactory.OpenSession(new Interceptor()));
        }
    }
}
