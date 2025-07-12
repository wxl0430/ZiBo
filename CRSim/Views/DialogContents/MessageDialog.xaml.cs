namespace CRSim.Views;
public sealed partial class MessageDialog : Page
{
    private string _text { get; set; }
    public MessageDialog(string text)
    {
        _text = text;
        InitializeComponent();
    }
}
