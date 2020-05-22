using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace ExtendedResizeBorder
{
    internal class ExtendedResizeController
    {
       
        public ExtendedResizeController(Window window)
        {
            _window = window;

            var shadowBorder = ExtendedResizeBorder.GetExtendedResizeBorder(_window);
            if (shadowBorder == null)
                return;

            var left = new ExtendedResizeWindow(Direction.Left);
            var top = new ExtendedResizeWindow(Direction.Top);
            var right = new ExtendedResizeWindow(Direction.Right);
            var bottom = new ExtendedResizeWindow(Direction.Bottom);

            _borders = new List<ExtendedResizeWindow> {left, top, right, bottom,};

            _windowInitStub = window.OnInitialized(Initialize);
        }

        #region 公有方法 & 属性

        public void Close()
        {
            RemoveEventListeners();

            _borders.ForEach(x =>
            {
                x.Owner = null;
                x.Close();
            });
        }

        #endregion

        #region 私有字段

        private readonly List<ExtendedResizeWindow> _borders;

        private readonly Window _window;
        private readonly Stub _windowInitStub;


        internal static readonly DependencyProperty ExtendedResizeControllerProperty = DependencyProperty.RegisterAttached(
            "ExtendedResizeController", typeof(ExtendedResizeController), typeof(ExtendedResizeController),
            new PropertyMetadata(default(ExtendedResizeController)));

        #endregion

        #region 事件处理

        private void WindowOnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool) e.NewValue)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (_window.WindowState != WindowState.Normal) return;
            UpdateBounds();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateBounds();
        }


        private async void Window_StateChanged(object sender, EventArgs e)
        {
            if (_window.WindowState == WindowState.Normal)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200));

                UpdateBounds();
                Show();
            }
            else
            {
                Hide();
            }
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region 辅助方法

        private void Initialize()
        {
            AddEventListeners();

            _borders.ForEach(x =>
            {
                if (_window?.IsLoaded != true) return;
                x.Owner = _window;
            });

            if (_window.ResizeMode == ResizeMode.CanMinimize || _window.ResizeMode == ResizeMode.NoResize)
            {
                _borders.ForEach(x => { x.IsHitTestVisible = false; });
            }

            if (_window.IsVisible && _window.WindowState == WindowState.Normal)
            {
                UpdateBounds();
                Show();
            }
        }

        private void UpdateBounds()
        {
            if (_window.WindowState != WindowState.Normal) return;

            var resizeBorder = ExtendedResizeBorder.GetExtendedResizeBorder(_window);
            if (resizeBorder == null) return;

            var bounds = new Rect(_window.Left, _window.Top, _window.ActualWidth, _window.ActualHeight);
            _borders.ForEach(x => x.Render(bounds, resizeBorder));
        }

        private void AddEventListeners()
        {
            _window.LocationChanged += Window_LocationChanged;
            _window.SizeChanged += Window_SizeChanged;
            _window.StateChanged += Window_StateChanged;
            _window.IsVisibleChanged += WindowOnIsVisibleChanged;
            _window.Closed += Window_Closed;
        }

        private void RemoveEventListeners()
        {
            _windowInitStub.IsEnabled = false;
            _window.StateChanged -= Window_StateChanged;
            _window.IsVisibleChanged -= WindowOnIsVisibleChanged;
            _window.Closed -= Window_Closed;
            _window.LocationChanged -= Window_LocationChanged;
            _window.SizeChanged -= Window_SizeChanged;
        }

        private void Hide()
        {
            _borders.ForEach(x =>
            {
                x.Hide();
                x.Owner = null;
            });
        }

        private void Show()
        {
            if (_window?.IsLoaded != true) return;
            try
            {
                if (_window.WindowState == WindowState.Normal)
                {
                    _borders.ForEach(x =>
                    {
                        if (_window?.IsLoaded != true) return;
                        x.Show();
                        x.Owner = _window;
                    });
                }
            }
            catch
            {
                // todo 
            }
        }

        #endregion
    }
}