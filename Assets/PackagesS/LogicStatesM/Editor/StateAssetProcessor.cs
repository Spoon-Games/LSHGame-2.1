using System.IO;

namespace LogicStateM.Editor
{
    public class StateAssetProcessor : UnityEditor.AssetModificationProcessor
    {
        public StateAssetProcessor()
        {

        }



        static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (string path in paths)
            {
                if (Path.GetExtension(path).Equals(".controller"))
                {
                    LSMRepository.Instance.UpdateController(path);
                }
            }
            return paths;
        }
    } 
}
