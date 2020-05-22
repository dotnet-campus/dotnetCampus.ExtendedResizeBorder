using System;
using System.Windows;
using System.Windows.Markup;

namespace ExtendedResizeBorder
{
    public class ExtendedResizeExtension : MarkupExtension
    {
        public Thickness Radius { get; set; } = new Thickness(8);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new ExtendedResizeBorder
            {
                Radius = Radius,
            };
        }
    }
}