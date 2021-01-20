using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.Util
{
    public class EffectTrigger : MonoBehaviour, IEffectTrigger
    {
        private IEffectPlayer[] effectPlayers;

        protected IEffectPlayer[] EffectPlayers
        {
            get
            {
                if(effectPlayers == null)
                {
                    List<IEffectPlayer> list = new List<IEffectPlayer>();
                    this.GetComponents(list);
                    this.GetComponentsInChildren(list);
                    effectPlayers = list.ToArray();
                }
                return effectPlayers;
            }
        }

        public void AddToDict(Dictionary<string, IEffectTrigger> triggers)
        {
            triggers.Add(name, this);
        }

        public void SetMaterial(string material){}

        public void Stop()
        {
            foreach (var p in EffectPlayers)
            {
                p.Stop();
            }
        }

        public void Trigger()
        {
            Debug.Log("Trigger");
            Trigger(null);
        }

        public void Trigger(Bundle parameters)
        {
            foreach(var p in EffectPlayers)
            {
                p.Play(parameters);
            }
        }
    }
}
