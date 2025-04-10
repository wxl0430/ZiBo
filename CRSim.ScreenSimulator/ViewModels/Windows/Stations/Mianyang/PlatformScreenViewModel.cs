﻿using CRSim.Core.Services;
using CRSim.ScreenSimulator.Models;
using System.Collections.ObjectModel;
using System.Windows;
namespace CRSim.ScreenSimulator.ViewModels.Mianyang
{
    public class PlatformScreenViewModel : ScreenViewModel
    {
        private ITimeService _timeService;
        public ObservableCollection<TrainInfo> Screen { get; set; } = [];
        public PlatformScreenViewModel(ITimeService timeService, ISettingsService settingsService)
            : base(timeService, settingsService)
        {
            _timeService = timeService;
            timeService.RefreshSecondsElapsed += RefreshDisplay;
            Initialize();
        }
        private async void Initialize()
        {
            await WaitForDataLoadAsync();
            Text = ThisPlatform;
            RefreshDisplay(null,null);
        }
        private void RefreshDisplay(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Screen.Add(TrainInfo.Count > 0 ? TrainInfo[0] : new());
                if (Screen.Count == 1) return;
                Screen.RemoveAt(0);
            });
        }
        public override void RefreshData(object? sender, EventArgs e)
        {
            if (CurrentPageIndex == 0)
            {
                List<TrainInfo> itemsToRemove = [];
                foreach (TrainInfo trainInfo in TrainInfo)
                {
                    if (trainInfo.DepartureTime == null)
                    {
                        if (trainInfo.ArrivalTime.Value.Add(_settings.StopDisplayFromArrivalDuration) < _timeService.GetDateTimeNow())
                        {
                            itemsToRemove.Add(trainInfo);
                        }
                    }
                    else
                    {
                        if (trainInfo.DepartureTime.Value < _timeService.GetDateTimeNow())
                        {
                            itemsToRemove.Add(trainInfo);
                        }
                    }
                }
                foreach (var item in itemsToRemove)
                {
                    TrainInfo.Remove(item);
                }
            }
        }
    }
}
