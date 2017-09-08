using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Windows.Matic.v1.Recorder.Logger
{
    public sealed class InputListener
    {
        #region Singleton Mechanism
        static class Nested
        {
            static Nested() { }

            internal static readonly InputListener instance = new InputListener();
        }

        public static InputListener Instance
        {
            get { return Nested.instance; }
        }
        #endregion

        private bool _isListening = false;

        private IntPtr _user32ModuleHandle = IntPtr.Zero;
        private IntPtr _keyboardHookHandle = IntPtr.Zero;
        private KeyboardHookEventProc _keyboardHookProc;

        public InputListener()
        {
            _keyboardHookProc = new KeyboardHookEventProc();
        }

        public void StartListening()
        {
            if (!_isListening)
            {
                hook();
                _isListening = true;
            }
        }

        public void StopListening()
        {
            if (_isListening)
            {
                unhook();
                _isListening = false;
            }
        }

        public void hook()
        {
            _user32ModuleHandle = LoadLibrary("User32");
            _keyboardHookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, _keyboardHookProc, _user32ModuleHandle, 0);
        }

        public void unhook()
        {
            UnhookWindowsHookEx(_keyboardHookHandle);
        }

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookEventProc callback, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHookEventStruct lParam);
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        public delegate int KeyboardHookEventProc(int code, int wParam, ref KeyboardHookEventStruct lParam);

        public struct KeyboardHookEventStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;
    }
}
