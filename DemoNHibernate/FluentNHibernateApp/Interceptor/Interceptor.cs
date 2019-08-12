using System;
using System.Linq;
using NHibernate;
using NHibernate.SqlCommand;

namespace FluentNHibernateApp.Interceptor
{
    public class Interceptor : EmptyInterceptor
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

                // Add hint with nolock to sql string
                lists.Insert(lists.IndexOf(from) + 3, "WITH (NOLOCK)");

                sql = SqlString.Parse(string.Join(" ", lists));
            }
            return sql;
        }
    }
}
