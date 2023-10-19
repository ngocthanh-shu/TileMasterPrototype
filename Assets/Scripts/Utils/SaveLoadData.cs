using System.Collections.Generic;
using System.IO;
using Managers;
using Newtonsoft.Json;
using UnityEngine;

namespace Utils
{
    public static class SaveLoadData
    {
        private static string _combinePath = Application.persistentDataPath + "/";
        private static string _fileTypes = ".shu";

        public static void SaveLevels(List<LevelInformation> levels)
        {
            for(int i = 0; i < levels.Count; i++)
            {
                SaveLevel(levels[i], i + 1);
            }
        }

        private static void SaveLevel(LevelInformation level, int i)
        {
            string path = _combinePath + "level" + i + _fileTypes;
            string jsonData = JsonConvert.SerializeObject(level);
            File.WriteAllText(path, jsonData);
        }
        
        public static LevelInformation LoadLevel(int level)
        {
            string path = _combinePath + "level" + level + _fileTypes;
            if (File.Exists(path))
            {
                string jsonData = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<LevelInformation>(jsonData);
            }
            return CheckFileExistInResources(level);
        }
        
        //Check file exist in folder Resources
        public static LevelInformation CheckFileExistInResources(int level)
        {
            string filename = "level" + level;
            var textAsset = Resources.Load<TextAsset>($"Data/Levels/" + filename);
            if(textAsset != null)
            {
                string path = _combinePath + "level" + level + _fileTypes;
                File.WriteAllText(path, textAsset.text);
                return JsonConvert.DeserializeObject<LevelInformation>(textAsset.text);
            }
            else
            {
                return null;
            }
        }

        public static void DeleteData(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
