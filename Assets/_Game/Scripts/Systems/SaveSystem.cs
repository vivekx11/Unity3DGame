using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Game.Systems
{
    /// <summary>
    /// Simple multi-slot binary save/load using BinaryFormatter for demo purposes.
    /// Editor tips: Save slots are files under Application.persistentDataPath. For production, consider safe encryption and versioning.
    /// Design notes: Uses a lightweight SaveData POCO. BinaryFormatter is marked obsolete in some contexts; replace with custom serializer in production.
    /// </summary>
    public static class SaveSystem
    {
        [Serializable]
        public class SaveData
        {
            public int souls;
            public Vector3 playerPosition;
            public int level;
        }

        public static void Save(int slot, SaveData data)
        {
            var path = PathForSlot(slot);
            try
            {
                using (var fs = new FileStream(path, FileMode.Create))
                {
#pragma warning disable SYSLIB0011
                    var bf = new BinaryFormatter();
                    bf.Serialize(fs, data);
#pragma warning restore SYSLIB0011
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Save failed: " + ex);
            }
        }

        public static SaveData Load(int slot)
        {
            var path = PathForSlot(slot);
            if (!File.Exists(path)) return null;
            try
            {
                using (var fs = new FileStream(path, FileMode.Open))
                {
#pragma warning disable SYSLIB0011
                    var bf = new BinaryFormatter();
                    var obj = bf.Deserialize(fs) as SaveData;
#pragma warning restore SYSLIB0011
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Load failed: " + ex);
                return null;
            }
        }

        private static string PathForSlot(int slot)
        {
            return Path.Combine(Application.persistentDataPath, $"save_slot_{slot}.bin");
        }
    }
}
