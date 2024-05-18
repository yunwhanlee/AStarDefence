using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CollectionScripts;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TileMapController : MonoBehaviour {
    public static Vector2Int GOAL_POS;
    public static Vector2Int START_POS;

    [Header("Wall Tile Map")]
    public Tilemap WallTileMap;
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
    [field: SerializeField] public GameObject SwitchBefHitObject {get; set;}

    void Start() {
        //! なぜかこのY軸は逆にしないとエラーになる
        int x = (GM._.Stage == Config.Stage.STG_INFINITE_DUNGEON)? 7 : 6;
        int topY = (GM._.Stage == Config.Stage.STG_INFINITE_DUNGEON)? 3 : 2;
        GOAL_POS = new Vector2Int(-x, 2);
        START_POS = new Vector2Int(x, -topY);

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
    /// <summary>
    /// タイルマップのクリック処理
    /// </summary>
    /// <param name="_x">MergableItemのクリックイベント用としてX軸を渡す</param>
    /// <param name="_y">MergableItemのクリックイベント用としてY軸を渡す</param>
    /// <param name="col">MergableItemのクリックイベント用のコライダーを渡す</param>
    public void OnClickTile(int _x = -9999, int _y = -9999, Collider2D col = null) {
        //* UIに触れているなら、以下のRayCast処理しなくて終了
        // #if UNITY_EDITOR
        if (EventSystem.current.currentSelectedGameObject && !col) {
            Debug.Log($"OnClickTile():: Click UI Area");
            return;
        }
        // #else
        //     if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;
        // #endif

        if(GM._.State == GameState.Pause) return;
        if(GM._.State == GameState.Gameover) return;

        SM._.SfxPlay(SM.SFX.ClickSFX);

        // 스크린 좌표를 월드 좌표로 변환
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
        int x = (_x == -9999)? (int)Math.Round(mousePos.x) : _x;
        int y = (_y == -9999)?  (int)Math.Round(mousePos.y) : _y;

        //* スタートとゴール地点なら、以下処理しない
        if(new Vector2(x,y) == GM._.pfm.startPos) return;
        if(new Vector2(x,y) == GM._.pfm.goalPos) return;

        //* アクションバー領域をクリックしたら、以下の処理しない
        const int ACTION_BAR_BEGIN_POS_Y = -4;
        if(y <= ACTION_BAR_BEGIN_POS_Y) return;
        Debug.Log("<color=white>onClickTile():: mouse.x= " + x + ", y= " + y + "</color>");

        // Raycast場外するオブジェクトのレイア
        int towerLayerMask = 1 << Enum.Layer.TowerRange;
        int enemyLayerMask = 1 << Enum.Layer.Enemy; //! (BUG)特にFlight敵などがタワーの上にあれば、タワークリックができなくて、Default選択として空のタイル選択になること対応
        int exceptLayerMasks = ~(towerLayerMask | enemyLayerMask);

        RaycastHit2D hit;
        // 방향은 일단 임시로 0 벡터를 사용하거나, 필요한 방향으로 설정할 수 있습니다.
        Ray2D ray = new Ray2D(new Vector2(x, y), Vector2.zero); 
        hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, exceptLayerMasks);
        Collider2D HitCollider = (col == null)? hit.collider : col;

        if(hit) Debug.Log("hit=" + hit.collider.name);
        if(HitCollider != null && HitCollider.gameObject.layer == Enum.Layer.UI) {
            Debug.Log("OnClickTile():: Hitcollider -> UI。 そのまま終了。");
            Reset();
            return;
        }

        //* お互いにタワー位置変更 (Switch-Icon)
        if(GM._.actBar.IsSwitchMode) {
            //* エラー１
            if(HitCollider == null || HitCollider.gameObject.layer == Enum.Layer.Wall) {
                GM._.gui.ShowMsgError("타워를 선택해주세요!");
                return;
            }
            //* エラー２
            else if(SwitchBefHitObject == HitCollider.gameObject) {
                GM._.gui.ShowMsgError("자기 이외에 타워를 선택해주세요!");
                return;
            }
            //* 位置切り替え
            else if(HitCollider && SwitchBefHitObject != HitCollider) {
                //* カウント減る
                GM._.actBar.SwitchCntTxt.text = $"{--GM._.actBar.SwitchCnt}";

                //* 位置変更
                Vector2 tempPos = HitCollider.transform.position;
                HitCollider.transform.position = SwitchBefHitObject.transform.position;
                SwitchBefHitObject.transform.position = tempPos;

                //* トリガーOFF・メッセージOFF
                GM._.actBar.PanelObj.SetActive(false);
                GM._.actBar.SwitchModeOff();

                //* 選択OFF リセット
                SelectedTileMap.ClearAllTiles();
                GM._.actBar.PanelObj.SetActive(false);

                Reset();
                return;
            }
        }


        //* 先に初期化 (選択OFFの状態)
        SelectedTileMap.ClearAllTiles();
        GM._.actBar.PanelObj.SetActive(false);
        //* DeleteトリガーとUI初期化
        GM._.actBar.IsDeleteTrigger = false;
        GM._.actBar.IconBtns[(int)ActionBarUIManager.ICON.Delete].GetComponent<Image>().color = Color.white;
        

        GameObject prevHitObj = HitObject;
        GameObject currHitObj = HitCollider? HitCollider.gameObject : null;
        Debug.Log($"OnClickTile():: SelectLayer= {SelectLayer}, befHitObj= {prevHitObj}, curHitObj= {currHitObj}");

        //* 以前に選択した領域のレイアがタワーであり
        if(SelectLayer != 0) {
            //* 以前に選択された物と現在選択した物があったら
            if(prevHitObj && currHitObj) {
                // そのまま、領域表示を続く
            }
            //* 以前とか現在に選択した物がなければ
            else {
                // リセット ➝ 終了
                Reset();
                return;
            }
        }

        //* また同じ場所のクリック
        if(CurSelectPos == new Vector2Int(x, y)) {
            // リセット ➝ 終了
            Reset();
            return;
        }

        //* 制限した領域のクリック
        if(x < GOAL_POS.x || x > START_POS.x || y > GOAL_POS.y || y < START_POS.y) {
            Reset(); // リセット ➝ 終了
            return;
        }

        CurSelectPos = new Vector2Int(x, y);

        //* 選択タイル 表示
        SelectedTileMap.SetTile(new Vector3Int(y, x, 0), SelectArea);

        //* アクションバーUI 表示
        var actBar = GM._.actBar;
        actBar.PanelObj.SetActive(true);

        //* 選択処理
        if(HitCollider == null) {
            Debug.Log($"OnClickTile():: HitCollider is Null= {HitCollider == null}");
            actBar.UpdateUI(Enum.Layer.Default);
            Reset(isClearPos: false);
        }
        else {
            //* Set HitObj & Layer
            HitObject = HitCollider.gameObject;
            SelectLayer = HitCollider.gameObject.layer;

            switch(SelectLayer) {
                case Enum.Layer.Wall:
                    actBar.UpdateUI(SelectLayer);
                    Reset(isClearPos: false);
                    break;
                case Enum.Layer.Board:
                    GM._.tm.ClearAllTowerRanges();
                    if(HitObject.GetComponent<Board>().IsTowerOn) { //* タワーが有る
                        actBar.UpdateUI(Enum.Layer.Tower);
                        HitObject.GetComponentInChildren<Tower>().trc.SprRdr.enabled = true;
                    }
                    else { //* ボードのみ
                        actBar.UpdateUI(Enum.Layer.Board);
                    }
                    break;
                case Enum.Layer.CCTower:
                    GM._.tm.ClearAllTowerRanges();
                    var tower = HitObject.GetComponent<Tower>();
                    switch(tower.Type) {
                        case TowerType.CC_IceTower:
                            var icetower = tower as IceTower;
                            icetower.trc.SprRdr.enabled = true;
                            break;
                        case TowerType.CC_StunTower:
                            var stunTower = tower as StunTower;
                            stunTower.trc.SprRdr.enabled = true;
                            break;
                    }
                    actBar.UpdateUI(SelectLayer);
                    break;
                default:
                    actBar.UpdateUI(Enum.Layer.Default);
                    Reset(isClearPos: false);
                    break;
            }
        }
    }
#endregion

#region FUNC
    public Vector3Int getCurSelectedPos(bool isTile = false) {
        if(isTile) //* タイルなら、YとXが反転されている
            return new(CurSelectPos.y, CurSelectPos.x, 0);
        else
            return new(CurSelectPos.x, CurSelectPos.y, 0);
    } 
    
    /// <summary>
    /// 壁をランダムで設置
    /// </summary>
    public void SpawnWall() {
        List<Vector2Int> posList = new List<Vector2Int>();
        var sp = GOAL_POS;
        var gp = START_POS;

        WallTileMap.ClearAllTiles();

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
            var walls = GM._.StageDts[GM._.Stage].Walls;
            WallTileMap.SetTile(pos, walls[Random.Range(0, walls.Length)]);
        }
    }

    public void Reset(bool isClearPos = true) {
        Debug.Log("Reset():: Data");
        //* 選択した位置
        if(isClearPos)
            CurSelectPos = new Vector2Int(-999, -999);

        //* タワーが有ったら、タワー範囲表示を非表示
        GM._.tm.ClearAllTowerRanges();

        SelectLayer = 0;
        HitObject = null;

        // //* DeleteトリガーとUI初期化
        // GM._.actBar.IsDeleteTrigger = false;
        // GM._.actBar.IconBtns[(int)ActionBarUIManager.ICON.Delete].GetComponent<Image>().color = Color.white;
    }

    public void BreakWallTile() {
        Debug.Log($"BreakWallTile():: curSelectedPos= {getCurSelectedPos()}");
        var idx = GM._.Stage == 0? GameEF.StoneDestroyEF : GameEF.GrassDestroyEF;
        var pos = new Vector2(getCurSelectedPos().x, getCurSelectedPos().y);
        GM._.gef.ShowEF(idx, pos, Util.Time1);
        WallTileMap.SetTile(getCurSelectedPos(isTile: true), null); //* 壁 破壊
        Reset(isClearPos: false);
    }

    public void DeleteTile() {
        Debug.Log($"DeleteTile():: SelectLayer= {SelectLayer}");
        int refund = 0;

        //* CCタワー
        if(HitObject.layer == Enum.Layer.CCTower) {
            GM._.actBar.SetCCTowerCntTxt(-1);
            var ccTower = HitObject.GetComponent<Tower>();
            int totalPrice = ccTower.Lv * Config.G_PRICE.CCTOWER;
            refund = (int)Math.Floor(totalPrice * Config.G_PRICE.DELETE_REFUND_PER);

            //* 削除
            GM._.gef.ShowIconTxtEF(ccTower.transform.position, refund, "Meat");
            Destroy(ccTower.gameObject);
            Reset();
            GM._.actBar.PanelObj.SetActive(false);
            GM._.tmc.SelectedTileMap.ClearAllTiles();

            //* 新しい経路表示
            GM._.pfm.PathFinding(isShowPath: true);
        }
        //* Board -> Tower順番で消す
        else { 
            Board board = HitObject.GetComponent<Board>();

            //* RANDOMタワー
            if(board.IsTowerOn) { 
                board.IsTowerOn = false;
                GM._.actBar.UpdateUI(Enum.Layer.Board);
                Tower tower = board.GetComponentInChildren<Tower>();

                // 返金
                int totalPrice = tower.Lv * Config.G_PRICE.TOWER;
                refund = (int)Math.Floor(totalPrice * Config.G_PRICE.DELETE_REFUND_PER);

                // 削除
                GM._.gef.ShowIconTxtEF(tower.transform.position, refund, "Meat");
                Destroy(tower.gameObject);
            }
            //* ボード
            else { 
                if(GM._.gui.ShowErrMsgCreateTowerAtPlayState())
                    return;

                // 返金
                refund = Config.G_PRICE.BOARD;

                // 削除
                GM._.gef.ShowIconTxtEF(board.transform.position, Config.G_PRICE.BOARD, "Meat");
                GM._.gef.ShowEF(GameEF.WoodDestroyEF, board.transform.position);
                Destroy(board.gameObject);
                Reset();
                GM._.actBar.PanelObj.SetActive(false);
                GM._.tmc.SelectedTileMap.ClearAllTiles();

                //* 新しい経路表示
                GM._.pfm.PathFinding(isShowPath: true);
            }
        }
        //* 返金適用
        GM._.SetMoney(+refund);
    }

#endregion
}
