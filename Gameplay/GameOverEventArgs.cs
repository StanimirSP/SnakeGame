using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gameplay
{
    public class GameOverEventArgs: EventArgs
    {
        public uint Score { get; }
        public string Reason { get; }
        public GameOverEventArgs(uint score, string reason)
        {
            Score = score;
            Reason = reason;
        }
    }
}
