using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Windows.Matic.v1.Task
{
    public class InputCommand
    {
        private List<Keys> _keyset;

        public InputCommand(List<Keys> keyset)
        {
            _keyset = keyset;
        }

        public void AddKey(Keys k)
        {
            _keyset.Add(k);
        }

        public List<Keys> Keyset
        {
            get { return _keyset; }
        }

        /*public static bool operator ==(InputCommand lhs, InputCommand rhs)
        {
            return IdenticalKeys(lhs, rhs);
        }

        public static bool operator !=(InputCommand lhs, InputCommand rhs)
        {
            return !IdenticalKeys(lhs, rhs);
        }

        private static bool IdenticalKeys(InputCommand lhs, InputCommand rhs)
        {
            return ContainsAllSameItems(lhs.Keyset, rhs.Keyset);
        }

        private static bool ContainsAllSameItems(List<Keys> a, List<Keys> b)
        {
            return !b.Except(a).Any();
        }*/
    }
}
