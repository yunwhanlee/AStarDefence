using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TileMapController : MonoBehaviour {
    [Header("Wall Tile Map")]
    public Tilemap WallTileMap;
    [field: SerializeField] public TileBase[] Walls {get; set;}
    [field: SerializeField] public int WallSpawnPer {get; private set;} = 5;  // 壁の生成確率
    [field: SerializeField] public int WallSpawnMax {get; private set;} = 15; // 壁のMAX数
    [field: SerializeField] public int WallSpawnCnt {get; private set;}

    [Header("Selected Tile Map")]
    public Tilemap SelectedTileMap;
    [field: SerializeField] public TileBase RedArea {get; set;}
    [field: SerializeField] public TileBase SelectArea {get; set;}
    [field: SerializeField] public Vector2Int curSelectPos {get; set;}

    void Start() {
        WallSpawnCnt = 0;
        spawnWall();
    }

    void Update() {
        //* Click Tile 
        if(Input.GetMouseButtonDown(0)) {
            RaycastHit2D hit;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 스크린 좌표를 월드 좌표로 변환
            Ray2D ray = new Ray2D(mousePos, Vector2.zero); // 방향은 일단 임시로 0 벡터를 사용하거나, 필요한 방향으로 설정할 수 있습니다.
            hit = Physics2D.Raycast(ray.origin, ray.direction);

            int rayX = (int)Math.Round(ray.origin.x);
            int rayY = (int)Math.Round(ray.origin.y);
            Debug.Log("ray Pos: " + rayX + ", " + rayY);

            //* 選択OFF リセット
            SelectedTileMap.ClearAllTiles();
            GM._.actBar.PanelObj.SetActive(false);

            //* 同じ場所のまたクリック、そのまま終了
            if(curSelectPos == new Vector2Int(rayX, rayY)) {
                curSelectPos = new Vector2Int(-999, -999);
                return;
            }

            //* 選択領域の制限
            if(rayX < Config.START_POS.x) return;
            if(rayX > Config.GOAL_POS.x) return;
            if(rayY > Config.START_POS.y) return;
            if(rayY < Config.GOAL_POS.y) return;

            curSelectPos = new Vector2Int(rayX, rayY);

            //* 選択タイル 表示
            SelectedTileMap.SetTile(new Vector3Int(rayY, rayX, 0), SelectArea);

            //* アクションバーUI 表示
            var actBar = GM._.actBar;
            actBar.PanelObj.SetActive(true);

            Debug.Log("hit.collider= " + (hit.collider == null));
            if(hit.collider == null) {
                actBar.activeIconsByLayer("Land");
            }
            else {
                int layer = hit.collider.gameObject.layer;
                if(layer == LayerMask.NameToLayer(Enum.Layer.Wall.ToString())) {
                    actBar.activeIconsByLayer(Enum.Layer.Wall.ToString());
                }
            }

            // if (hit.collider == null){
                // Debug.Log("Hit Obj: " + hit.collider.gameObject.name);
                // SelectedTileMap.SetTile(new Vector3Int(rayY, rayX, 0), RedArea);
            // }
            // else {
                // SelectedTileMap.SetTile(new Vector3Int(rayY, rayX, 0), SelectArea);
            // }

        }
    }

#region FUNC
    public void spawnWall() {
        var sp = Config.START_POS;
        var gp = Config.GOAL_POS;
        const int ofs = 1;
        for(int y = gp.y + ofs; y < sp.y + ofs; y++) {
            for(int x = sp.x + ofs; x < gp.x; x++) {
                int rd = Random.Range(0, 100);
                if(rd < WallSpawnPer) {
                    //* 生成・カウント++
                    WallTileMap.SetTile(new Vector3Int(y, x, 0), Walls[Random.Range(0, Walls.Length)]);
                    WallSpawnCnt++;

                    //* 生成数を満たしたら、終了
                    if(WallSpawnCnt >= WallSpawnMax) {
                        return;
                    }
                }
            }
        }

        //* 生成数が足りなかったら、再起呼び出す
        if(WallSpawnCnt < WallSpawnMax) {
            spawnWall();
        }
    }
#endregion
}
