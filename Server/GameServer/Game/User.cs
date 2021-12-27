using GameServer.Net.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public partial class User
    {
        public ClientSession Session { get; private set; }
        public ulong userUID { get; }
        public FieldObject FieldObject { get; }

        public User(ClientSession _session)
        {

        }
    }
}
