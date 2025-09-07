using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesDomain.Queries
{
    public class CatalogListQuery
    {
        public string? selected { get; set; }
        public string? except { get; set; }
        public int? startDepth { get; set; }
        public int? endDepth { get; set; }
    }
}
