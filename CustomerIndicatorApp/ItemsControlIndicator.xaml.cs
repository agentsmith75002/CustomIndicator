using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CustomerIndicatorApp
{
    public sealed partial class ItemsControlIndicator : UserControl//, INotifyPropertyChanged
    {
        public ItemsControlIndicator()
        {
            this.InitializeComponent();
            DataContext = new IndicatorDataContext { IndexText = "ItemsControlIndicator" };
        }


        public IEnumerable ItemsControlSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsControlSource", typeof(IEnumerable), typeof(ItemsControlIndicator), 
                new PropertyMetadata(null, ItemsControlSourceChanged));

        private static void ItemsControlSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var indicator = d as ItemsControlIndicator;
            if(indicator != null)
            {
                var itemsSource = new List<ItemElement>();
                int i = 0;
                foreach (var item in (e.NewValue as IEnumerable))
                {
                    itemsSource.Add(new ItemElement { Index = i++, Selected = false });
                }
                (indicator.DataContext as IndicatorDataContext).ItemsSource = itemsSource;
            }
        }

        public object ItemControlSelected
        {
            get { return (object)GetValue(ItemControlSelectedProperty); }
            set { SetValue(ItemControlSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemControlSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemControlSelectedProperty =
            DependencyProperty.Register("ItemControlSelected", typeof(object), typeof(ItemsControlIndicator), 
                new PropertyMetadata(null, ItemControlSelectedChanged));

        private static void ItemControlSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var indicator = d as ItemsControlIndicator;
            if (indicator != null)
            {
                var dataContext = indicator.DataContext as IndicatorDataContext;
                int i = 0;
                foreach (var item in indicator.ItemsControlSource)
                {
                    dataContext.ItemsSource.ElementAt(i++).Selected = item.Equals(e.NewValue);
                }
                dataContext.ItemSelected = dataContext.ItemsSource.FirstOrDefault(item=>item.Selected);
            }
        }

        public Brush IndicatorForeColor
        {
            get { return (Brush)GetValue(IndicatorForeColorProperty); }
            set { SetValue(IndicatorForeColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IndicatorForeColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorForeColorProperty =
            DependencyProperty.Register("IndicatorForeColor", typeof(Brush), typeof(ItemsControlIndicator), null);



        public Brush IndicatorBackColor
        {
            get { return (Brush)GetValue(IndicatorBackColorProperty); }
            set { SetValue(IndicatorBackColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IndicatorBackColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorBackColorProperty =
            DependencyProperty.Register("IndicatorBackColor", typeof(Brush), typeof(ItemsControlIndicator), null);



        public Brush IndicatorBackground
        {
            get { return (Brush)GetValue(IndicatorBackgroundProperty); }
            set { SetValue(IndicatorBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IndicatorBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorBackgroundProperty =
            DependencyProperty.Register("IndicatorBackground", typeof(Brush), typeof(ItemsControlIndicator), null);
    }

    class IndicatorDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _indexText;
        public string IndexText
        {
            get { return _indexText; }
            set
            {
                _indexText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IndexText)));
            }
        }

        private List<ItemElement> _itemsSource = new List<ItemElement>();
        internal List<ItemElement> ItemsSource
        {
            get { return _itemsSource; }
            set
            {
                _itemsSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemsSource)));
            }
        }

        private ItemElement _itemSelected;
        internal ItemElement ItemSelected
        {
            get { return _itemSelected; }
            set
            {
                _itemSelected = value;
                IndexText = $"Index{_itemSelected.Index} selection is {_itemSelected.Selected}";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemSelected)));
            }
        }
    }

    class ItemElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    
        public int Index { get; set; }

        private bool _selected;
        internal bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selected)));
            }
        }
    }

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
