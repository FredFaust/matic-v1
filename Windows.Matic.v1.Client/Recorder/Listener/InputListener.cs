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

        private IntPtr _mouseHookHandle = IntPtr.Zero;
        private IntPtr _keyboardHookHandle = IntPtr.Zero;
        private IntPtr _user32ModuleHandle = IntPtr.Zero;

        public InputListener() { }

        public void StartListening(KeyboardHookEventProc keyboardCallback, MouseHookEventProc mouseCallback)
        {
            if (!_isListening)
            {
                SetWindowsHooks(keyboardCallback, mouseCallback);
                _isListening = true;
            }
        }

        public void StopListening()
        {
            if (_isListening)
            {
                UnsetWindowsHooks();
                _isListening = false;
            }
        }

        public void SetWindowsHooks(KeyboardHookEventProc keyboardCallback, MouseHookEventProc mouseCallback)
        {
            _user32ModuleHandle = LoadLibrary("User32");
            _mouseHookHandle = SetWindowsHookEx(WH_MOUSE_LL, mouseCallback, _user32ModuleHandle, 0);
            _keyboardHookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, keyboardCallback, _user32ModuleHandle, 0);
        }

        public void UnsetWindowsHooks()
        {
            UnhookWindowsHookEx(_mouseHookHandle);
            UnhookWindowsHookEx(_keyboardHookHandle);
        }

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, MouseHookEventProc callback, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookEventProc callback, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHookEventStruct lParam);
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        public delegate int MouseHookEventProc(int code, int wParam, ref MouseHookEventStruct lParam);
        public delegate int KeyboardHookEventProc(int code, int wParam, ref KeyboardHookEventStruct lParam);

        public struct MouseHookEventStruct
        {
            public Point pt;
            public int mouseData;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        public struct KeyboardHookEventStruct
        {
            public int vkCode;
            public uint scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        public struct Point
        {
            public int x;
            public int y;
        }

        const int WH_MOUSE_LL = 14;
        const int WH_KEYBOARD_LL = 13;
    }
}
