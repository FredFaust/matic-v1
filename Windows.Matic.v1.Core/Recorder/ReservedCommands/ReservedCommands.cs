using System.Collections.Generic;
using System.Windows.Forms;

namespace Windows.Matic.v1.Core.Recorder.ReservedCommands
{
    public static class MaticReservedCommands
    {
        public static Dictionary<CommandNames, ReservedCommand> GetCommands()
        {
            Dictionary<CommandNames, ReservedCommand> maticReservedCommands = new Dictionary<CommandNames, ReservedCommand>();

            // TODO : Load from config

            maticReservedCommands.Add(CommandNames.Pause, new ReservedCommand(Keys.P, ModifierKeys.Ctrl, ModifierKeys.Alt));
            maticReservedCommands.Add(CommandNames.Resume, new ReservedCommand(Keys.R, ModifierKeys.Ctrl, ModifierKeys.Alt));
            maticReservedCommands.Add(CommandNames.Finish, new ReservedCommand(Keys.F, ModifierKeys.Ctrl, ModifierKeys.Alt));

            return maticReservedCommands;
        }
    }

    public class ReservedCommand
    {
        private Keys _finalKey;
        private List<ModifierKeys> _keyModifiers;

        public ReservedCommand(Keys finalKey, params ModifierKeys[] keyModifiers)
        {
            _finalKey = finalKey;
            _keyModifiers = new List<ModifierKeys>();
            foreach (ModifierKeys mk in keyModifiers)
            {
                _keyModifiers.Add(mk);
            }
        }

        public Keys FinalKey
        {
            get { return _finalKey; }
        }

        public List<ModifierKeys> KeyModifiers
        {
            get { return _keyModifiers; }
        }
    }

    public enum CommandNames
    {
        None,
        Pause,
        Resume,
        Finish
    }

    public enum ModifierKeys
    {
        Alt,
        Ctrl,
        Shift,
        Win
    }
}
