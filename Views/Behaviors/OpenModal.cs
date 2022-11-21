using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Otaku_Database.Views.Behaviors
{
    public class OpenModal
    {
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(OpenModal), new PropertyMetadata(false));

        public static bool GetIsOpen(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsOpenProperty);
        }
        public static void SetIsModal(DependencyObject obj, bool value)
        {
            obj.SetValue(IsOpenProperty, value);
        }

        public static object GetWindowViewModel(DependencyObject obj)
        {
            return (object)obj.GetValue(DataContextProperty);
        }
        public static void SetWindowViewModel(DependencyObject obj, object value)
        {
            obj.SetValue(DataContextProperty, value);
        }
        // Using a DependencyProperty as the backing store for WindowViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.RegisterAttached("DataContext", typeof(object), typeof(OpenModal), new PropertyMetadata(null, OnDataContextChanged));

        private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            if (element == null) return;

            
        }
    }
}
