using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Windows.Matic.v1.Recorder.Logger
{
    public class KeysetReadyEventArgs : EventArgs
    {
        private List<Keys> _keyset;

        public KeysetReadyEventArgs(List<Keys> keyset)
        {
            _keyset = keyset;
        }
        
        public List<Keys> Keyset
        {
            get { return _keyset; }
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
        private KeyEventType _lastKeyEventType = KeyEventType.None;
        private bool _loggingSessionInProgress = false;
        private List<Keys> _keysBuffer = new List<Keys>();
        private static string LoggerInitializedDateTime = DateTime.Now.ToString("yyyyMMddTHHmmss");

        public event EventHandler<KeysetReadyEventArgs> RaiseKeysetReady;

        public KeyListener()
        {
            _keyboardHookProc = new keyboardHookProc(hookProc);
        }

        ~KeyListener()
        {
            StopLogging();
        }

        public void ResetKeysetReadyIncovationList()
        {
            RaiseKeysetReady = null;
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
            //_keyboardHookProc = new keyboardHookProc(hookProc);
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
                Keys key = (Keys)lParam.vkCode;
                //if (HookedKeys.Contains(key))

                KeyEventArgs kea = new KeyEventArgs(key);
                if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    AddKeyToBuffer(key);
                    _lastKeyEventType = KeyEventType.Down;
                }
                else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    if (_lastKeyEventType == KeyEventType.Down)
                    {
                        List<Keys> keyset = new List<Keys>(_keysBuffer);
                        RemoveKeyFromBuffer(key);
                        _lastKeyEventType = KeyEventType.Up;
                        OnRaiseKeysetReady(keyset);
                    }
                    else
                    {
                        RemoveKeyFromBuffer(key);
                        _lastKeyEventType = KeyEventType.Up;
                    }
                }
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

        private void AddKeyToBuffer(Keys k)
        {
            if (!_keysBuffer.Contains(k))
            {
                _keysBuffer.Add(k);
            }
        }

        private void RemoveKeyFromBuffer(Keys k)
        {
            _keysBuffer.Remove(k);
        }

        private void OnRaiseKeysetReady(List<Keys> keyset)
        {
            EventHandler<KeysetReadyEventArgs> handler = RaiseKeysetReady;

            if (handler != null)
            {
                KeysetReadyEventArgs ea = new KeysetReadyEventArgs(keyset);
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

        public enum KeyEventType
        {
            None,
            Down,
            Up
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
