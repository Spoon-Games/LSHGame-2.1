using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolK.Editor
{
    public class UINavEditor : EditorWindow
    {
        private UINavGraphView graphView;

        private string filePath;

        private UINavRepository repository;

        [MenuItem("Window/Util/UI Navigation Editor")]
        public static void GetWindow()
        {
            GetWindow<UINavEditor>("UI Navigation Editor");
        }


        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Object o = EditorUtility.InstanceIDToObject(instanceID);

            if (o is UINavRepository repository)
            {
                string path = AssetDatabase.GetAssetPath(instanceID);
                UINavEditor dialogEditor = null;

                UINavEditor[] allWindows = Resources.FindObjectsOfTypeAll<UINavEditor>();
                foreach (UINavEditor e in allWindows)
                {
                    if (e.IsRepository(repository))
                    {
                        dialogEditor = e;
                        dialogEditor.Show();
                        break;
                    }
                }

                if (dialogEditor == null)
                    dialogEditor = CreateWindow<UINavEditor>();

                dialogEditor.Open(repository, path);
                return true;
            }

            return false;
        }

        private bool IsRepository(UINavRepository repository)
        {
            return Equals(this.repository, repository);
        }

        private void Open(UINavRepository repository, string path)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (!IsRepository(repository))
            {
                if (this.repository != null)
                {
                    if (EditorUtility.DisplayDialog("File Not Saved", "Save " + System.IO.Path.GetFileNameWithoutExtension(filePath), "OK", "Cancel"))
                    {
                        SaveData();
                    }
                }

                this.repository = repository;
                LoadData();
            }

            this.filePath = path;
            titleContent.text = System.IO.Path.GetFileNameWithoutExtension(path);

        }

        private void SaveData()
        {
            SaveUINavUtility.GetInstance(graphView).Save(filePath, repository);
        }

        private void LoadData()
        {
            SaveUINavUtility.GetInstance(graphView).Load(repository);
        }

        private void CreateGraph()
        {
            graphView = new UINavGraphView(this);
            rootVisualElement.Add(graphView);

            if(repository != null)
                LoadData();
        }

        private void CreateToolbar()
        {    
            Toolbar toolbar = new Toolbar();

            //toolbar.styleSheets.Add(Resources.Load<StyleSheet>("LSMToolbarUSS"));

            toolbar.Add(new Button(SaveData) { text = "Save" });
            //toolbar.Add(new Button(LoadData) { text = "Load Data" });


            //Button nodeCreateButton = new Button(() => { graphView.CreateDialogNode(); });
            //nodeCreateButton.text = "Create new node";
            //toolbar.Add(nodeCreateButton);

            rootVisualElement.Add(toolbar);
        }

        private void OnEnable()
        {
            CreateGraph();
            CreateToolbar();
        }

        private void OnDisable()
        {
            if (graphView != null)
            {
                //if (this.repository != null)
                //{
                //    //if (EditorUtility.DisplayDialog("File Not Saved", "Save " + Path.GetFileNameWithoutExtension(filePath), "OK", "Cancel"))
                //    //{
                //    //    SaveData();
                //    //}
                //}
                rootVisualElement.Remove(graphView);
            }
        }
    } 
}
