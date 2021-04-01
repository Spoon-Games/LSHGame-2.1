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

        internal float rotation;

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

            var tileMap = brushTarget.GetComponent<Tilemap>();
            var tileBase = tileMap.GetTile(position);

            //if (tileBase is PrefabTile prefabTile)
            //{
            //    //Debug.Log("Painted PrefabTile: " + registeredPrefabTile.tileName);
            //    InstantiatePrefab(gridLayout, brushTarget, position, prefabTile);

            //    eraseByPaint = true;
            //    base.Erase(gridLayout, brushTarget, position);
            //    eraseByPaint = false;

            //}
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            SetMatrix(Vector3Int.zero, Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotation)));
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
                instance.transform.rotation = Quaternion.Euler(0, 0, rotation);
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

        #endregion
    }

    [CustomEditor(typeof(UniversalBrush))]
    public class UniversalBrushEditor : UnityEditor.Tilemaps.GridBrushEditor
    {
        private UniversalBrush Brush { get { return target as UniversalBrush; } }

        private int _rotation = 0;
        private int Rotation { get => _rotation; set
            {
                _rotation = (value % 360 + 360) % 360;

                Brush.rotation = _rotation;
            } }

        public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);
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
            Rotation = EditorGUILayout.IntField("Rotation", Rotation);
        }

        private void GetInput()
        {
            Event e = Event.current;

            if (e.type == EventType.KeyDown)
            {
                if(e.keyCode == KeyCode.Q)
                {
                    Rotation += 90;
                    e.Use();
                }
                else if(e.keyCode == KeyCode.E)
                {
                    Rotation -= 90;
                    e.Use();
                }

                
            }
        }
    }

}

#endif