using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CRSim.ViewModels;

namespace CRSim.Views
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPageViewModel ViewModel { get; }

        public SettingsPage(SettingsPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("^[0-9]+$"); 
            e.Handled = !regex.IsMatch(e.Text);
        }
        private void NumberValidationTextBox2(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("^-?[0-9]*$"); 
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
