using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Mapping;
using FluentNHibernateApp.Entities;

namespace FluentNHibernateApp.Mappings
{
    public class BlogMap : ClassMap<Blog>
    {
        public BlogMap()
        {
            //Table("Blog_2");
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Category);
            Map(x => x.Image);
            Map(x => x.Views);
            Map(x => x.Date_Publish);
            Map(x => x.Summary).Index("index_summary");
            Map(x => x.Content);
            Map(x => x.Author)/*.Index("index_author")*/;
            Map(x => x.Title);
            Map(x => x.IsActive);
        }
    }
}
