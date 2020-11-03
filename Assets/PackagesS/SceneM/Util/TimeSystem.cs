using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SceneM
{
    public class TimeSystem : Singleton<TimeSystem>
    {
        public static Action OnFixedUpdate;

        private static SortedDictionary<float, Action<float>> delayActions = new SortedDictionary<float, Action<float>>();

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();

            if (delayActions.Count > 0) {
                var d = delayActions.First();
                if(d.Key <= Time.fixedTime)
                {
                    d.Value?.Invoke(Time.fixedTime);
                    delayActions.Remove(d.Key);
                }
            }
        }

        public static void Delay(float delay,Action<float> action,bool isFixedUpdate = false)
        {
            if (delay <= 0)
                action.Invoke(Time.time);

            if (!isFixedUpdate)
                delayActions.Add(Time.time + delay, action);
            else
                delayActions.Add(Time.fixedTime + delay, action);
        }

        public static void DelayAbsolut(Action<float> action,float endTime)
        {
            if (endTime <= Time.time)
                action.Invoke(Time.time);

            delayActions.Add(endTime, action);
        }
    }
}
