using System;
using System.Runtime.InteropServices;

namespace Windows.Matic.v1.Recorder.Listener
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

        public InputListener() { }

        public void StartListening(KeyboardHookEventProc callback)
        {
            if (!_isListening)
            {
                SetKeyboardHook(callback);
                _isListening = true;
            }
        }

        public void StopListening()
        {
            if (_isListening)
            {
                UnsetKeyboardHook();
                _isListening = false;
            }
        }

        public void SetKeyboardHook(KeyboardHookEventProc callback)
        {
            _user32ModuleHandle = LoadLibrary("User32");
            _keyboardHookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, callback, _user32ModuleHandle, 0);
        }

        public void UnsetKeyboardHook()
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
    }
}
