using CRSim.Core.Models;

namespace CRSim.Views
{
    /// <summary>
    /// UserInputDialog.xaml 的交互逻辑
    /// </summary>
    public partial class StationStopDialog : Window
    {
        public class ticketCheck
        {
            public string TicketCheck { get; set; }
            public bool Checked { get; set; } = false;
        }

        public ObservableCollection<string> Landmarks { get; set; } = ["无", "红色", "绿色", "褐色", "蓝色", "紫色", "黄色", "橙色"];

        public StationStop GeneratedStationStop;

        public ObservableCollection<ticketCheck> TicketChecksList { get; set; } = [];

        public ObservableCollection<string> Platforms { get; set; } = [];

        public StationStopDialog(List<string> ticketChecks,List<string> platforms)
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
            TicketChecksList.Clear();
            foreach (string s in ticketChecks)
            {
                TicketChecksList.Add(new ticketCheck()
                {
                    TicketCheck = s,
                    Checked = false
                });
            }
            foreach (string s in platforms)
            {
                Platforms.Add(s);
            }
            ValidateInput();
        }

        public StationStopDialog(List<string> ticketChecks, List<string> platforms, StationStop stationStop)
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
            NumberTextBox.IsEnabled = false;
            NumberTextBox.Text = stationStop.Number;
            ArrivalTextBox.Text = stationStop.Terminal;
            DepartureTextBox.Text = stationStop.Origin;
            StationKindPanel.IsEnabled = false;
            if (stationStop.ArrivalTime != null)
            {
                StartHour.Text = stationStop.ArrivalTime.Value.Hour.ToString();
                StartMinute.Text = stationStop.ArrivalTime.Value.Minute.ToString();
            }
            else
            {
                StartingStation.IsChecked = true;
            }
            if (stationStop.DepartureTime != null)
            {
                EndHour.Text = stationStop.DepartureTime.Value.Hour.ToString();
                EndMinute.Text = stationStop.DepartureTime.Value.Minute.ToString();
            }
            else
            {
                FinalStation.IsChecked = true;
            }
            if(!StartingStation.IsChecked.Value && !FinalStation.IsChecked.Value)
            {
                IntermediateStation.IsChecked = true;
            }
            TicketChecksList.Clear();
            foreach (string s in ticketChecks)
            {
                TicketChecksList.Add(new ticketCheck()
                {
                    TicketCheck = s,
                    Checked = stationStop.TicketChecks.Contains(s)
                });
            }
            foreach (string s in platforms)
            {
                Platforms.Add(s);
            }
            PlatformComboBox.SelectedIndex = Platforms.IndexOf(stationStop.Platform);
            LandmarkComboBox.SelectedIndex = stationStop.Landmark == null ? 0 : Landmarks.IndexOf(stationStop.Landmark);
            ValidateInput();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            GeneratedStationStop = new StationStop
            {
                Number = NumberTextBox.Text,
                Terminal = ArrivalTextBox.Text,
                Origin = DepartureTextBox.Text,
                ArrivalTime = StartHour.IsEnabled ? DateTime.Parse($"{StartHour.Text}:{StartMinute.Text}") : null,
                DepartureTime = EndHour.IsEnabled ? DateTime.Parse($"{EndHour.Text}:{EndMinute.Text}") : null,
                TicketChecks = TicketChecksList.Where(x => x.Checked).Select(x => x.TicketCheck).ToList(),
                Platform = (string)PlatformComboBox.SelectedItem,
                Landmark = (string)LandmarkComboBox.SelectedItem == "无" ? null: (string)LandmarkComboBox.SelectedItem,
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

            // 当所有条件满足时，启用 AccentButton
            AccentButton.IsEnabled = areTextBoxesFilled && !string.IsNullOrWhiteSpace(NumberTextBox.Text) && !string.IsNullOrWhiteSpace(ArrivalTextBox.Text) && !string.IsNullOrWhiteSpace(DepartureTextBox.Text) && PlatformComboBox.SelectedItem!=null;
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
            TicketChecksCheckList.IsEnabled = true;
        }

        private void IntermediateStation_Checked(object sender, RoutedEventArgs e)
        {
            if (StartHour == null) return;
            StartHour.IsEnabled = true;
            StartMinute.IsEnabled = true;
            EndHour.IsEnabled = true;
            EndMinute.IsEnabled = true;
            TicketChecksCheckList.IsEnabled = true;
        }

        private void FinalStation_Checked(object sender, RoutedEventArgs e)
        {
            if (StartHour == null) return;
            StartHour.IsEnabled = true;
            StartMinute.IsEnabled = true;
            EndHour.IsEnabled = false;
            EndMinute.IsEnabled = false;
            TicketChecksCheckList.IsEnabled = false;
        }

        private void TicketChecksCheckList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = sender
            };
            TicketChecksCheckList.RaiseEvent(eventArg);
        }

        private void NumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
        }
    }
}