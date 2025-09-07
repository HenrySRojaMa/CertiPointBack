using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesDomain.Entities
{
    public class Catalog
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public int? idParent { get; set; }
    }
}
