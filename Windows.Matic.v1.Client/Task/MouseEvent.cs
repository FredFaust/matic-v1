using System;
using System.Runtime.InteropServices;

namespace Windows.Matic.v1.Task
{
    public class MouseEvent : InputEvent
    {
        private MouseEventType _eventType;
        private MouseHookEventStruct _eventData;

        public MouseEvent(MouseHookEventStruct eventData, MouseEventType eventType, int delay)
        {
            _delay = delay;
            _eventType = EventType;
            _eventData = eventData;
        }

        public MouseHookEventStruct EventData
        {
            get { return _eventData; }
        }

        public MouseEventType EventType
        {
            get { return _eventType; }
        }

        public override string ToString()
        {
            return "MouseEvent -  TODO "; // + _mouseData + " " + _eventType;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MouseHookEventStruct
    {
        public Point pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int x;
        public int y;
    }

    public enum MouseEventType
    {
        None,
        Down,
        Up
    }
}
