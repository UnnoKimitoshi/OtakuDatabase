using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Otaku_Database.Views.Behaviors
{
    public class FileDrop
    {
        public static readonly DependencyProperty FileDropCmd =
            DependencyProperty.RegisterAttached("Cmd",
                                                typeof(ICommand),
                                                typeof(FileDrop),
                                                new PropertyMetadata(null, OnPropertyChanged));
        public static ICommand GetCmd(DependencyObject obj) =>
            (ICommand)obj.GetValue(FileDropCmd);

        public static void SetCmd(DependencyObject obj, ICommand value) =>
            obj.SetValue(FileDropCmd, value);

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            if (element == null) return;
            var cmd = GetCmd(element);

            if (cmd != null)
            {
                element.AllowDrop = true;
                element.DragOver += OnDragOver;
                element.Drop += OnDrop;
            }
            else
            {
                element.AllowDrop = false;
                element.DragOver -= OnDragOver;
                element.Drop -= OnDrop;
            }
        }

        private static void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = (e.Data.GetDataPresent(DataFormats.FileDrop)) ?
                DragDropEffects.Copy : e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private static void OnDrop(object sender, DragEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null) return;

            var cmd = GetCmd(element);
            var paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            cmd.Execute(paths);
        }
    }
}
