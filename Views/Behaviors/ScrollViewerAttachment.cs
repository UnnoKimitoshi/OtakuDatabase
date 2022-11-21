using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Otaku_Database.Views.Behaviors
{
    public enum ScrollViewerWheelMode
    {
        Normal,
        OnlyVisible,
        Auto,
    }

    public static class ScrollViewerAttachment
    {
        static private PropertyInfo handlesMouseWheelScrollingPropInfo;

        public static ScrollViewerWheelMode GetScrollViewerWheelMode(DependencyObject obj)
        {
            return (ScrollViewerWheelMode)obj.GetValue(ScrollViewerWheelModeProperty);
        }

        public static void SetScrollViewerWheelMode(DependencyObject obj, ScrollViewerWheelMode value)
        {
            obj.SetValue(ScrollViewerWheelModeProperty, value);
        }

        // Using a DependencyProperty as the backing store for ScrollViewerWheelMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollViewerWheelModeProperty =
            DependencyProperty.RegisterAttached(nameof(ScrollViewerWheelMode),
                typeof(ScrollViewerWheelMode),
                typeof(ScrollViewerAttachment),
                new PropertyMetadata(ScrollViewerWheelMode.Normal,
                    (d, e) =>
                    {
                        if (!(e.NewValue is ScrollViewerWheelMode mode)) { return; }
                        ScrollViewer sv;
                        if (d is ScrollViewer dsv)
                        {
                            sv = dsv;
                            DispatchEvent(sv, mode);
                        }
                        else
                        {
                            d.Dispatcher?.BeginInvoke((Action)(async () =>
                            {
                            // VisualTreeの子要素からScrollViewerを探す
                                await Task.Delay(10); // VisualTree構築待ち
                                sv = d.GetChildOfType<ScrollViewer>();
                                if (sv == null) { return; }
                                SetScrollViewerWheelMode(sv, mode);
                            }));
                        }
                    }));

        private static void DispatchEvent(ScrollViewer sv, ScrollViewerWheelMode mode)
        {
            if (mode == ScrollViewerWheelMode.Normal)
            {
                handlesMouseWheelScrollingPropInfo?.SetValue(sv, true);
                sv.PreviewMouseWheel -= OnPreviewMouseWheel;
            }
            else
            {
                sv.PreviewMouseWheel += OnPreviewMouseWheel;
            }
        }

        private static void OnPreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (!(sender is ScrollViewer sv))
            {
                return;
            }

            if (handlesMouseWheelScrollingPropInfo == null)
            {
                // リフレクションでinternalなプロパティを取得
                var svType = typeof(ScrollViewer);
                handlesMouseWheelScrollingPropInfo = svType.GetProperty("HandlesMouseWheelScrolling", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            var mode = GetScrollViewerWheelMode(sv);
            if (mode != ScrollViewerWheelMode.Normal)
            {
                // 縦スクロールバーが見えている時のみ、
                // マウスホイールイベントをハンドリングする
                handlesMouseWheelScrollingPropInfo.SetValue(sv, sv.ComputedVerticalScrollBarVisibility == Visibility.Visible);
            }

            if (mode == ScrollViewerWheelMode.Auto)
            {
                if (e.Delta > 0)
                {
                    handlesMouseWheelScrollingPropInfo.SetValue(sv, sv.VerticalOffset > 0);
                }
                else
                {
                    handlesMouseWheelScrollingPropInfo.SetValue(sv, sv.VerticalOffset < sv.ScrollableHeight);
                }
            }
        }
        public static T GetChildOfType<T>(this DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}
