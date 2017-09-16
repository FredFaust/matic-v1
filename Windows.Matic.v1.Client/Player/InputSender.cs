using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            Input input = new Input
            {
                Type = 1
            };
            input.Data.Keyboard = new KeyboardInput()
            {
                Vk = (ushort)keyCode,
                Scan = 0,
                Flags = (uint)KeyEventF.KeyDown,
                Time = 0,
                ExtraInfo = IntPtr.Zero,
            };

            Input[] inputs = new Input[] { input };
            if (SendInput(1, inputs, Marshal.SizeOf(typeof(Input))) == 0)
                throw new Exception();
        }

        public void SendKeyUpEvent(Keys keyCode)
        {
            Input input = new Input
            {
                Type = 1
            };
            input.Data.Keyboard = new KeyboardInput()
            {
                Vk = (ushort)keyCode,
                Scan = 0,
                Flags = (uint)KeyEventF.KeyUp,
                Time = 0,
                ExtraInfo = IntPtr.Zero,
            };

            Input[] inputs = new Input[] { input };
            if (SendInput(1, inputs, Marshal.SizeOf(typeof(Input))) == 0)
                throw new Exception();
        }

        public void SendMouseEvent(int x, int y, int flags, int mouseData)
        {
            Input input = new Input
            {
                Type = 1
            };
            input.Data.Mouse = new MouseInput()
            {
                X = x,
                Y = y,
                MouseData = (uint)mouseData,
                Flags = (uint)flags,
                Time = 0,
                ExtraInfo = IntPtr.Zero
            };

            Input[] inputs = new Input[] { input };
            if (SendInput(1, inputs, Marshal.SizeOf(typeof(Input))) == 0)
                throw new Exception();
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);
        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        [Flags]
        private enum InputType
        {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 2
        }

        [Flags]
        private enum KeyEventF
        {
            KeyDown = 0x0000,
            ExtendedKey = 0x0001,
            KeyUp = 0x0002,
            Unicode = 0x0004,
            Scancode = 0x0008,
        }

        /// <summary>
        /// See "SendInput {struct.name}" on msdn for more info
        /// Probably shouldn't modify the orders and types too much
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct Input
        {
            public uint Type;
            public MouseKeyboardHardwareInput Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct MouseKeyboardHardwareInput
        {
            [FieldOffset(0)]
            public HardwareInput Hardware;
            [FieldOffset(0)]
            public KeyboardInput Keyboard;
            [FieldOffset(0)]
            public MouseInput Mouse;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HardwareInput
        {
            public uint Msg;
            public ushort ParamL;
            public ushort ParamH;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KeyboardInput
        {
            public ushort Vk;
            public ushort Scan;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MouseInput
        {
            public int X;
            public int Y;
            public uint MouseData;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }
    }
}
