using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateInitialData.Domain
{
    public class Detail
    {
        public virtual long Id { set; get; }
        public virtual string Note { get; set; }
    }
}
