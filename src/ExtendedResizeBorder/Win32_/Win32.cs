using System;
using System.Runtime.InteropServices;

namespace ExtendedResizeBorder
{
    public static class Win32
    {
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);


        [DllImport("user32")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);


        [DllImport("user32")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        public enum GetWindowLongFields
        {
            GWL_EXSTYLE = -20,
        }

        [Flags]
        public enum ExtendedWindowStyles : long
        {
            WS_EX_TOOLWINDOW = 0x00000080L,
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct MinMaxInfo
        {
            Point Reserved;

            /// <summary>
            ///     The maximized width (x member) and the maximized height (y member) of the window. For top-level windows, this value
            ///     is based on the width of the primary monitor.
            /// </summary>
            public Point MaxSize;

            /// <summary>
            ///     The position of the left side of the maximized window (x member) and the position of the top of the maximized
            ///     window (y member). For top-level windows, this value is based on the position of the primary monitor.
            /// </summary>
            public Point MaxPosition;

            /// <summary>
            ///     The minimum tracking width (x member) and the minimum tracking height (y member) of the window. This value can be
            ///     obtained programmatically from the system metrics SM_CXMINTRACK and SM_CYMINTRACK (see the GetSystemMetrics
            ///     function).
            /// </summary>
            public Point MinTrackSize;

            /// <summary>
            ///     The maximum tracking width (x member) and the maximum tracking height (y member) of the window. This value is based
            ///     on the size of the virtual screen and can be obtained programmatically from the system metrics SM_CXMAXTRACK and
            ///     SM_CYMAXTRACK (see the GetSystemMetrics function).
            /// </summary>
            public Point MaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point : IEquatable<Point>
        {
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public bool Equals(Point other)
            {
                return (X == other.X) && (Y == other.Y);
            }

            public override bool Equals(object obj)
            {
                return obj is Point && Equals((Point) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((int) X * 397) ^ (int) Y;
                }
            }

            public int X, Y;

            public static bool operator ==(Point left, Point right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Point left, Point right)
            {
                return !(left == right);
            }
        }
    }
}