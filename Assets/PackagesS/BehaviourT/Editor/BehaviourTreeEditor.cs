using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourT.Editor
{
    public class BehaviourTreeEditorr : EditorWindow
    {
        private TreeGraphView graphView;

        private BehaviourTree repository;

        private ToolbarToggle generateScriptToogle;
        bool wasPlaying = false;

        private bool _isUnsaved = false;
        public bool IsUnsaved { get => _isUnsaved; set
            {
                if(value != _isUnsaved)
                {
                    _isUnsaved = value;
                    if (_isUnsaved)
                        this.titleContent.text = "*" + repository?.name;
                    else
                        this.titleContent.text = repository?.name;

                }
            } }

        [MenuItem("Window/Util/Behaviour Tree Editor")]
        public static void GetWindow()
        {
            GetWindow<BehaviourTreeEditorr>("Behaviour Tree Editor");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Object o = EditorUtility.InstanceIDToObject(instanceID);

            if (o is BehaviourTree repository)
            {
                string path = AssetDatabase.GetAssetPath(instanceID);
                BehaviourTreeEditorr dialogEditor = null;

                BehaviourTreeEditorr[] allWindows = Resources.FindObjectsOfTypeAll<BehaviourTreeEditorr>();
                foreach (BehaviourTreeEditorr e in allWindows)
                {
                    if (e.IsRepository(repository))
                    {
                        dialogEditor = e;
                        dialogEditor.Show();
                        break;
                    }
                }

                if (dialogEditor == null)
                    dialogEditor = CreateWindow<BehaviourTreeEditorr>();

                dialogEditor.Open(repository, path);
                return true;
            }

            return false;
        }

        private void OnInspectorUpdate()
        {
            if (Application.isPlaying)
            {
                if (!wasPlaying)
                    OnSelectionChange();

                graphView?.RuntimeUpdate();
            }
            else if(wasPlaying && repository != null)
            {
                repository.Reset();
                graphView?.RuntimeUpdate();
            }

            wasPlaying = Application.isPlaying;
        }

        private void OnSelectionChange()
        {
            if(Selection.activeGameObject != null && Selection.activeGameObject.TryGetComponent(out BehaviourTreeComponent c) && c.BehaviourTreeInstance != null)
            {
                repository = c.BehaviourTreeInstance;
                LoadData();
            }
        }

        private bool IsRepository(BehaviourTree repository)
        {
            return Equals(this.repository, repository);
        }

        private void Open(BehaviourTree repository, string path)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (!IsRepository(repository))
            {

                this.repository = repository;
                LoadData();
            }
            titleContent.text = System.IO.Path.GetFileNameWithoutExtension(path);

        }

        private void SaveData()
        {
            bool generateScript = generateScriptToogle != null ? generateScriptToogle.value : false;
            SaveBehaviourTreeUtility.GetInstance(graphView).Save( repository, generateScript);
            IsUnsaved = false;
        }

        private void LoadData()
        {
            ShowSaveDialog();
            SaveBehaviourTreeUtility.GetInstance(graphView).Load(repository);
        }

        private void CreateGraph()
        {
            var treeAsset = Resources.Load<VisualTreeAsset>("BehaviourTreeEditor");
            //var ussStyleSheet = Resources.Load<StyleSheet>("BehaviourTreeEditor");
            var editorElement = treeAsset.CloneTree();
            editorElement.StretchToParentSize();
            //editorElement.styleSheets.Add(ussStyleSheet);
            rootVisualElement.Add(editorElement);

            #region Toolbar
            editorElement.Q<ToolbarButton>("SaveButton").clicked += SaveData;
            generateScriptToogle = editorElement.Q<ToolbarToggle>("GenerateScriptToogle");

            VisualElement inspectorContainer = editorElement.Q("InspectorContainer");
            VisualElement propertiesContainer = editorElement.Q("PropertiesContainer");

            //var inspectorToggle = editorElement.Q<ToolbarToggle>("InpsectorToggle");
            //var propertiesToggle = editorElement.Q<ToolbarToggle>("PropertiesToggle");
            //inspectorToggle.RegisterValueChangedCallback(isToogled =>
            //{
            //    if (isToogled.newValue)
            //    {
            //        inspectorContainer.style.display = DisplayStyle.Flex;
            //        propertiesContainer.style.display = DisplayStyle.None;
            //        propertiesToggle.SetValueWithoutNotify(false);
            //    }
            //    else
            //        inspectorToggle.SetValueWithoutNotify(true);
            //});

            //propertiesToggle.RegisterValueChangedCallback(isToogled =>
            //{
            //    if (isToogled.newValue)
            //    {
            //        propertiesContainer.style.display = DisplayStyle.Flex;
            //        inspectorContainer.style.display = DisplayStyle.None;
            //        inspectorToggle.SetValueWithoutNotify(false);
            //    }
            //    else
            //        propertiesToggle.SetValueWithoutNotify(true);
            //});
            #endregion

            graphView = new TreeGraphView(this, inspectorContainer);
            editorElement.Q("GraphViewContainer").Add(graphView);

            if (repository != null)
                LoadData();
        }

        private void OnEnable()
        {
            CreateGraph();
        }

        private void OnDisable()
        {
            if (graphView != null)
            {
                ShowSaveDialog();
                //rootVisualElement.Remove(graphView);
            }
        }

        private void ShowSaveDialog()
        {
            if (this.repository != null && IsUnsaved)
            {
                if (EditorUtility.DisplayDialog("File Not Saved", "Save " + repository.name, "OK", "Cancel"))
                {
                    SaveData();
                }
            }
        }
    }
}
