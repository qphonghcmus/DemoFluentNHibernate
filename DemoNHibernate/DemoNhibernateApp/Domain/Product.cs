using System.Collections.Generic;

namespace DemoNhibernateApp.Domain
{
    public class Product
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Category { get; set; }
        public virtual bool Discontinued { get; set; }
        public virtual ISet<Detail> Detail { get; set; }
    }
}
