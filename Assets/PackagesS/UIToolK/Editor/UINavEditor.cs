using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UINavigation.Editor
{
    public class UINavEditor : EditorWindow
    {
        private UINavGraphView graphView;
        private NavToolbar toolbar;

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

        internal void SaveData()
        {
            SaveUINavUtility.GetInstance(graphView,toolbar).Save(filePath, repository);
        }

        private void LoadData()
        {
            SaveUINavUtility.GetInstance(graphView,toolbar).Load(repository);
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
            toolbar = new NavToolbar(this);
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

                rootVisualElement.Remove(graphView);
            }
        }
    } 

    public class NavToolbar : Toolbar
    {
        public TextField DefaultInputControllerField;
        public TextField DefaultInAnimationField;
        public TextField DefaultOutAnimationField;

        public NavToolbar(UINavEditor parent)
        {
            Add(new Button(() => parent.SaveData()) { text = "Save" });

            DefaultInputControllerField = new TextField("Default Input Controller");
            Add(DefaultInputControllerField);
            DefaultInAnimationField = new TextField("Default In Animation");
            Add(DefaultInAnimationField);
            DefaultOutAnimationField = new TextField("Default Out Animation");
            Add(DefaultOutAnimationField);

            parent.rootVisualElement.Add(this);
        }
    }
}
