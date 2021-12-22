namespace NetLib.Define
{
    public enum SERVER_TYPE
    {
        NONE = 0,

        LOBBY,
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
