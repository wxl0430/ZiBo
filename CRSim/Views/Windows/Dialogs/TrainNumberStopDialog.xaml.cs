using CRSim.Core.Models;

namespace CRSim.Views
{
    /// <summary>
    /// UserInputDialog.xaml 的交互逻辑
    /// </summary>
    public partial class TrainNumberStopDialog : Window
    {
        public TrainStop GeneratedTrainStop;

        public TrainNumberStopDialog(TrainStop? trainStop)
        {
            DataContext = this;
            WindowChrome.SetWindowChrome(
                this,
                new WindowChrome
                {
                    CaptionHeight = 50,
                    CornerRadius = default,
                    GlassFrameThickness = new Thickness(-1),
                    ResizeBorderThickness = ResizeMode == ResizeMode.NoResize ? default : new Thickness(4),
                    UseAeroCaptionButtons = true
                }
            );
            InitializeComponent();
            if (trainStop != null)
            {
                NameTextBox.IsEnabled = false;
                NameTextBox.Text = trainStop.Station;
                StationKindPanel.IsEnabled = false;
                if (trainStop.ArrivalTime != null)
                {
                    StartHour.Text = trainStop.ArrivalTime.Value.Hours.ToString();
                    StartMinute.Text = trainStop.ArrivalTime.Value.Minutes.ToString();
                }
                else
                {
                    StartingStation.IsChecked = true;
                }
                if (trainStop.DepartureTime != null)
                {
                    EndHour.Text = trainStop.DepartureTime.Value.Hours.ToString();
                    EndMinute.Text = trainStop.DepartureTime.Value.Minutes.ToString();
                }
                else
                {
                    FinalStation.IsChecked = true;
                }
                if (!StartingStation.IsChecked.Value && !FinalStation.IsChecked.Value)
                {
                    IntermediateStation.IsChecked = true;
                }
            }
            ValidateInput();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            GeneratedTrainStop = new TrainStop
            {
                Station = NameTextBox.Text,
                ArrivalTime = StartHour.IsEnabled ? TimeSpan.Parse($"{StartHour.Text}:{StartMinute.Text}") : null,
                DepartureTime = EndHour.IsEnabled ? TimeSpan.Parse($"{EndHour.Text}:{EndMinute.Text}") : null,
            };
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void StartHour_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
            if (StartHour.Text.Length == 2) StartMinute.Focus();
        }

        private void StartMinute_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
            if (StartMinute.Text.Length == 2) EndHour.Focus();
        }

        private void EndHour_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
            if (EndHour.Text.Length == 2) EndMinute.Focus();
        }

        private void EndMinute_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
            if (EndMinute.Text.Length == 2) AccentButton.Focus();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ValidateInput();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ValidateInput();
        }

        private static bool ValidateTime(string input, int maxValue)
        {
            if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, null, out int time))
            {
                return 0 <= time && time < maxValue;
            }
            return false;
        }

        private void ValidateInput()
        {
            // 检查四个文本框是否非空
            bool areTextBoxesFilled =
                (!StartHour.IsEnabled || ValidateTime(StartHour.Text, 24)) &&
                (!StartMinute.IsEnabled || ValidateTime(StartMinute.Text, 60)) &&
                (!EndHour.IsEnabled || ValidateTime(EndHour.Text, 24)) &&
                (!EndMinute.IsEnabled || ValidateTime(EndMinute.Text, 60));

            AccentButton.IsEnabled =
                areTextBoxesFilled &&
                !string.IsNullOrWhiteSpace(NameTextBox.Text);
        }

        private void StartingStation_Checked(object sender, RoutedEventArgs e)
        {
            if (StartHour == null) return;
            StartHour.IsEnabled = false;
            StartMinute.IsEnabled = false;
            EndHour.IsEnabled = true;
            EndMinute.IsEnabled = true;
        }

        private void IntermediateStation_Checked(object sender, RoutedEventArgs e)
        {
            if (StartHour == null) return;
            StartHour.IsEnabled = true;
            StartMinute.IsEnabled = true;
            EndHour.IsEnabled = true;
            EndMinute.IsEnabled = true;
        }

        private void FinalStation_Checked(object sender, RoutedEventArgs e)
        {
            if (StartHour == null) return;
            StartHour.IsEnabled = true;
            StartMinute.IsEnabled = true;
            EndHour.IsEnabled = false;
            EndMinute.IsEnabled = false;
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
        }
    }
}