
using UnityEngine;

namespace LSHGame.UI
{
    [CreateAssetMenu(menuName = "LSHGame/Dialog/Person")]
    public class Person : ScriptableObject
    {
        public string Name;
        public string TitleName;

        public Mood[] Moods = new Mood[1] { new Mood() { Name = "Default" } };
    }

    [System.Serializable]
    public class Mood
    {
        public string Name;
        public Sprite Picture;
        public CharacterVoice Voice;
    }
}
