
namespace CRSim.Services
{
    public class DialogService : IDialogService
    {
        private Window? _owner;
        public void SetOwner(Window owner)
        {
            _owner = owner;
        }
        public string? GetInput(string title)
        {
            var dialog = new InputDialog(title)
            {
                Owner = _owner
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                string input = dialog.UserInput;
                return input;
            }
            return null;
        }
        public bool GetConfirm(string title)
        {
            var dialog = new ConfirmDialog(title)
            {
                Owner = _owner
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                return true;
            }
            return false;
        }
        public void ShowMessage(string title, string message)
        {
            dynamic dialog = message.Length > 25 ? new LongMessageDialog(title, message) : new MessageDialog(title, message);
            dialog.Owner = _owner;
            dialog.ShowDialog();
        }

        public (string, List<string>) GetInputTicketCheck(List<string> waitingAreaNames)
        {
            var dialog = new TicketCheckDialog(waitingAreaNames)
            {
                Owner = _owner
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                return (dialog.WaitingAreaName, dialog.TicketCheck);
            }
            return (null,null);
        }

        public string? GetFile(string filter)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = filter
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                return filePath;
            }
            return null;
        }
        public TrainStop? GetInputTrainNumberStop(TrainStop? t)
        {
            var dialog = new TrainNumberStopDialog(t)
            {
                Owner = _owner
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                return dialog.GeneratedTrainStop;
            }
            return null;
        }

        public TrainStop? GetInputTrainStop(List<string> ticketChecks, List<string> platforms)
        {
            var dialog = new TrainStopDialog(ticketChecks,platforms)
            {
                Owner = _owner
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                return dialog.GeneratedTrainStop;
            }
            return null;
        }

        public TrainStop? GetInputTrainStop(List<string> ticketChecks, List<string> platforms, TrainStop trainStop)
        {
            var dialog = new TrainStopDialog(ticketChecks,platforms, trainStop)
            {
                Owner = _owner
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                return dialog.GeneratedTrainStop;
            }
            return null;

        }

        public string? SaveFile(string filter, string f)
        {
            SaveFileDialog saveFileDialog = new()
            {
                FileName = f,
                CheckPathExists = true,
                Filter = filter
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                return filePath;
            }
            return null;
        }

        public List<Platform>? GetInputPlatform()
        {
            var dialog = new PlatformDialog()
            {
                Owner = _owner
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                return dialog.GeneratedPlatforms;
            }
            return null;
        }
    }
}
