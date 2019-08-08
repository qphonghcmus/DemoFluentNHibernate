using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernateApp.Repositories;
using NHibernate;
using NHibernate.Metadata;

namespace FluentNHibernateApp.Helper
{
    public interface IHelper
    {
        IClassMetadata GetClassMetadata(Type type);
        IRepository GetRepositiory();
        ISession GetSession();
        IStatelessSession GetStatelessSession();
        IRepository GetBulkRepository();
    }
}
