
using System.IO;

namespace UtilLib.Data
{
    public class JsonLoadder
    {
        public static T GetLoadJsonObject<T>(string _jsonFileName)
        {
            using (StreamReader r = new StreamReader(_jsonFileName)) {
                var json = r.ReadToEnd();
                //var jsonData = System.Text.Json.JsonConvert.DeserializeObject<T>(json);
                //return jsonData;
                return default(T);
            }
        }
    }
}
