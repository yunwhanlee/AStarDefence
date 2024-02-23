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
    [field: SerializeField] public Collider2D HitCollider {get; private set;}

    [Header("Board Tile Map")]
    public Tilemap BoardTileMap;
    [field: SerializeField] public TileBase[] Boards {get; set;}

    [Header("CC Tile Map")]
    public Tilemap CCTowerTileMap;
    [field: SerializeField] public TileBase IceTower {get; set;}
    [field: SerializeField] public TileBase StunTower {get; set;}

    void Start() {
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
        HitCollider = hit.collider;

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
            HitCollider = null;
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

        Debug.Log("HitCollider= " + (HitCollider == null));
        if(HitCollider == null) {
            actBar.ActiveIconsByLayer(Enum.Layer.Default);
        }
        else {
            int layer = HitCollider.gameObject.layer;
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
    /// <summary>
    /// 壁をランダムで設置
    /// </summary> <summary>
    private void SpawnWall() {
        List<Vector2Int> posList = new List<Vector2Int>();
        var sp = Config.START_POS;
        var gp = Config.GOAL_POS;

        //* 全ての位置をリスト化
        const int ofs = 1;
        for(int y = gp.y + ofs; y < sp.y + ofs; y++)
            for(int x = sp.x + ofs; x < gp.x; x++)
                posList.Add(new Vector2Int(x, y));
        Debug.Log("posList.Count= " + posList.Count);

        //* リスト Mix
        List<Vector2Int> mixedList = new List<Vector2Int>();
        int cnt = posList.Count;
        for(int i = 0; i < cnt; i++) {
            int rand = Random.Range(0, posList.Count);
            mixedList.Add(posList[rand]);
            posList.RemoveAt(rand);
        }
        posList = mixedList; // 結果

        //* 壁を設置
        for(WallSpawnCnt = 0; WallSpawnCnt < WallSpawnMax; WallSpawnCnt++) {
            var pos = new Vector3Int(posList[WallSpawnCnt].y, posList[WallSpawnCnt].x, 0);
            WallTileMap.SetTile(pos, Walls[Random.Range(0, Walls.Length)]);
        }
    }
    private Vector3Int getCurSelectedPos() => new(CurSelectPos.y, CurSelectPos.x, 0);
    public void InstallBoardTile() {
        Debug.Log("InstallBoard()::");
        BoardTileMap.SetTile(getCurSelectedPos(), Boards[Random.Range(0, Boards.Length)]);
    }
    public void InstallIceTowerTile() {
        Debug.Log("InstallIceTower()::");
        CCTowerTileMap.SetTile(getCurSelectedPos(), IceTower);
    }
    public void InstallStunTowerTile() {
        Debug.Log("InstallStunTowerTile()::");
        CCTowerTileMap.SetTile(getCurSelectedPos(), StunTower);
    }
    public void DeleteTile() {
        if(HitCollider != null) {
            Debug.Log("HitCollider.layer= " + HitCollider.gameObject.layer);
        }
    }

#endregion
}
