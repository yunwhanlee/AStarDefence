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
    [field: SerializeField] public Vector2Int CurSelectPos {get; set;}

    [Header("Board Tile Map")]
    public Tilemap BoardTileMap;
    [field: SerializeField] public TileBase[] Boards {get; set;}

    void Start() {
        WallSpawnCnt = 0;
        SpawnWall();
    }

    void Update() {
    #region CLICK TILE EVENT
        if(Input.GetMouseButtonDown(0)) {
            onClickTile();
        }
    #endregion
    }

#region EVENT
    private void onClickTile() {
            RaycastHit2D hit;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 스크린 좌표를 월드 좌표로 변환
            Ray2D ray = new Ray2D(mousePos, Vector2.zero); // 방향은 일단 임시로 0 벡터를 사용하거나, 필요한 방향으로 설정할 수 있습니다.
            hit = Physics2D.Raycast(ray.origin, ray.direction);

            int rayX = (int)Math.Round(ray.origin.x);
            int rayY = (int)Math.Round(ray.origin.y);
            Debug.Log($"onClickTile():: rayX= {rayX}, rayY= {rayY}");

            //* アクションバー上をクリックしたら、以下の処理しなくて終了
            if(rayY <= -4 && GM._.actBar.PanelObj.activeSelf) 
                return;

            //* 選択OFF リセット
            SelectedTileMap.ClearAllTiles();
            GM._.actBar.PanelObj.SetActive(false);

            //* 同じ場所のまたクリック、そのまま終了
            if(CurSelectPos == new Vector2Int(rayX, rayY)) {
                CurSelectPos = new Vector2Int(-999, -999);
                return;
            }

            //* 選択領域の制限
            if(rayX < Config.START_POS.x) return;
            if(rayX > Config.GOAL_POS.x) return;
            if(rayY > Config.START_POS.y) return;
            if(rayY < Config.GOAL_POS.y) return;

            CurSelectPos = new Vector2Int(rayX, rayY);

            //* 選択タイル 表示
            SelectedTileMap.SetTile(new Vector3Int(rayY, rayX, 0), SelectArea);

            //* アクションバーUI 表示
            var actBar = GM._.actBar;
            actBar.PanelObj.SetActive(true);

            Debug.Log("hit.collider= " + (hit.collider == null));
            if(hit.collider == null) {
                actBar.ActiveIconsByLayer(Enum.Layer.Default);
            }
            else {
                int layer = hit.collider.gameObject.layer;
                switch(layer) {
                    case Enum.Layer.Wall:
                        actBar.ActiveIconsByLayer(layer);
                        break;
                    case Enum.Layer.Board:
                        actBar.ActiveIconsByLayer(layer);
                        break;
                    case Enum.Layer.CCTower:
                        actBar.ActiveIconsByLayer(layer);
                        break;
                    default:
                        actBar.ActiveIconsByLayer(Enum.Layer.Default);
                        break;
                }
            }
    }
#endregion

#region FUNC
    private void SpawnWall() {
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
            SpawnWall();
        }
    }
    public void InstallBoard() {
        Debug.Log("InstallBoard()::");
        var pos = new Vector3Int(CurSelectPos.y, CurSelectPos.x, 0);
        BoardTileMap.SetTile(pos, Boards[Random.Range(0, Boards.Length)]);
    }
#endregion
}
