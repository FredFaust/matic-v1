using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Windows.Matic.v1.Core.Common;
using Windows.Matic.v1.Core.Task;

namespace Windows.Matic.v1.Core.Recorder.ReservedCommands
{
    public class ReservedCommandChecker
    {
        private Dictionary<CommandNames, ReservedCommand> _reservedCommands;

        public ReservedCommandChecker()
        {
            _reservedCommands = MaticReservedCommands.GetCommands();
        }

        public CommandNames Check(List<Keys> activeKeys)
        {
            CommandNames commandFound = CommandNames.None;
            foreach(KeyValuePair<CommandNames, ReservedCommand> kvp in _reservedCommands)
            {
                if (ActiveKeysMatchesCommand(activeKeys, kvp.Value))
                {
                    commandFound = kvp.Key;
                    break;
                }
            }

            return commandFound;
        }

        private bool ActiveKeysMatchesCommand(List<Keys> activeKeys, ReservedCommand command)
        {
            return ActiveKeysIncludesCommandModifiers(activeKeys, command.KeyModifiers)
                && activeKeys.Contains(command.FinalKey)
                && activeKeys.Count == command.KeyModifiers.Count + 1;
        }

        private bool ActiveKeysIncludesCommandModifiers(List<Keys> activeKeys, List<ModifierKeys> keyModifiers)
        {
            int matchCount = 0;
            foreach(ModifierKeys mk in keyModifiers)
            {
                foreach(Keys k in activeKeys)
                {
                    if (ActiveKeyIsKeyModifier(k, mk))
                    {
                        matchCount++;
                        break;
                    }
                }
            }
            return matchCount == keyModifiers.Count;
        }

        private bool ActiveKeyIsKeyModifier(Keys k, ModifierKeys mk)
        {
            switch(mk)
            {
                case ModifierKeys.Alt:
                    return k == Keys.LMenu || k == Keys.RMenu;
                case ModifierKeys.Ctrl:
                    return k == Keys.LControlKey || k == Keys.RControlKey;
                case ModifierKeys.Shift:
                    return k == Keys.LShiftKey || k == Keys.RShiftKey;
                case ModifierKeys.Win:
                    return k == Keys.LWin || k == Keys.RWin;
                default:
                    return false;
            }
        }

        private bool KeyboardEventMatchesReservedCommandModifier(KeyboardEvent ke, ReservedCommand rc)
        {
            bool matchFound = false;
            foreach(ModifierKeys mk in rc.KeyModifiers)
            {
                if (ActiveKeyIsKeyModifier(ke.Key, mk))
                {
                    matchFound = true;
                    break;
                }
            }
            return matchFound;
        }

        public List<InputEvent> RemoveReservedCommandEventsFromBuffer(List<InputEvent> buffer, CommandNames commandInvoked)
        {
            Keys finalKey = Keys.None;
            if (commandInvoked != CommandNames.None)
            {
                finalKey = _reservedCommands[commandInvoked].FinalKey;
            }

            if (finalKey != Keys.None)
            {
                int iLastElement = buffer.Count - 1;
                KeyboardEvent lastKeyboardEvent = buffer[iLastElement] as KeyboardEvent;

                if (lastKeyboardEvent != null)
                {
                    if (lastKeyboardEvent.Key == finalKey)
                    {
                        List<int> indexesToRemove = new List<int>();
                        indexesToRemove.Add(iLastElement);

                        bool isSuite = true;
                        for (int i = iLastElement - 1; i >= 0 && isSuite; i--)
                        {
                            KeyboardEvent indexedKeyboardEvent = buffer[i] as KeyboardEvent;
                            if (indexedKeyboardEvent != null)
                            {
                                if (KeyboardEventMatchesReservedCommandModifier(indexedKeyboardEvent, _reservedCommands[commandInvoked]))
                                {
                                    indexesToRemove.Add(i);
                                }
                                else
                                {
                                    isSuite = false;
                                }
                            }
                        }

                        foreach (int i in indexesToRemove)
                        {
                            buffer.RemoveAt(i);
                        }

                        // Repass through the buffer and make sure no keys are being hold down
                        // because of the use case where a user chains command with Ctrl and ends
                        // up with a reserved command
                        List<Keys> simulatedActiveKeys = new List<Keys>();
                        foreach (KeyboardEvent ie in buffer.OfType<KeyboardEvent>())
                        {
                            if (ie.EventFlag == KeyEventFlags.KeyDown && !simulatedActiveKeys.Contains(ie.Key))
                            {
                                simulatedActiveKeys.Add(ie.Key);
                            }
                            else if (ie.EventFlag == KeyEventFlags.KeyUp && simulatedActiveKeys.Contains(ie.Key))
                            {
                                simulatedActiveKeys.Remove(ie.Key);
                            }
                        }

                        foreach (Keys k in simulatedActiveKeys)
                        {
                            buffer.Add(new KeyboardEvent(k, KeyEventFlags.KeyUp, 50));
                        }
                    }
                }
            }
            return buffer;
        }
    }
}
