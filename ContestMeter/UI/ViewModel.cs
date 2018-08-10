using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Practices.Unity;
using System.Windows;

namespace ContestMeter.UI
{
    public abstract class ViewModel<TView> : INotifyPropertyChanged
        where TView : class
    {
        protected ViewModel()
        {
            IsViewOwner = true;
        }
        protected ViewModel(IUnityContainer container, TView view, bool isViewOwner)
        {
            Check.NotNull(container, "container");
            Check.NotNull(view, "view");

            Container = container;
            IsViewOwner = isViewOwner;
            View = view;
        }

        const string IsViewOwnerPropertyName = "IsViewOwner";
        private bool _isViewOwner;
        public bool IsViewOwner
        {
            get { return _isViewOwner; }
            set
            {
                if (_isViewOwner != value)
                {
                    _isViewOwner = value;
                    OnPropertyChanged(IsViewOwnerPropertyName);
                }
            }
        }

        [Dependency]
        public virtual IUnityContainer Container { protected get; set; }

        [Dependency]
        public virtual TView View
        {
            get { return _view; }
            set
            {
                if (_view != value)
                {
                    _view = value;
                    if (_view != null && _view is FrameworkElement && IsViewOwner)
                    {
                        (_view as FrameworkElement).DataContext = this;
                    }

                    OnViewChanged();
                }
            }
        }

        protected virtual void OnViewChanged()
        {
            OnPropertyChanged("View");
        }

        const string IsBusyPropertyName = "IsBusy";
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged(IsBusyPropertyName);
                }
            }
        }

        private TView _view;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
