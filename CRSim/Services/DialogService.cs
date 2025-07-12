using CRSim.Views;
using System.Text.RegularExpressions;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace CRSim.Services
{
    public class DialogService : IDialogService
    {
        private XamlRoot? _xamlRoot;
        public void SetXamlRoot(XamlRoot xamlRoot)
        {
            _xamlRoot = xamlRoot;
        }
        public async Task<string?> GetInputAsync(string title)
        {
            ContentDialog dialog = new()
            {
                XamlRoot = _xamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = title,
                PrimaryButtonText = "确定",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                Content = new InputDialog()
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var inputDialog = dialog.Content as InputDialog;
                string input = inputDialog?.InputText??string.Empty;
                return input;
            }
            return null;
        }
        public async Task<bool> GetConfirmAsync(string title)
        {
            ContentDialog dialog = new()
            {
                XamlRoot = _xamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "警告",
                PrimaryButtonText = "确定",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                Content = new MessageDialog(title)
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                return true;
            }
            return false;
        }
        public async Task ShowMessageAsync(string title, string message)
        {
            ContentDialog dialog = new()
            {
                XamlRoot = _xamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = title,
                PrimaryButtonText = "确定",
                DefaultButton = ContentDialogButton.Primary,
                Content = new MessageDialog(message)
            };
            await dialog.ShowAsync();
        }
        public async Task ShowTextAsync(string title, string message)
        {
            ContentDialog dialog = new()
            {
                XamlRoot = _xamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = title,
                PrimaryButtonText = "确定",
                DefaultButton = ContentDialogButton.Primary,
                Content = new TextDialog(message)
            };
            await dialog.ShowAsync();
        }
        public async Task<(string, List<string>)> GetInputTicketCheckAsync(List<string> waitingAreaNames)
        {
            bool isButtonEnabled = false;
            ContentDialog dialog = new()
            {
                XamlRoot = _xamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "新增检票口",
                PrimaryButtonText = "确定",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                IsPrimaryButtonEnabled = false
            };
            var ticketCheckDialog = new TicketCheckDialog(waitingAreaNames, isValid => dialog.IsPrimaryButtonEnabled = isValid);
            dialog.Content = ticketCheckDialog;
            dialog.PrimaryButtonClick += (s, e) =>
            {
                ticketCheckDialog.GenerateTicketChecks();
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var inputDialog = dialog.Content as TicketCheckDialog;
                return (inputDialog.SelectedWaitingArea, inputDialog.GeneratedTicketChecks);
            }
            return (null, null);
        }

        public async Task<string?> GetFileAsync(string[] filter)
        {
            var openPicker = new FileOpenPicker();
            var window = App.MainWindow;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            foreach (var f in filter)
            {
                openPicker.FileTypeFilter.Add(f);
            }
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                return file.Path;
            }
            return null;
        }

        public TrainStop? GetInputTrainNumberStop(TrainStop? t)
        {
            //var dialog = new TrainNumberStopDialog(t)
            //{
            //    Owner = _owner
            //};
            //bool? result = dialog.ShowDialog();
            //if (result == true)
            //{
            //    return dialog.GeneratedTrainStop;
            //}
            return null;
        }

        public async Task<TrainStop?> GetInputTrainStopAsync(List<string> ticketChecks, List<string> platforms)
        {
            bool isButtonEnabled = false;
            ContentDialog dialog = new()
            {
                XamlRoot = _xamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "编辑车次",
                PrimaryButtonText = "确定",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                IsPrimaryButtonEnabled = false
            };
            var platformDialog = new TrainStopDialog(ticketChecks,platforms,isValid => dialog.IsPrimaryButtonEnabled = isValid);
            dialog.Content = platformDialog;
            dialog.PrimaryButtonClick += (s, e) =>
            {
                platformDialog.GenerateTrainStop();
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var inputDialog = dialog.Content as TrainStopDialog;
                return inputDialog.GeneratedTrainStop;
            }
            return null;
        }

        public async Task<TrainStop?> EditInputTrainStopAsync(List<string> ticketChecks, List<string> platforms, TrainStop trainStop)
        {
            bool isButtonEnabled = false;
            ContentDialog dialog = new()
            {
                XamlRoot = _xamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "编辑车次",
                PrimaryButtonText = "确定",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                IsPrimaryButtonEnabled = false
            };
            var platformDialog = new TrainStopDialog(ticketChecks, platforms, trainStop, isValid => dialog.IsPrimaryButtonEnabled = isValid);
            dialog.Content = platformDialog;
            dialog.PrimaryButtonClick += (s, e) =>
            {
                platformDialog.GenerateTrainStop();
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var inputDialog = dialog.Content as TrainStopDialog;
                return inputDialog.GeneratedTrainStop;
            }
            return null;
        }

        public async Task<string?> SaveFileAsync(string filter, string f)
        {
            FileSavePicker savePicker = new();
            var window = App.MainWindow;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.SuggestedFileName = f;
            savePicker.FileTypeChoices.Add("File", [filter]);
            StorageFile file = await savePicker.PickSaveFileAsync();
            return file.Path;
        }

        public async Task<List<Platform>?> GetInputPlatformAsync()
        {
            bool isButtonEnabled = false;
            ContentDialog dialog = new()
            {
                XamlRoot = _xamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "新增站台",
                PrimaryButtonText = "确定",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                IsPrimaryButtonEnabled = false
            };
            var platformDialog = new PlatformDialog(isValid => dialog.IsPrimaryButtonEnabled = isValid);
            dialog.Content = platformDialog;
            dialog.PrimaryButtonClick += (s, e) =>
            {
                platformDialog.GeneratePlatform();
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var inputDialog = dialog.Content as PlatformDialog;
                return inputDialog.GeneratedPlatforms;
            }
            return null;
        }
    }
}
