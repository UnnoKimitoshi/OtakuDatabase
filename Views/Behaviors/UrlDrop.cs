using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Otaku_Database.Views.Behaviors
{
    public class UrlDrop
    {
        public static readonly DependencyProperty UrlDropCmd =
            DependencyProperty.RegisterAttached("Cmd",
                                                typeof(ICommand),
                                                typeof(UrlDrop),
                                                new PropertyMetadata(null, OnPropertyChanged));
        public static ICommand GetCmd(DependencyObject obj) =>
            (ICommand)obj.GetValue(UrlDropCmd);

        public static void SetCmd(DependencyObject obj, ICommand value) =>
            obj.SetValue(UrlDropCmd, value);

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var itemCtrl = d as FrameworkElement;
            if (itemCtrl == null) return;
            var cmd = GetCmd(itemCtrl);

            if (cmd != null)
            {
                itemCtrl.AllowDrop = true;
                itemCtrl.DragOver += OnDragOver;
                itemCtrl.Drop += OnDrop;
            }
            else
            {
                itemCtrl.AllowDrop = false;
                itemCtrl.DragOver -= OnDragOver;
                itemCtrl.Drop -= OnDrop;
            }
        }

        private static void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = (e.Data.GetDataPresent("UniformResourceLocator")) ?
                DragDropEffects.Copy : e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private static void OnDrop(object sender, DragEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null) return;

            var cmd = GetCmd(element);
            var url = e.Data.GetData(DataFormats.Text).ToString();
            e.Handled = true;
            cmd.Execute(url);
        }
    }
}
