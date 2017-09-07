using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Windows.Matic.v1.Task;

namespace Windows.Matic.v1.Recorder.Logger
{
    public class InputEventReadyEventArgs : EventArgs
    {
        private InputEvent _inputEvent;

        public InputEventReadyEventArgs(InputEvent ie)
        {
            _inputEvent = ie;
        }
        
        public InputEvent InputEvent
        {
            get { return _inputEvent; }
        }
    }

    sealed class KeyListener
    {
        #region Singleton Mechanism
        static class Nested
        {
            static Nested() { }

            internal static readonly KeyListener instance = new KeyListener();
        }

        public static KeyListener Instance
        {
            get { return Nested.instance; }
        }
        #endregion

        const bool PLAY_NICE = false;
        private bool _eventsIceBroken = false;
        private DateTime _lastEventDateTime;
        private bool _loggingSessionInProgress = false;
        private List<InputEvent> _eventBuffer = new List<InputEvent>();

        public event EventHandler<InputEventReadyEventArgs> RaiseInputEventReady;

        public KeyListener()
        {
            _keyboardHookProc = new keyboardHookProc(hookProc);
        }

        ~KeyListener()
        {
            StopLogging();
        }

        public void ResetInputEventReadyIncovationList()
        {
            RaiseInputEventReady = null;
        }

        public void StartLogging()
        {
            if (!_loggingSessionInProgress)
            {
                hook();
                _loggingSessionInProgress = true;
            }
        }

        public void StopLogging()
        {
            if (_loggingSessionInProgress)
            {
                unhook();
                _loggingSessionInProgress = false;
            }
        }

        public void hook()
        {
            hInstance = LoadLibrary("User32");
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _keyboardHookProc, hInstance, 0);
        }

        public void unhook()
        {
            UnhookWindowsHookEx(hhook);
        }

        public int hookProc(int code, int wParam, ref keyboardHookStruct lParam)
        {
            if (code >= 0)
            {
                int delaySinceLastEvent = 0;
                if (_eventsIceBroken)
                {
                    delaySinceLastEvent = (int)(DateTime.Now - _lastEventDateTime).TotalMilliseconds;
                    _lastEventDateTime = DateTime.Now;
                }
                else
                {
                    _lastEventDateTime = DateTime.Now;
                }

                _eventsIceBroken = true;
                Keys key = (Keys)lParam.vkCode;
                KeyEventArgs kea = new KeyEventArgs(key);
                if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    OnRaiseInputEventReady(new InputEvent(key, KeyEventType.Down, delaySinceLastEvent));
                }
                else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    OnRaiseInputEventReady(new InputEvent(key, KeyEventType.Up, delaySinceLastEvent));
                }

                // TODO : Might be worth it to investigate this
                if (kea.Handled)
                {
                    return 1;
                }
            }

            if (PLAY_NICE)
            {
                return CallNextHookEx(hhook, code, wParam, ref lParam);
            }
            else
            {
                // we should not play nice if there are mean hooks on our system that unhook us
                return 0;
            }
        }

        private void OnRaiseInputEventReady(InputEvent ie)
        {
            EventHandler<InputEventReadyEventArgs> handler = RaiseInputEventReady;

            if (handler != null)
            {
                InputEventReadyEventArgs ea = new InputEventReadyEventArgs(ie);
                handler(this, ea);
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref keyboardHookStruct lParam);
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        public struct keyboardHookStruct
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

        private IntPtr hInstance;
        IntPtr hhook = IntPtr.Zero;
        private keyboardHookProc _keyboardHookProc;
        public delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct lParam);
    }
}
