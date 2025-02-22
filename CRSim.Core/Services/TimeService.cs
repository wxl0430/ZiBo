
using CRSim.Core.Models;
using System.Timers;

namespace CRSim.Core.Services
{
    public class TimeService : ITimeService
    {
        private readonly Settings _settings;
        private readonly System.Timers.Timer _oneSecondTimer;
        private readonly System.Timers.Timer _twentySecondsTimer;  

        public event EventHandler OneSecondElapsed; 
        public event EventHandler RefreshSecondsElapsed;  

        public TimeService(ISettingsService settingsService)
        {
            _settings = settingsService.GetSettings();

            _oneSecondTimer = new(1000);  
            _oneSecondTimer.Elapsed += OnOneSecondElapsed;

            _twentySecondsTimer = new(_settings.SwitchPageSeconds*1000);
            _twentySecondsTimer.Elapsed += OnTwentySecondsElapsed;
        }

        public void Start()
        {
            _oneSecondTimer.Start(); 
            _twentySecondsTimer.Start(); 
        }

        public void Stop()
        {
            _oneSecondTimer.Stop();  
            _twentySecondsTimer.Stop();  
        }

        private void OnOneSecondElapsed(object sender, ElapsedEventArgs e)
        {
            OneSecondElapsed?.Invoke(this, EventArgs.Empty);  
        }

        private void OnTwentySecondsElapsed(object sender, ElapsedEventArgs e)
        {
            RefreshSecondsElapsed?.Invoke(this, EventArgs.Empty); 
        }

        public DateTime GetDateTimeNow()
        {
            return  DateTime.Now+ _settings.TimeOffset;
        }
    }
}