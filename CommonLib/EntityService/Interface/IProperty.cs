namespace EntityService
{
    public interface IPropertyRead
    {
        bool ContainsKey(string key);
        ESProperty GetProperty(string key);
        bool GetBool(string key);
        int GetInt(string key);
        float GetFloat(string key);
        string GetString(string key);
    }

    public interface IPropertyWrite
    {
        void Set(string key, ESProperty vaule);
        void Set(string key, bool vaule);
        void Set(string key, int vaule);
        void Set(string key, float vaule);
        void Set(string key, string vaule);
    }
}