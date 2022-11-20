using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine;

public class ScriptableTiles : MonoBehaviour
{
   public Tilemap board;
   public TileData newData;
   public GridLayout gridLayout;


   public Vector3Int OnMouseOver(){ //work in progress - needs rework
        Vector3Int tilePos = new Vector3Int(0,0,0);
        if(Input.GetMouseButtonDown(0)){
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tileComponents = gridLayout.WorldToCell(worldPos);
            tilePos.Set(tileComponents.x,tileComponents.y,tileComponents.z);
            board.SetTileFlags(tilePos,TileFlags.None);
            if(board.GetColor(tilePos) == Color.white){
                board.SetColor(tilePos, Color.black);
            }
        }
        return tilePos;
   }

   

//#if UNITY_EDITOR
// The following is a helper that adds a menu item to create a ScriptableTiles object
  //  [MenuItem("Assets/Create/ScriptableTiles")]
    //public static void CreateScriptableTile()
    //{
      //  string path = EditorUtility.SaveFilePanelInProject("Save Scriptable Tile", "New Scriptable Tile", "Asset", "Save Scriptable Tile", "Assets");
        //if (path == "")
          //  return;
   // AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ScriptableTiles>(), path);
    //}
//#endif
}
