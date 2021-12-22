using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLib.Define
{
    public enum SERVER_TYPE
    {
        NONE = 0,

        LOBBY,
        GAME,
        SYNC,
    }

    public enum SERVER_STATE
    {
        NONE = 0,

        INIT,
        RUNNING,
        STOP,
        CLOSE,

        MAX,
    }
}
