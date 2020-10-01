using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneM
{
    public interface IDataPersister
    {
        DataSettings GetDataSettings();

        Data SaveData();

        void LoadData(Data data);
    }

    [Serializable]
    public class DataSettings
    {

        public string dataKey = System.Guid.NewGuid().ToString();
        public PersistenceType persistenceType = PersistenceType.LevelOnly;
#if UNITY_EDITOR
        public int InstanceId;
#endif
        public override string ToString()
        {
            return dataKey + " " + persistenceType.ToString();
        }
    }

    

   
}
