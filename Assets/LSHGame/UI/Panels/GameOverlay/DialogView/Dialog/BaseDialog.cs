using System;
using TagInterpreterR;
using UnityEngine;

namespace LSHGame.UI
{
    public abstract class BaseDialog : ScriptableObject
    {
        public Action<string> ActionCallback;

        protected abstract string GetData();
        public abstract void Show();

        public TagChain Parse()
        {
            string data = GetData();
            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    TagChain tagChain = TagInterpreter.LoadTags(data);
                    return tagChain;
                }
                catch (Exception e)
                {
                    Debug.LogError("Unable to parse dialog " + e);
                }
            }
            return new TagChain();
        }

        public virtual void InvokeAction(string action)
        {
            ActionCallback?.Invoke(action);
        }
    } 
}
