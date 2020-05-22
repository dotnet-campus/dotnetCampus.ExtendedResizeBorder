using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;

namespace ExtendedResizeBorder
{
    
    internal class ExtendedResizeWindow : Window
    {
        public ExtendedResizeWindow(Direction dir)
        {
            Direction = dir;
            SetStyle(dir);

            Title = "Felix Shadow Window";
            Background = Brushes.Transparent;
            ShowInTaskbar = false;
            ShowActivated = false;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;
            WindowChrome.SetWindowChrome(this, new WindowChrome()
            {
                GlassFrameThickness = new Thickness(-1),
                CaptionHeight = 0
            });
            SnapsToDevicePixels = true;

            GlowBorder = new Border()
            {
                SnapsToDevicePixels = true,
                IsHitTestVisible = false,
                Background = Brushes.Black,
            };


            Content = GlowBorder;
        }


        public Direction Direction { get; set; }

        public Border GlowBorder { get; }

        public void Render(Rect bounds, ExtendedResizeBorder extendedResizeBorder)
        {
            Opacity = 1.0 / 255;

            var shrink = Normalize(new Thickness(1));
            var edge = extendedResizeBorder.Radius;
            var inner = new Rect(new Point(bounds.Left + shrink.Left, bounds.Top + shrink.Top),
                new Point(bounds.Right - shrink.Right, bounds.Bottom - shrink.Bottom));
            var outer = new Rect(new Point(bounds.Left - edge.Left, bounds.Top - edge.Top),
                new Point(bounds.Right + edge.Right, bounds.Bottom + edge.Bottom));

            inner = Normalize(inner);
            outer = Normalize(outer);
            _top = edge.Top;
            _bottom = _top + bounds.Height;
            switch (Direction)
            {
                case Direction.Left:
                    UpdateBounds(new Rect(new Point(outer.Left, outer.Top), new Point(inner.Left, outer.Bottom)));
                    GlowBorder.HorizontalAlignment = HorizontalAlignment.Left;
                    GlowBorder.Width = edge.Left + shrink.Left;
                    break;
                case Direction.Right:
                    UpdateBounds(new Rect(new Point(inner.Right, outer.Top), new Point(outer.Right, outer.Bottom)));
                    GlowBorder.HorizontalAlignment = HorizontalAlignment.Right;
                    GlowBorder.Width = edge.Right + edge.Right;
                    break;
                case Direction.Top:
                    UpdateBounds(new Rect(new Point(inner.Left, outer.Top), inner.TopRight));
                    GlowBorder.VerticalAlignment = VerticalAlignment.Top;
                    GlowBorder.Height = edge.Top + edge.Top;
                    break;
                case Direction.Bottom:
                    UpdateBounds(new Rect(inner.BottomLeft, new Point(inner.Right, outer.Bottom)));
                    GlowBorder.VerticalAlignment = VerticalAlignment.Bottom;
                    GlowBorder.Height = edge.Bottom + edge.Bottom;
                    break;
            }
        }

        #region 私有变量

        private HitTestValues _hitTestValue;
        private Cursor _cursor;
        private double _top;
        private double _bottom;

        #endregion

        #region 辅助方法

        private void SetStyle(Direction direction)
        {
            var (hitTestValues, cursor) = BuildStyle(direction);
            _hitTestValue = hitTestValues;
            _cursor = cursor;
        }

        private (HitTestValues, Cursor) BuildStyle(Direction direction)
        {
            var hitTestValue = HitTestValues.HTLEFT;
            var cursor = Cursors.AppStarting;
            switch (direction)
            {
                case Direction.Left:
                    hitTestValue = HitTestValues.HTLEFT;
                    cursor = Cursors.SizeWE;
                    break;

                case Direction.Top:
                    hitTestValue = HitTestValues.HTTOP;
                    cursor = Cursors.SizeNS;
                    break;

                case Direction.Right:
                    hitTestValue = HitTestValues.HTRIGHT;
                    cursor = Cursors.SizeWE;
                    break;

                case Direction.Bottom:
                    hitTestValue = HitTestValues.HTBOTTOM;
                    cursor = Cursors.SizeNS;
                    break;

                case Direction.TopLeft:
                    hitTestValue = HitTestValues.HTTOPLEFT;
                    cursor = Cursors.SizeNWSE;
                    break;

                case Direction.TopRight:
                    hitTestValue = HitTestValues.HTTOPRIGHT;
                    cursor = Cursors.SizeNESW;
                    break;

                case Direction.BottomLeft:
                    hitTestValue = HitTestValues.HTBOTTOMLEFT;
                    cursor = Cursors.SizeNESW;
                    break;

                case Direction.BottomRight:
                    hitTestValue = HitTestValues.HTBOTTOMRIGHT;
                    cursor = Cursors.SizeNWSE;
                    break;
            }

            return (hitTestValue, cursor);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var handle = new WindowInteropHelper(this).Handle;
            var hwndSource = HwndSource.FromHwnd(handle);
            hwndSource?.AddHook(WndProc);

            var style = Win32.GetWindowLong(handle, (int)Win32.GetWindowLongFields.GWL_EXSTYLE);

            // 使窗口在 Alt+Tab 中不可见的两种方式：1. WS_EX_TOOLWINDOW；2. Visibility=Collapsed。
            var extendedStyle = Win32.ExtendedWindowStyles.WS_EX_TOOLWINDOW;


            Win32.SetWindowLong(handle, (int) Win32.GetWindowLongFields.GWL_EXSTYLE, style | (int) extendedStyle);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((WM) msg)
            {
                case WM.MOUSEACTIVATE:
                {
                    handled = true;
                    return new IntPtr(3);
                }

                case WM.LBUTTONDOWN:
                {
                    if (!IsHitTestVisible)
                        break;

                    Win32.PostMessage(new WindowInteropHelper(Owner).Handle,
                        (int) WM.NCLBUTTONDOWN, (int) _hitTestValue,
                        0);
                }
                    break;

                case WM.GETMINMAXINFO:
                {
                    var obj =
                        (Win32.MinMaxInfo) Marshal.PtrToStructure(lParam, typeof(Win32.MinMaxInfo));

                    if (obj.MaxSize.X > 0)
                    {
                        obj.MaxSize.X = obj.MaxSize.Y =
                            obj.MaxTrackSize.X = obj.MaxTrackSize.Y = int.MaxValue;
                        Marshal.StructureToPtr(obj, lParam, true);
                    }
                }
                    break;

                case WM.NCHITTEST:
                {
                    var cursor = _cursor;

                    if (cursor != null && cursor != Cursor)
                        Cursor = cursor;
                }
                    break;

                case WM.MOUSEMOVE:
                {
                    if (Direction == Direction.Right || Direction == Direction.Left)
                    {
                        var y = lParam.ToInt32() >> 16;
                        UpdateCourse(y);
                    }
                }
                    break;
            }

            return IntPtr.Zero;
        }

        private void UpdateCourse(int i)
        {
            if (i < _top)
            {
                if (Direction == Direction.Right)
                {
                    SetStyle(Direction.TopRight);
                }
                else if (Direction == Direction.Left)
                {
                    SetStyle(Direction.TopLeft);
                }
            }
            else if (i > _bottom)
            {
                if (Direction == Direction.Right)
                {
                    SetStyle(Direction.BottomRight);
                }
                else if (Direction == Direction.Left)
                {
                    SetStyle(Direction.BottomLeft);
                }
            }
            else
            {
                if (Direction == Direction.Right)
                {
                    SetStyle(Direction.Right);
                }
                else if (Direction == Direction.Left)
                {
                    SetStyle(Direction.Left);
                }
            }
        }

        #endregion


        private void UpdateBounds(Rect bounds)
        {
            Left = bounds.Left;
            Top = bounds.Top;
            Width = bounds.Width;
            Height = bounds.Height;
        }

        private Rect Normalize(Rect rect)
        {
            return new Rect(new Point(Math.Round(rect.Left), Math.Round(rect.Top)),
                new Point(Math.Round(rect.Right), Math.Round(rect.Bottom)));
        }

        private Thickness Normalize(Thickness t)
        {
            return new Thickness(Math.Round(t.Left), Math.Round(t.Top), Math.Round(t.Right), Math.Round(t.Bottom));
        }
    }
}