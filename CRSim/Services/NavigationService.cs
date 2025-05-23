﻿namespace CRSim.Services
{
    public class NavigationService(IServiceProvider serviceProvider) : INavigationService
    {
        private Frame _frame;

        private Type _currentPageType = null;

        private readonly Stack<Type> _history = new();
        private readonly Stack<Type> _future = new();

        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public event EventHandler<NavigatingEventArgs> Navigating;

        public void SetFrame(Frame frame)
        {
            _frame = frame;
        }

        public void NavigateTo(Type type)
        {
            if (type != null)
            {
                _future.Clear();
                RaiseNavigatingEvent(type);
            }
        }

        public void Navigate(Type type)
        {
            if (type != null)
            {
                _history.Push(_currentPageType);
                _currentPageType = type;
                var page = _serviceProvider.GetRequiredService(type);
                _frame.Navigate(page);
            }
        }

        public void NavigateBack()
        {
            if (_history.Count > 0)
            {
                Type type = _history.Pop();
                if (type != null)
                {
                    _future.Push(type);
                    RaiseNavigatingEvent(type);
                    _history.Pop();
                }
            }
        }

        public void NavigateForward()
        {
            if (_future.Count > 0)
            {
                Type type = _future.Pop();
                if (type != null)
                {
                    _history.Push(type);
                    RaiseNavigatingEvent(type);
                }
            }
        }

        public void RaiseNavigatingEvent(Type type)
        {
            Navigating?.Invoke(this, new NavigatingEventArgs(type));
        }

        public bool IsBackHistoryNonEmpty()
        {
            var item = _history.Peek();
            if (item == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
