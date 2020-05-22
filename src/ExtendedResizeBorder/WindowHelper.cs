using System;
using System.Windows;

namespace ExtendedResizeBorder
{
    internal static class WindowHelper
    {
        public static Stub OnInitialized(this Window window, Action func)
        {
            var stub = new Stub();
            if (window.IsLoaded)
            {
                func();
            }
            else
            {
                var refer = new EventHandler[] {null};
                refer[0] = (sender, e) =>
                {
                    window.ContentRendered -= refer[0];
                    if (stub.IsEnabled)
                    {
                        func();
                    }
                };
                window.ContentRendered += refer[0];
            }

            return stub;
        }
    }

    internal class Stub
    {
        public bool IsEnabled { get; set; } = true;
    }
}