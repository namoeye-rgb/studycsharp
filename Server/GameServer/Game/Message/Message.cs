
using EntityService;

namespace GameCommon.Contracts.Message
{
    public class CompAdded : MsgBody
    {
        public MessageTarget Owner;

    }
    public class CompRemoved : MsgBody { }

    public class EnteringMap : MsgBody { }
    public class EnteredMap : MsgBody { }
    public class LeftingMap : MsgBody { }
    public class LeftedMap : MsgBody { }
    public class Collided : MsgBody { }

    public class MSG_FO_Created : MsgBody { }
    public class MSG_FO_Initialized : MsgBody { }

}
