using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace FluentNHibernateApp.Entities
{
    public class Blog
    {
        public virtual long Id { get; protected set; }
        public virtual string Category { get; set; }
        public virtual string Image { get; set; }
        public virtual long Views { get; set; }
        public virtual DateTime Date_Publish { get; set; }
        public virtual string Summary { get; set; }
        public virtual string Content { get; set; }
        public virtual long Author { get; set; }
        public virtual string Title { get; set; }
        public virtual bool IsActive { get; set; }
    }
}
