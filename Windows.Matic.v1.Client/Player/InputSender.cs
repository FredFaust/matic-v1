using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Windows.Matic.v1.Common;

namespace Windows.Matic.v1.Player
{
    sealed class InputSender
    {
        #region Singleton Mechanism
        static class Nested
        {
            static Nested() { }

            internal static readonly InputSender instance = new InputSender();
        }

        public static InputSender Instance
        {
            get { return Nested.instance; }
        }
        #endregion

        public InputSender()
        {
            //Nothing for now
        }

        ~InputSender()
        {
            //Nothing for now
        }

        public void SendKeyDownEvent(Keys keyCode)
        {
            OSInput input = GetInputToSend(keyCode, (uint)KeyEventFlags.KeyDown);
            if (SendInput(1, ref input, Marshal.SizeOf(typeof(OSInput))) == 0)
                throw new Exception();
        }

        public void SendKeyUpEvent(Keys keyCode)
        {
            OSInput input = GetInputToSend(keyCode, (uint)KeyEventFlags.KeyUp);
            if (SendInput(1, ref input, Marshal.SizeOf(typeof(OSInput))) == 0)
                throw new Exception();
        }

        public void SendMouseEvent(int x, int y, int mouseMessage)
        {
            OSInput input = new OSInput();
            input.Type = (uint)InputType.Mouse;
            input.Data.Mouse = new MouseInput()
            {
                X = User32Utils.CalculateAbsoluteCoordinateX(x),
                Y = User32Utils.CalculateAbsoluteCoordinateY(y),
                MouseData = 0,
                Flags = (uint)MouseEventFlags.MOUSEEVENTF_MOVE | (uint)MouseEventFlags.MOUSEEVENTF_ABSOLUTE,
                Time = 0,
                ExtraInfo = IntPtr.Zero
            };

            if (SendInput(1, ref input, Marshal.SizeOf(typeof(OSInput))) == 0)
                throw new Exception();

            input.Data.Mouse.Flags = User32Utils.GetEventFlagFromMessage(mouseMessage);
            if (SendInput(1, ref input, Marshal.SizeOf(typeof(OSInput))) == 0)
                throw new Exception();
        }

        private OSInput GetInputToSend(Keys keyCode, uint eventFlags)
        {
            OSInput input = new OSInput
            {
                Type = (uint)InputType.Keyboard
            };
            input.Data.Keyboard = new KeyboardInput()
            {
                Vk = (ushort)keyCode,
                Scan = 0,
                Flags = eventFlags,
                Time = 0,
                ExtraInfo = IntPtr.Zero,
            };

            return input;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, ref OSInput input, int cbSize);
        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();
    }
}
