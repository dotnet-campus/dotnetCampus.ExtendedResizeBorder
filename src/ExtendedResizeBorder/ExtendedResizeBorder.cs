using System.Windows;

namespace ExtendedResizeBorder
{
    public class ExtendedResizeBorder : DependencyObject
    {
        #region 依赖项属性

        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
            "Radius", typeof(Thickness), typeof(ExtendedResizeBorder),
            new PropertyMetadata(default(Thickness)));

        #endregion

        #region 公有成员

        public Thickness Radius
        {
            get { return (Thickness) GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }


        internal Window Owner { get; set; }

        #endregion


        #region ExtendedResizeBorder 附加属性

        public static readonly DependencyProperty ExtendedResizeBorderProperty = DependencyProperty.RegisterAttached(
            "ExtendedResizeBorder", typeof(ExtendedResizeBorder), typeof(ExtendedResizeBorder),
            new PropertyMetadata(default(ExtendedResizeBorder), ExtendedResizeBorderPropertyChangedCallback));

        private static void ExtendedResizeBorderPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var window = dependencyObject as Window;
            if (window == null) return;

            if (e.OldValue == null && e.NewValue == null)
            {
                return;
            }

            if (e.OldValue != null)
            {
                var shadowController =
                    (ExtendedResizeController) window.GetValue(ExtendedResizeController.ExtendedResizeControllerProperty);
                shadowController.Close();
                ((ExtendedResizeBorder) e.OldValue).Owner = null;
                window.SetValue(ExtendedResizeController.ExtendedResizeControllerProperty, null);
            }

            if (e.NewValue != null)
            {
                ((ExtendedResizeBorder) e.NewValue).Owner = window;
                var shadowController = new ExtendedResizeController(window);
                window.SetValue(ExtendedResizeController.ExtendedResizeControllerProperty, shadowController);
            }
        }

        public static void SetExtendedResizeBorder(Window element, ExtendedResizeBorder value)
        {
            element.SetValue(ExtendedResizeBorderProperty, value);
        }

        public static ExtendedResizeBorder GetExtendedResizeBorder(Window element)
        {
            return (ExtendedResizeBorder) element.GetValue(ExtendedResizeBorderProperty);
        }

        #endregion
    }
}