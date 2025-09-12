
namespace EntitiesDomain.Entities
{
    public class Option
    {
        public int IdOption { get; set; }
        public string OptionName { get; set; }
        public string Route { get; set; }
        public int IdParent { get; set; }
        public string Icon { get; set; }
    }
    public class Actions
    {
        public int IdOption { get; set; }
        public int IdAction { get; set; }
        public string ActionName { get; set; }
        public string ActionCode { get; set; }
    }
}
