using System.Collections.Generic;
using System.Windows.Forms;

namespace Windows.Matic.v1.Recorder.ReservedCommands
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
                    return k == Keys.LMenu || k == Keys.LMenu;
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
    }
}
