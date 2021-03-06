﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CustomerIndicatorApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
    }

    namespace ViewModels
    {
        using System.Reflection;

        partial class Locator : Common.BindableBase
        {
            MainPageViewModel sMainPageViewModel = default(MainPageViewModel);
            public MainPageViewModel MainPageViewModel
            {
                get
                {
                    if (sMainPageViewModel == null)
                    {
                        sMainPageViewModel = new MainPageViewModel();
                        if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                            sMainPageViewModel.LoadDesignTimeData();
                        else
                            sMainPageViewModel.LoadDataCommand.Execute();
                    }
                    return sMainPageViewModel;
                }
            }
        }

        class MainPageViewModel : Common.ViewModelBase
        {
            System.Collections.ObjectModel.ObservableCollection<Models.ColorInfo> _Colors
                = new System.Collections.ObjectModel.ObservableCollection<Models.ColorInfo>();
            public System.Collections.ObjectModel.ObservableCollection<Models.ColorInfo> Colors { get { return _Colors; } }

            Models.ColorInfo _SelectedColor = default(Models.ColorInfo);
            public Models.ColorInfo SelectedColor
            {
                get { return _SelectedColor; }
                set
                {
                    base.SetProperty(ref _SelectedColor, value);
                    foreach (var item in this.Colors.Where(x => x.Selected))
                        item.Selected = false;
                    if (value != null)
                        value.Selected = true;
                }
            }

            public void LoadDesignTimeData()
            {
                var colors = typeof(Windows.UI.Colors).GetRuntimeProperties()
                    .Select(x => new Models.ColorInfo { Name = x.Name, Color = (Windows.UI.Color)x.GetValue(null) });
                this.Colors.Clear();
                foreach (var color in colors.OrderBy(x => Guid.NewGuid()).Take(10))
                    this.Colors.Add(color);
                this.SelectedColor = this.Colors[1];
            }

            Common.RelayCommand _LoadDataCommand = null;
            public Common.RelayCommand LoadDataCommand
            {
                get
                {
                    if (_LoadDataCommand == null)
                    {
                        _LoadDataCommand = new Common.RelayCommand
                        (
                            () => { /* TODO */ this.LoadDesignTimeData(); },
                            () => { return true; }
                        );
                        this.PropertyChanged += (s, e) => _LoadDataCommand.RaiseCanExecuteChanged();
                    }
                    return _LoadDataCommand;
                }
            }

            Common.RelayCommand<Models.ColorInfo> _SelectCommand = null;
            public Common.RelayCommand<Models.ColorInfo> SelectCommand
            {
                get
                {
                    if (_SelectCommand == null)
                    {
                        _SelectCommand = new Common.RelayCommand<Models.ColorInfo>
                        (
                            (o) => { this.SelectedColor = o; },
                            (o) => { return true; }
                        );
                        this.PropertyChanged += (s, e) => _SelectCommand.RaiseCanExecuteChanged();
                    }
                    return _SelectCommand;
                }
            }
        }
    }

    namespace Models
    {
        class ColorInfo : Common.ModelBase
        {
            public string Name { get; set; }
            public Windows.UI.Color Color { get; set; }
            public SolidColorBrush Brush { get { return new SolidColorBrush(this.Color); } }

            bool _Selected = default(bool);
            public bool Selected { get { return _Selected; } set { base.SetProperty(ref _Selected, value); } }
        }
    }

    namespace Converters
    {
        class VisibleWhenTrue : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, string language)
            {
                if (System.Convert.ToBoolean(value))
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }

            public object ConvertBack(object value, Type targetType, object parameter, string language)
            {
                throw new NotImplementedException();
            }
        }
    }

    namespace Common
    {
        abstract class ModelBase : Common.BindableBase { }

        abstract class ViewModelBase : Common.BindableBase { }

        abstract class BindableBase : System.ComponentModel.INotifyPropertyChanged
        {
            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
            protected void SetProperty<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] String propertyName = null)
            {
                if (!object.Equals(storage, value))
                {
                    storage = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
                }
            }
            protected void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = null)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        sealed class RelayCommand : System.Windows.Input.ICommand
        {
            readonly Func<bool> _canExecute;
            readonly Action _execute;

            public RelayCommand(Action execute) : this(execute, null) { }
            public RelayCommand(Action execute, Func<bool> canExecute)
            {
                if (execute == null)
                    throw new ArgumentNullException("execute");
                _execute = execute;
                _canExecute = canExecute;
            }

            public bool CanExecute() { return this.CanExecute(null); }
            public bool CanExecute(object parameter) { return _canExecute == null ? true : _canExecute(); }
            public void Execute() { this.Execute(null); }
            public void Execute(object parameter) { _execute(); }
            public event EventHandler CanExecuteChanged;
            public void RaiseCanExecuteChanged()
            {
                EventHandler handler = CanExecuteChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        sealed class RelayCommand<TParameter> : System.Windows.Input.ICommand
        {
            readonly Func<TParameter, bool> _canExecute;
            readonly Action<TParameter> _execute;

            public RelayCommand(Action<TParameter> execute) : this(execute, null) { }
            public RelayCommand(Action<TParameter> execute, Func<TParameter, bool> canExecute)
            {
                if (execute == null)
                    throw new ArgumentNullException("execute");
                _execute = execute;
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) { return _canExecute == null ? true : _canExecute((TParameter)parameter); }
            public void Execute(object parameter) { _execute((TParameter)parameter); }
            public event EventHandler CanExecuteChanged;
            public void RaiseCanExecuteChanged()
            {
                EventHandler handler = CanExecuteChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }
    }
}
