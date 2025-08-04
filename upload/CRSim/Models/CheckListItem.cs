namespace CRSim.Models
{
    public partial class CheckListItem : ObservableObject
    {
        [ObservableProperty]
        public partial bool IsSelected { get; set; }
        public string Name { get; set; }
    }
}
