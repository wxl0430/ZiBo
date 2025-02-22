using static CRSim.Views.TrainStopDialog;

namespace CRSim.Views
{
    /// <summary>
    /// UserInputDialog.xaml 的交互逻辑
    /// </summary>
    public partial class TrainStopDialog : Window
    {
        public TrainStop GeneratedTrainStop;

        public TrainStopDialog()
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
            IntermediateStation.IsChecked = true;
            ValidateInput();
        }

        public TrainStopDialog(TrainStop trainStop)
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
            StationTextBox.IsEnabled = false;
            StationTextBox.Text = trainStop.Station;
            StationKindPanel.IsEnabled = false;
            if (trainStop.ArrivalTime != null)
            {
                StartHour.Text = trainStop.ArrivalTime.Value.Hour.ToString();
                StartMinute.Text = trainStop.ArrivalTime.Value.Minute.ToString();
            }
            else
            {
                StartingStation.IsChecked = true;
            }
            if (trainStop.DepartureTime != null)
            {
                EndHour.Text = trainStop.DepartureTime.Value.Hour.ToString();
                EndMinute.Text = trainStop.DepartureTime.Value.Minute.ToString();
            }
            else
            {
                FinalStation.IsChecked = true;
            }
            ValidateInput();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            GeneratedTrainStop = new TrainStop
            {
                Station = StationTextBox.Text,
                ArrivalTime = StartHour.IsEnabled ? DateTime.Parse($"{StartHour.Text}:{StartMinute.Text}") : null,
                DepartureTime = EndHour.IsEnabled ? DateTime.Parse($"{EndHour.Text}:{EndMinute.Text}") : null
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

        private static bool ValidateTime(string input,int maxValue)
        {
            if(!string.IsNullOrWhiteSpace(input) && int.TryParse(input,null,out int time))
            {
                return 0 <= time && time < maxValue;
            }
            return false;
        }

        private void ValidateInput()
        {
            // 检查四个文本框是否非空
            bool areTextBoxesFilled =
                (!StartHour.IsEnabled || ValidateTime(StartHour.Text,23)) &&
                (!StartMinute.IsEnabled || ValidateTime(StartMinute.Text, 59)) &&
                (!EndHour.IsEnabled || ValidateTime(EndHour.Text, 23)) &&
                (!EndMinute.IsEnabled || ValidateTime(EndMinute.Text, 59));

            // 当所有条件满足时，启用 AccentButton
            AccentButton.IsEnabled = areTextBoxesFilled && !string.IsNullOrWhiteSpace(StationTextBox.Text);
        }

        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj == null)
                return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T t)
                {
                    return t;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
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

        private void StationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
        }
    }
}
