#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

using UnityEditor;


namespace LSHGame.Util
{
    [CustomGridBrush(true, false, false, "Universal Brush")]
    public class UniversalBrush : GridBrush
    {

        private static bool eraseByPaint = false;


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

            if (tileMap.GetTile(position) is PrefabTile prefabTile)
            {
                //Debug.Log("Painted PrefabTile: " + registeredPrefabTile.tileName);
                InstantiatePrefab(gridLayout, brushTarget, position, prefabTile);

                eraseByPaint = true;
                base.Erase(gridLayout, brushTarget, position);
                eraseByPaint = false;
            }
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            base.BoxFill(gridLayout, brushTarget, position);
            var tileMap = brushTarget.GetComponent<Tilemap>();

            foreach (var pos in position.allPositionsWithin)
            {
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
                return null;
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
        private UniversalBrush lineBrush { get { return target as UniversalBrush; } }

        public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);
            //if (lineBrush.lineStartActive)
            //{
            //    Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
            //    if (tilemap != null)
            //        tilemap.ClearAllEditorPreviewTiles();

            //    // Draw preview tiles for tilemap
            //    Vector2Int startPos = new Vector2Int(lineBrush.lineStart.x, lineBrush.lineStart.y);
            //    Vector2Int endPos = new Vector2Int(position.x, position.y);
            //    if (startPos == endPos)
            //        PaintPreview(grid, brushTarget, position.min);
            //    else
            //    {
            //        foreach (var point in LineBrush.GetPointsOnLine(startPos, endPos, lineBrush.fillGaps))
            //        {
            //            Vector3Int paintPos = new Vector3Int(point.x, point.y, position.z);
            //            PaintPreview(grid, brushTarget, paintPos);
            //        }
            //    }

            //    if (Event.current.type == EventType.Repaint)
            //    {
            //        var min = lineBrush.lineStart;
            //        var max = lineBrush.lineStart + position.size;

            //        // Draws a box on the picked starting position
            //        GL.PushMatrix();
            //        GL.MultMatrix(GUI.matrix);
            //        GL.Begin(GL.LINES);
            //        Handles.color = Color.blue;
            //        Handles.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(max.x, min.y, min.z));
            //        Handles.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, max.y, min.z));
            //        Handles.DrawLine(new Vector3(max.x, max.y, min.z), new Vector3(min.x, max.y, min.z));
            //        Handles.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(min.x, min.y, min.z));
            //        GL.End();
            //        GL.PopMatrix();
            //    }
            //}
        }
    }

}

#endif