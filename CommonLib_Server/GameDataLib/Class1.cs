using game_data_dll;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLib;

namespace GameDataLib
{
    public class GameDataLoader : Singleton<GameDataLoader>
    {
        public List<champ_data> Champdata { get; private set; } = new List<champ_data>();
        public List<exp_data> ExpDataList { get; private set; } = new List<exp_data>();
        public List<ability_data> AbilityDataList { get; private set; } = new List<ability_data>();

        public bool Init(string _path)
        {
            Champdata = Load<champ_data>(_path, "champ_data");
            ExpDataList = Load<exp_data>(_path, "exp_data");
            AbilityDataList = Load<ability_data>(_path, "ability_data");

            return true;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] datas;
        }

        private static T[] FromJson<T>(string _json)
        {
            Wrapper<T> wrapper = JsonConvert.DeserializeObject<Wrapper<T>>(_json);//JsonUtility.FromJson<Wrapper<T>>(_json);
            return wrapper.datas;
        }

        private static List<T> Load<T>(string _path, string _fileName)
        {
            string jsonData = File.ReadAllText(string.Format("{0}/{1}.json", _path, _fileName));

            var _data = FromJson<T>(jsonData);

            List<T> _datas = new List<T>();

            for (int i = 0; i < _data.Length; ++i)
            {
                _datas.Add(_data[i]);
            }

            return _datas;
        }
    }
}
