using UnityEngine;

namespace TagInterpreterR
{
    public static class FileLoader
    {
        public static string Load(string path)
        {
            TextAsset asset = Resources.Load<TextAsset>(path);

            if (asset == null)
            {
                Debug.LogError("Asset " + path + "could not be found");
                return null;
            }

            return asset.text;
        }
    }
}
