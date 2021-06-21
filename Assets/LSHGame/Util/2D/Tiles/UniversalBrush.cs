#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace LSHGame.Util
{
    [CustomGridBrush(true, false, false, "Universal Brush")]
    public class UniversalBrush : GridBrush
    {

        private static bool eraseByPaint = false;

        private int _rotation = 0;
        internal int Rotation
        {
            get => _rotation; set
            {
                _rotation = (value % 360 + 360) % 360;
            }
        }
        internal Vector2 Scale { get; set; } = Vector2.one;

        private bool _isSnap = false;
        internal bool IsSnap
        {
            get => _isSnap; set
            {
                if (_isSnap != value)
                {
                    if (value)
                    {
                        if (!FindSnapTilemap())
                            return;
                        Rotation = 0;
                    }
                    _isSnap = value;

                }
            }
        }

        [SerializeField]
        private string _snapTilemapName = "";
        internal string SnapTilemapName { get => _snapTilemapName; set
            {
                if (value != _snapTilemapName)
                {
                    _snapTilemapName = value;
                    IsSnap = false;
                    EditorUtility.SetDirty(this);
                }
            }
        }

        private Tilemap snapTilemap;
        private bool FindSnapTilemap()
        {
            if (!string.IsNullOrEmpty(SnapTilemapName)) {
                var go = GameObject.Find(SnapTilemapName);
                if (go != null && go.TryGetComponent<Tilemap>(out Tilemap tilemap)){

                    snapTilemap = tilemap;
                    return true;
                }
                else
                {
                    Debug.LogError("Could not find SnapTilemap: " + SnapTilemapName);
                }
            }
            else
            {
                Debug.LogError("No SnapTilemap was specified");
            }
            return false;
        }

        [MenuItem("Assets/Create/Universal Brush")]
        public static void CreateBrush()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Universal Brush", "New Universal Brush", "asset", "Save Universal Brush", "Assets");

            if (path == "")
                return;

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<UniversalBrush>(), path);
        }

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            base.Paint(gridLayout, brushTarget, position);
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            
            SetMatrix(Vector3Int.zero, Matrix4x4.Rotate(Quaternion.Euler(0, 0, Rotation + GetSnapRotation(position))) * Matrix4x4.Scale(Scale));
            base.BoxFill(gridLayout, brushTarget, position);
            var tileMap = brushTarget.GetComponent<Tilemap>();

            foreach (var pos in position.allPositionsWithin)
            {
                //if (tileMap.GetTile(pos) is Tile t)
                //{
                //    t.transform *= Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotation));
                //}
                
                if (tileMap.GetTile(pos) is PrefabTile prefabTile)
                {
                    InstantiatePrefab(gridLayout, brushTarget, pos, prefabTile);
                    eraseByPaint = true;
                    base.Erase(gridLayout, brushTarget, pos);
                    eraseByPaint = false;
                }
            }
        }

        //public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        //{
        //    Deregister();

        //    base.Erase(gridLayout, brushTarget, position);

        //    if (registeredPrefabTile != null )
        //    {
        //        Debug.Log("Erased PrefabTile: " + registeredPrefabTile.name);
        //        EraseGameObjects(gridLayout, brushTarget, position);
        //    }
        //    Deregister();
        //}

        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            base.BoxErase(gridLayout, brushTarget, position);
            if (!eraseByPaint)
                EraseGameObjects(gridLayout, brushTarget, position);
        }

        #region Overrides
        //public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        //{
        //    Deregister();

        //    base.BoxErase(gridLayout, brushTarget, position);

        //    if (IsRegistered(position.position))
        //    {
        //        Debug.Log("Erased PrefabTile: " + registeredPrefabTiles.name);
        //    }
        //}

        //public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        //{
        //    base.BoxFill(gridLayout, brushTarget, position);
        //}

        //public override void Select(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        //{
        //    base.Select(gridLayout, brushTarget, position);
        //}

        //public override void Move(GridLayout gridLayout, GameObject brushTarget, BoundsInt from, BoundsInt to)
        //{
        //    base.Move(gridLayout, brushTarget, from, to);
        //}

        //public override void ChangeZPosition(int change)
        //{
        //    base.ChangeZPosition(change);
        //}

        //public override void ResetZPosition()
        //{
        //    base.ResetZPosition();
        //}

        //public override void FloodFill(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        //{
        //    base.FloodFill(gridLayout, brushTarget, position);
        //}

        //public override void Rotate(RotationDirection direction, GridLayout.CellLayout layout)
        //{
        //    base.Rotate(direction, layout);
        //}

        //public override void Flip(FlipAxis flip, GridLayout.CellLayout layout)
        //{
        //    base.Flip(flip, layout);
        //}

        //public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pickStart)
        //{
        //    base.Pick(gridLayout, brushTarget, position, pickStart);
        //}

        //public override void MoveStart(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        //{
        //    base.MoveStart(gridLayout, brushTarget, position);
        //}

        //public override void MoveEnd(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        //{
        //    base.MoveEnd(gridLayout, brushTarget, position);
        //} 
        #endregion

        #region Helper Methods

        private void InstantiatePrefab(GridLayout gridLayout, GameObject brushTarget, Vector3Int position, PrefabTile prefabTile)
        {
            if (prefabTile.GetPrefab() == null)
                return;
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabTile.GetPrefab());
            Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Prefabs");
            if (instance != null)
            {
                instance.transform.SetParent(brushTarget.transform);
                instance.transform.position = gridLayout.LocalToWorld(gridLayout.CellToLocalInterpolated(position + new Vector3(.5f, .5f, .5f))) + (Vector3)prefabTile.pivot;
                instance.transform.rotation = Quaternion.Euler(0, 0, Rotation + GetSnapRotation(position));
                instance.transform.localScale = new Vector3(Scale.x,Scale.y,1);
            }
        }

        private void EraseGameObjects(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            List<Transform> erased = GetObjectsInCell(gridLayout, brushTarget.transform, position);
            foreach (var e in erased)
                Undo.DestroyObjectImmediate(e.gameObject);
        }

        private static List<Transform> GetObjectsInCell(GridLayout grid, Transform parent, BoundsInt position)
        {
            int childCount = parent.childCount;
            if (childCount == 0)
                return new List<Transform>();
            Vector3 min = grid.LocalToWorld(grid.CellToLocalInterpolated(position.min));
            Vector3 max = grid.LocalToWorld(grid.CellToLocalInterpolated(position.max));
            Bounds bounds = new Bounds((max + min) * .5f, max - min);

            List<Transform> transforms = new List<Transform>();

            for (int i = 0; i < childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (bounds.Contains(child.position))
                    transforms.Add(child);
            }
            return transforms;
        }

        private int GetSnapRotation(BoundsInt position)
        {
            return GetSnapRotation(position.min);
        }

        private int GetSnapRotation(Vector3Int position)
        {
            if (IsSnap && snapTilemap != null)
            {
                if (snapTilemap.HasTile(position + Vector3Int.up))
                    return 180;
                else if (snapTilemap.HasTile(position + Vector3Int.left))
                    return 270;
                else if (snapTilemap.HasTile(position + Vector3Int.right))
                    return 90;
            }
            return 0;
        }

        #endregion
    }

    [CustomEditor(typeof(UniversalBrush))]
    public class UniversalBrushEditor : UnityEditor.Tilemaps.GridBrushEditor
    {
        private UniversalBrush Brush { get { return target as UniversalBrush; } }
        

        public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);
            GetInput();
        }


        public override void OnSceneGUI(GridLayout gridLayout, GameObject brushTarget)
        {
            GetInput();
            base.OnSceneGUI(gridLayout, brushTarget);
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            //lineBrush.zp
            GetInput();
            Brush.Rotation = EditorGUILayout.IntField("Rotation", Brush.Rotation);
            Brush.Scale = EditorGUILayout.Vector2Field("Scale", Brush.Scale);

            Brush.IsSnap = EditorGUILayout.Toggle("Is Snap", Brush.IsSnap);
            Brush.SnapTilemapName = EditorGUILayout.TextField("Snap Tilemap", Brush.SnapTilemapName);

            if (GUI.changed)
                EditorUtility.SetDirty(Brush);

            
        }


        private void GetInput()
        {
            int controllId = GUIUtility.GetControlID(FocusType.Keyboard);
            Event e = Event.current;

            if (e.GetTypeForControl(controllId) == EventType.KeyDown)
            {
                if(e.keyCode == KeyCode.Q)
                {
                    Brush.Rotation += 90;
                    e.Use();
                }
                else if(e.keyCode == KeyCode.E)
                {
                    Brush.Rotation -= 90;
                    e.Use();
                }else if(e.keyCode == KeyCode.X)
                {
                    Brush.Scale = new Vector2(-Brush.Scale.x, Brush.Scale.y);
                    e.Use();
                }else if(e.keyCode == KeyCode.S)
                {
                    Brush.IsSnap = !Brush.IsSnap;
                    e.Use();
                }
    
                EditorUtility.SetDirty(this);
            }
        }
    }

}

#endif