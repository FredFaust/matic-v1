using System.Runtime.InteropServices;

namespace Windows.Matic.v1.Core.Common
{
    public static class User32Utils
    {
        public static uint GetEventFlagFromMessage(int message)
        {
            uint eventFlag = 0;
            switch(message)
            {
                case MouseMessages.WM_LBUTTONDOWN:
                    eventFlag = (uint)MouseEventFlags.MOUSEEVENTF_LEFTDOWN;
                    break;
                case MouseMessages.WM_LBUTTONUP:
                    eventFlag = (uint)MouseEventFlags.MOUSEEVENTF_LEFTUP;
                    break;
                case MouseMessages.WM_MOUSEMOVE:
                    eventFlag = (uint)MouseEventFlags.MOUSEEVENTF_MOVE;
                    break;
                case MouseMessages.WM_MOUSEWHEEL:
                    eventFlag = (uint)MouseEventFlags.MOUSEEVENTF_WHEEL;
                    break;
                case MouseMessages.WM_RBUTTONDOWN:
                    eventFlag = (uint)MouseEventFlags.MOUSEEVENTF_RIGHTDOWN;
                    break;
                case MouseMessages.WM_RBUTTONUP:
                    eventFlag = (uint)MouseEventFlags.MOUSEEVENTF_RIGHTUP;
                    break;
            }
            return eventFlag;
        }

        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(SystemMetric smIndex);

        public static int CalculateAbsoluteCoordinateX(int x)
        {
            return (x * 65536) / GetSystemMetrics(SystemMetric.SM_CXSCREEN);
        }

        public static int CalculateAbsoluteCoordinateY(int y)
        {
            return (y * 65536) / GetSystemMetrics(SystemMetric.SM_CYSCREEN);
        }

        enum SystemMetric
        {
            SM_CXSCREEN = 0,
            SM_CYSCREEN = 1,
        }
    }
}
