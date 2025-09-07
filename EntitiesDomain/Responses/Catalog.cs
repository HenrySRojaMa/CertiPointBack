using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesDomain.Responses
{
    public class CatalogItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? IdParent { get; set; }
        public List<CatalogItem> SubItems { get; set; }
    }
}
