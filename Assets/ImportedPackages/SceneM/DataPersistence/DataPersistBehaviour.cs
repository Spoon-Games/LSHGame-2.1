using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneM
{
    public abstract class DataPersistBehaviour : MonoBehaviour, IDataPersister
    {
        [SerializeField]
        private DataSettings dataSettings = new DataSettings();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(dataSettings.InstanceId != GetInstanceID() && !Application.isPlaying)
            {
                dataSettings.dataKey = System.Guid.NewGuid().ToString();
                dataSettings.InstanceId = GetInstanceID();
            }
        }
#endif

        public DataSettings GetDataSettings()
        {
            return dataSettings;
        }

        public abstract Data SaveData();

        public abstract void LoadData(Data data);
    }
}
