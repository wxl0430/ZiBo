using CRSim.Core.Services;
using CRSim.ScreenSimulator.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace CRSim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Settings _settings;
        private readonly INavigationService _navigationService;
        public MainWindowViewModel ViewModel { get; }
        public MainWindow(MainWindowViewModel viewModel, IServiceProvider serviceProvider, INavigationService navigationService, ISettingsService settingsService)
        {
            _serviceProvider = serviceProvider;
            _settings = settingsService.GetSettings();
            DataContext = this;
            ViewModel = viewModel;
            InitializeComponent();

            UpdateWindowBackground();
            UpdateTitleBarButtonsVisibility();

            _navigationService = navigationService;
            _navigationService.Navigating += OnNavigating;
            _navigationService.SetFrame(this.RootContentFrame);
            _navigationService.Navigate(typeof(DashboardPage));

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

            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
            this.StateChanged += MainWindow_StateChanged;
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                MainGrid.Margin = new Thickness(8);
            }
            else
            {
                MainGrid.Margin = default;
            }
        }
        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateTitleBarButtonsVisibility();
            });
        }

        private void OnNavigating(object? sender, NavigatingEventArgs e)
        {
            List<ControlInfoDataItem> list = ViewModel.GetNavigationItemHierarchyFromPageType(e.PageType);

            if (list.Count > 0)
            {
                TreeViewItem selectedTreeViewItem = null;
                ItemsControl itemsControl = ControlsList;
                foreach (ControlInfoDataItem item in list)
                {
                    if (itemsControl.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem tvi)
                    {
                        tvi.IsExpanded = true;
                        tvi.UpdateLayout();
                        itemsControl = tvi;
                        selectedTreeViewItem = tvi;
                    }
                }

                if (selectedTreeViewItem != null)
                {
                    selectedTreeViewItem.IsSelected = true;
                    ControlsList_SelectedItemChanged();
                }
            }
        }

        private void UpdateWindowBackground()
        {
            if ((!Utilities.IsBackdropDisabled() &&
                        !Utilities.IsBackdropSupported()))
            {
                this.SetResourceReference(BackgroundProperty, "WindowBackground");
            }
        }
        private void ControlsList_SelectedItemChanged()
        {
            if (ControlsList.SelectedItem is ControlInfoDataItem navItem)
            {
                _navigationService.Navigate(navItem.PageType);
                var tvi = ControlsList.ItemContainerGenerator.ContainerFromItem(navItem) as TreeViewItem;
                tvi?.BringIntoView();
            }
        }
        private void UpdateTitleBarButtonsVisibility()
        {
            if (Utilities.IsBackdropDisabled() || !Utilities.IsBackdropSupported() ||
                    SystemParameters.HighContrast == true)
            {
                MinimizeButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Visible;
                CloseButton.Visibility = Visibility.Visible;
            }
            else
            {
                MinimizeButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Collapsed;
                CloseButton.Visibility = Visibility.Collapsed;
            }
        }
        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                MaximizeIcon.Text = "\uE922";
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                MaximizeIcon.Text = "\uE923";
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            AutomationPeer peer = UIElementAutomationPeer.CreatePeerForElement((Button)sender);
            peer.RaiseNotificationEvent(
               AutomationNotificationKind.Other,
                AutomationNotificationProcessing.ImportantMostRecent,
                "Settings Page Opened",
                "ButtonClickedActivity"
            );
        }

        private void ControlsList_Loaded(object sender, RoutedEventArgs e)
        {
            if (ControlsList.Items.Count > 0)
            {
                TreeViewItem firstItem = (TreeViewItem)ControlsList.ItemContainerGenerator.ContainerFromItem(ControlsList.Items[0]);
                if (firstItem != null)
                {
                    firstItem.IsSelected = true;
                }
            }
        }

        private void RootContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            ViewModel.UpdateCanNavigateBack();
        }

        private void ControlsList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SelectedItemChanged(ControlsList.ItemContainerGenerator.ContainerFromItem((sender as TreeView).SelectedItem) as TreeViewItem);
            }
        }

        private void ControlsList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is ToggleButton)
            {
                return;
            }
            SelectedItemChanged(ControlsList.ItemContainerGenerator.ContainerFromItem((sender as TreeView).SelectedItem) as TreeViewItem);
        }
        private void SelectedItemChanged(TreeViewItem? tvi)
        {
            ControlsList_SelectedItemChanged();
            if (tvi != null)
            {
                tvi.IsExpanded = !tvi.IsExpanded;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_settings.ReopenUnclosedScreensOnLoad)
            {
                await ReopenUnclosedScreensAsync();
            }
        }

        private async Task ReopenUnclosedScreensAsync()
        {
            List<ScreenInfo> screenInfos = [];
            if (File.Exists("Screen.json"))
            {
                var fileContent = await File.ReadAllTextAsync("Screen.json");
                screenInfos = JsonSerializer.Deserialize<List<ScreenInfo>>(fileContent) ?? [];
            }
            else
            {
                return;
            }
            foreach (var screenInfo in screenInfos)
            {
                var Window = _serviceProvider.GetRequiredService(StylesInfoDataSource.Instance.StylesInfo.Where(x => x.ViewType.FullName == screenInfo.SelectedStyle).FirstOrDefault().ViewType) as dynamic;
                Window.SessionID = screenInfo.SessionID;
                if (!string.IsNullOrEmpty(screenInfo.Text))
                {
                    Window.ViewModel.Text = screenInfo.Text;
                }
                if (screenInfo.SelectedLoaction != null && screenInfo.SelectedLoaction != 0)
                {
                    Window.ViewModel.Location = screenInfo.SelectedLoaction.Value;
                }
                if (screenInfo.Video != null && screenInfo.Video != string.Empty)
                {
                    Window.ViewModel.Video = new Uri(screenInfo.Video, UriKind.Absolute);
                }

                Window.ViewModel.LoadData(_serviceProvider.GetRequiredService<IDatabaseService>().GetStationByName(screenInfo.SelectedStationName),
                    screenInfo.SelectedTicketCheck ?? string.Empty,
                    screenInfo.SelectedPlatformName ?? string.Empty); ;
                Window.Show();
            }
        }
    }
}