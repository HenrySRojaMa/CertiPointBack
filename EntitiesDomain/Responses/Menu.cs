using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesDomain.Responses
{
    public class MenuOption
    {
        public int IdOption { get; set; }
        public string OptionName { get; set; }
        public string Route { get; set; }
        public int IdParent { get; set; }
        public string Icon { get; set; }
        public List<MenuAction> Actions { get; set; }
        public List<MenuOption> SubOptions { get; set; }
    }
    public class MenuAction
    {
        public int IdAction { get; set; }
        public string ActionName { get; set; }
        public string ActionCode { get; set; }
    }
}
