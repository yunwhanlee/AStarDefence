using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CollectionScripts;
using Unity.Mathematics;
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
    [field: SerializeField] public int SelectLayer {get; set;}

    [field: SerializeField] public GameObject HitObject {get; set;}

    [Header("Prefab")]
    [SerializeField] private GameObject[] BoardPfs;

    void Start() {
        SpawnWall();
    }

    void Update() {
    #region CLICK TILE EVENT
        if(Input.GetMouseButtonDown(0)) {
            OnClickTile();
        }
    #endregion
    }

#region EVENT
    private void OnClickTile() {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 스크린 좌표를 월드 좌표로 변환
        int x = (int)Math.Round(mousePos.x);
        int y = (int)Math.Round(mousePos.y);

        //* アクションバー上をクリックしたら、以下の処理しない
        const int ACTION_BAR_START_POS_Y = -4;
        if(y <= ACTION_BAR_START_POS_Y) return;
        Debug.Log("<color=white>onClickTile():: mouse.x= " + x + ", y= " + y + "</color>");

        RaycastHit2D hit;
        Ray2D ray = new Ray2D(new Vector2(x, y), Vector2.zero); // 방향은 일단 임시로 0 벡터를 사용하거나, 필요한 방향으로 설정할 수 있습니다.
        hit = Physics2D.Raycast(ray.origin, ray.direction);
        Collider2D HitCollider = hit.collider;

        //* 選択OFF リセット
        SelectedTileMap.ClearAllTiles();
        GM._.actBar.PanelObj.SetActive(false);

        //* 同じ場所のまたクリック、そのまま終了
        if(CurSelectPos == new Vector2Int(x, y)) {
            CurSelectPos = new Vector2Int(-999, -999);
            Reset();
            return;
        }

        //* 選択領域の制限
        if(x < Config.START_POS.x) return;
        if(x > Config.GOAL_POS.x) return;
        if(y > Config.START_POS.y) return;
        if(y < Config.GOAL_POS.y) return;

        CurSelectPos = new Vector2Int(x, y);

        //* 選択タイル 表示
        SelectedTileMap.SetTile(new Vector3Int(y, x, 0), SelectArea);

        //* アクションバーUI 表示
        var actBar = GM._.actBar;
        actBar.PanelObj.SetActive(true);

        if(HitCollider == null) {
            Debug.Log($"OnClickTile():: HitCollider is Null= {HitCollider == null}");
            actBar.ActiveIconsByLayer(Enum.Layer.Default);
            Reset();
        }
        else {
            HitObject = HitCollider.gameObject;
            SelectLayer = HitCollider.gameObject.layer;
            switch(SelectLayer) {
                case Enum.Layer.Wall:
                    actBar.ActiveIconsByLayer(SelectLayer);
                    break;
                case Enum.Layer.Board:
                    if(HitObject.GetComponent<Board>().IsTowerOn)
                        actBar.ActiveIconsByLayer(Enum.Layer.Tower);
                    else
                        actBar.ActiveIconsByLayer(Enum.Layer.Board);
                    break;
                case Enum.Layer.CCTower:
                    actBar.ActiveIconsByLayer(SelectLayer);
                    break;
                default:
                    actBar.ActiveIconsByLayer(Enum.Layer.Default);
                    break;
            }
        }
    }
#endregion

#region FUNC
    private Vector3Int getCurSelectedPos(bool isTile = false) {
        if(isTile) //* タイルなら、YとXが反転されている
            return new(CurSelectPos.y, CurSelectPos.x, 0);
        else
            return new(CurSelectPos.x, CurSelectPos.y, 0);
    } 
    
    /// <summary>
    /// 壁をランダムで設置
    /// </summary>
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

    public void Reset() {
        Debug.Log("Reset():: Data");
        SelectLayer = 0;
        HitObject = null;
    }

    public void BreakWallTile() {
        Debug.Log($"BreakWallTile():: curSelectedPos= {getCurSelectedPos()}");
        WallTileMap.SetTile(getCurSelectedPos(isTile: true), null);
        CurSelectPos = new Vector2Int(-999, -999);
        Reset();
    }

    public void InstallBoardTile() {
        Debug.Log("InstallBoard()::");
        GameObject ins = Instantiate(BoardPfs[Random.Range(0, BoardPfs.Length)], getCurSelectedPos(), quaternion.identity);
        HitObject = ins;
    }

    public void InstallIceTowerTile() {
        Debug.Log("InstallIceTower()::");
        GameObject ins = Instantiate(GM._.tm.IceTowers[0], getCurSelectedPos(), quaternion.identity);
        HitObject = ins;
    }

    public void InstallStunTowerTile() {
        Debug.Log("InstallStunTowerTile()::");
        GameObject ins = Instantiate(GM._.tm.StunTowers[0], getCurSelectedPos(), quaternion.identity);
        HitObject = ins;
    }

    public void DeleteTile() {
        Debug.Log($"DeleteTile():: SelectLayer= {SelectLayer}");
        if(HitObject.layer == Enum.Layer.CCTower)
            GM._.actBar.SetCCTowerCntTxt(-1);
        Destroy(HitObject);
        CurSelectPos = new Vector2Int(-999, -999);
        Reset();
    }

#endregion
}
