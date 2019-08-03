using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.SqlCommand;

namespace DemoNhibernateApp.Repositories
{
    public class Interceptor: EmptyInterceptor
    {
        public override SqlString OnPrepareStatement(SqlString sql)
        {
            if (sql.StartsWithCaseInsensitive("SELECT"))
            {
                var lists = sql.ToString().Split().ToList();
                var from = lists.FirstOrDefault(p => p.Trim().Equals("FROM", StringComparison.OrdinalIgnoreCase));
                var index = from != null ? lists.IndexOf(from) : -1;

                if (index == -1)
                    return sql;

                lists.Insert(lists.IndexOf(from) + 3, "WITH (NOLOCK)");
               
                sql = SqlString.Parse(string.Join(" ", lists));
            }

            return sql;
        }
    }
}
