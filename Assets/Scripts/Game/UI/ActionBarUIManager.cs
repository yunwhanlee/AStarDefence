using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;

public class ActionBarUIManager : MonoBehaviour {
    public enum ICON {
        Break, Board, Tower, IceTower, ThunderTower, Upgrade, Merge, Switch, Delete, Exit
    };

    const int ICON_PRICE_IDX = 1;

    //* Value
    [field: SerializeField] public bool IsSwitchMode {get; set;}
    [SerializeField] int switchCnt; public int SwitchCnt {
        get => switchCnt;
        set {
            switchCnt = value;
            SwitchCntTxt.text = $"{switchCnt}";
            GM._.bossRwd.SetCurValueTxt(Enum.BossRwd.SwitchTowerPositionCnt, switchCnt);
        }
    }
    [field: SerializeField] public int FreeBoardCnt {get; set;}
    [field: SerializeField] public int FreeBreakRockCnt {get; set;}
    
    [SerializeField] int successionTicket; public int SuccessionTicket {
        get => successionTicket;
        set {
            successionTicket = value;
            SuccessionTicketCntTxt.text = $"{value}";
            GM._.bossRwd.SetCurValueTxt(Enum.BossRwd.SuccessionTicket, value);
        }
    }
    [SerializeField] int changeTypeTicket; public int ChangeTypeTicket {
        get => changeTypeTicket;
        set {
            changeTypeTicket = value;
            ChangeTypeTicketCntTxt.text = $"{value}";
            GM._.bossRwd.SetCurValueTxt(Enum.BossRwd.ChangeTypeTicket, value);
        }
    }

    [Header("UI")]
    public GameObject PanelObj;

    //* タワータイプ変更POPUP
    [field: SerializeField] public GameObject ChangeTypePopUp {get; set;}
    [field: SerializeField] public GameObject[] ChangeTypeCards {get; set;}

    //* 左 (情報 Vertical)
    [field: SerializeField] public TextMeshProUGUI CCTowerCntTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI SuccessionTicketCntTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI ChangeTypeTicketCntTxt {get; set;}
    //* 左 (アイコン Horizontal)
    [field: SerializeField] public Button SuccessionIconBtn {get; set;}
    [field: SerializeField] public Button ChangeTypeIconBtn {get; set;}
    [field: SerializeField] public TextMeshProUGUI SuccessionConsumeTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI ChangeTypeConsumeTxt {get; set;}

    //* 中央（アイコン）
    [field: SerializeField] public TextMeshProUGUI SwitchCntTxt {get; set;}
    [field: SerializeField] public Button[] IconBtns {get; set;}
    [field: SerializeField] public GameObject MergePossibleMark;
    [field: SerializeField] public Sprite MergeOffSpr {get; set;}
    [field: SerializeField] public Sprite MergeOnSpr {get; set;}

    private TextMeshProUGUI boardPriceTxt;
    private TextMeshProUGUI breakPriceTxt;

    void Start() {
        PanelObj.SetActive(false);
        SetCCTowerCntTxt(0);

        IsSwitchMode = false;
        SwitchCnt = 2;
        FreeBoardCnt = Config.FREE_BOARD_CNT;
        FreeBreakRockCnt = Config.FREE_BREAKROCK_CNT;
        SuccessionTicket = 0;
        ChangeTypeTicket = 0;
        SwitchCntTxt.text = SwitchCnt.ToString();
        SuccessionTicketCntTxt.text = SuccessionTicket.ToString();
        ChangeTypeTicketCntTxt.text = ChangeTypeTicket.ToString();

        //* アイコン初期化
        breakPriceTxt = IconBtns[(int)ICON.Break].GetComponentsInChildren<TextMeshProUGUI>()[ICON_PRICE_IDX];
        breakPriceTxt.text = $"<color=green>무료 {Config.FREE_BREAKROCK_CNT}</color>";
        boardPriceTxt = IconBtns[(int)ICON.Board].GetComponentsInChildren<TextMeshProUGUI>()[ICON_PRICE_IDX];
        boardPriceTxt.text = $"<color=green>무료 {Config.FREE_BOARD_CNT}</color>";
        IconBtns[(int)ICON.Tower].GetComponentsInChildren<TextMeshProUGUI>()[ICON_PRICE_IDX].text = $"{Config.G_PRICE.TOWER}";
        IconBtns[(int)ICON.IceTower].GetComponentsInChildren<TextMeshProUGUI>()[ICON_PRICE_IDX].text = $"{Config.G_PRICE.CCTOWER}";
        IconBtns[(int)ICON.ThunderTower].GetComponentsInChildren<TextMeshProUGUI>()[ICON_PRICE_IDX].text = $"{Config.G_PRICE.CCTOWER}";
        IconBtns[(int)ICON.Upgrade].GetComponentsInChildren<TextMeshProUGUI>()[ICON_PRICE_IDX].text = $"{Config.G_PRICE.CC_UPG}";
        IconBtns[(int)ICON.Merge].GetComponentsInChildren<TextMeshProUGUI>()[ICON_PRICE_IDX].text = $"{Config.G_PRICE.MERGE}";
        IconBtns[(int)ICON.Delete].GetComponentsInChildren<TextMeshProUGUI>()[ICON_PRICE_IDX].text = $"{(1 - Config.G_PRICE.DELETE_REFUND_PER) * 100}%";
    }

#region EVENT BUTTON
    public void OnClickSuccessionIconBtn() {
        Tower tower = GM._.tmc.HitObject.GetComponentInChildren<Tower>();

        //* チケット減る
        SuccessionTicket -= tower.Lv;

        //* 同じタイプのタワーにマージを固定
        bool isSuccess = MergeTower(tower.Kind);

        if(isSuccess) {
            GM._.gui.ShowMsgNotice("타입계승 합성 완료!");
        }
        else {
            GM._.gui.ShowMsgError("합성할 같은 타워가 없습니다.");
            return;
        }

        UpdateUI(Enum.Layer.Tower);
    }

    public void OnClickChangeTypeIconBtn() {
        GM._.gui.Pause();
        ChangeTypePopUp.SetActive(true); //* 表示

        //* 変更するタイプカード 表示 (自分と同じタイプは非表示)
        Tower tower = GM._.tmc.HitObject.GetComponentInChildren<Tower>();
        for(int i = 0; i < ChangeTypeCards.Length; i++)
            ChangeTypeCards[i].SetActive((int)tower.Kind != i);
    }
    public void OnClickChangeTypePopUpCancelBtn() {
        GM._.gui.Play();
        ChangeTypePopUp.SetActive(false); //* 非表示
    }
    public void OnClickChangeTypeCard(int kindIdx) {
        GM._.gui.Play();
        Tower tower = GM._.tmc.HitObject.GetComponentInChildren<Tower>();

        //* チケット減る
        ChangeTypeTicket -= tower.Lv;

        tower.Lv--;
        Debug.Log($"OnClickChangeTypeCard():: kindIdx= {kindIdx}, tower.Lv= {tower.Lv}");

        SM._.SfxPlay(SM.SFX.InvEquipSFX);

        //* タワーのタイプ変更
        switch(kindIdx) {
            case (int)TowerKind.Warrior:
                //* 自分を削除
                GM._.tm.CreateTower(TowerType.Random, tower.Lv, TowerKind.Warrior);
                DestroyImmediate(tower.gameObject);
                break;
            case (int)TowerKind.Archer:
                GM._.tm.CreateTower(TowerType.Random, tower.Lv, TowerKind.Archer);
                DestroyImmediate(tower.gameObject);
                break;
            case (int)TowerKind.Magician:
                GM._.tm.CreateTower(TowerType.Random, tower.Lv, TowerKind.Magician);
                DestroyImmediate(tower.gameObject);
                break;
        }

        ChangeTypePopUp.SetActive(false); //* 非表示

        GM._.gui.ShowMsgNotice("타입 변경 완료!");

        UpdateUI(Enum.Layer.Tower);
    }

    public void OnClickBreakIconBtn() {
        if(GM._.gui.ShowErrMsgCreateTowerAtPlayState())
            return;
        else if(FreeBreakRockCnt > 0) { //* 無料カウント
            FreeBreakRockCnt--;
            breakPriceTxt.text = $"<color=green>무료 {FreeBreakRockCnt}</color>";
            //* 全部使ったら、値段表示
            if(FreeBreakRockCnt <= 0)
                breakPriceTxt.text = $"{Config.G_PRICE.BREAK}";
        }
        else if(!GM._.CheckMoney(Config.G_PRICE.BREAK))
            return;

        SM._.SfxPlay(SM.SFX.BreakSFX);
        GM._.tmc.BreakWallTile();
        GM._.tmc.SelectedTileMap.ClearAllTiles();
        PanelObj.SetActive(false);
    }

    public void OnClickBoardIconBtn() {
        if(GM._.gui.ShowErrMsgCreateTowerAtPlayState())
            return;
        //* 無料カウント減る
        else if(FreeBoardCnt > 0) { 
            FreeBoardCnt--;
            boardPriceTxt.text = $"<color=green>무료 {FreeBoardCnt}</color>";
            //* 全部使ったら、値段表示
            if(FreeBoardCnt <= 0)
                boardPriceTxt.text = $"{Config.G_PRICE.BOARD}";
        }
        else if(!GM._.CheckMoney(Config.G_PRICE.BOARD))
            return;

        SM._.SfxPlay(SM.SFX.BuildSFX);
        GM._.tm.InstallBoard(); // ボード生成
        GM._.gui.InActiveResetWallBtn();
        StartCoroutine(CoCheckPathFind(Enum.Layer.Board));
    }

    public void OnClickRandomTowerIconBtn() {
        if(!GM._.CheckMoney(Config.G_PRICE.TOWER))
            return;

        SM._.SfxPlay(SM.SFX.CreateTowerSFX);
        GM._.tm.CreateTower(TowerType.Random); // CCタワー生成

        UpdateUI(Enum.Layer.Tower);
    }

    public void OnClickIceTowerIconBtn() {
        if(GM._.gui.ShowErrMsgCreateTowerAtPlayState())
            return;
        else if(GM._.gui.ShowErrMsgCCTowerLimit())
            return;
        else if(!GM._.CheckMoney(Config.G_PRICE.CCTOWER)) // CCタワー生成
            return;

        SetCCTowerCntTxt(+1);
        SM._.SfxPlay(SM.SFX.BuildSFX);
        GM._.tm.CreateTower(TowerType.CC_IceTower);
        GM._.gui.InActiveResetWallBtn();
        StartCoroutine(CoCheckPathFind(Enum.Layer.CCTower));
    }

    public void OnClickStunTowerIconBtn() {
        if(GM._.gui.ShowErrMsgCreateTowerAtPlayState())
            return;
        else if(GM._.gui.ShowErrMsgCCTowerLimit())
            return;
        else if(!GM._.CheckMoney(Config.G_PRICE.CCTOWER))
            return;

        SetCCTowerCntTxt(+1);
        SM._.SfxPlay(SM.SFX.BuildSFX);
        GM._.tm.CreateTower(TowerType.CC_StunTower);
        GM._.gui.InActiveResetWallBtn();
        StartCoroutine(CoCheckPathFind(Enum.Layer.CCTower));
    }

    public void OnClickUpgradeIconBtn() {
        Debug.Log($"OnClickUpgradeIconBtn():: HitObject= {GM._.tmc.HitObject}");

        if(!GM._.CheckMoney(Config.G_PRICE.CC_UPG))
            return;

        SM._.SfxPlay(SM.SFX.UpgradeSFX);
        Tower tower = GM._.tmc.HitObject.GetComponentInChildren<Tower>();
        switch(tower.Type) {
            case TowerType.CC_IceTower:
                var iceTower = tower as IceTower;
                iceTower.Upgrade();
                break;
            case TowerType.CC_StunTower:
                var stunTower = tower as StunTower;
                stunTower.Upgrade();
                break;
        }
        UpdateUI(Enum.Layer.CCTower);
    }

    public void OnClickMergeIconBtn() {
        if(!GM._.CheckMoney(Config.G_PRICE.MERGE))
            return;

        //* マージ
        bool isSuccess = MergeTower();

        if(isSuccess) {
            
            GM._.gui.ShowMsgNotice("합성 완료!");

            //* Reset MergableManager Data
            GM._.mgb.Init();
        }
        else {
            GM._.gui.ShowMsgError("합성할 같은 타워가 없습니다.");
            GM._.SetMoney(Config.G_PRICE.MERGE);
            return;
        }

        UpdateUI(Enum.Layer.Tower);
    }

    public void OnClickSwitchIconBtn() {
        if(SwitchCnt > 0) {
            IsSwitchMode = true;
            GM._.gui.ShowMsgInfo(isActive: true, "위치를 바꿀 타워를 선택해주세요!");

            //* 現在の選択したObjectを保存
            GM._.tmc.SwitchBefHitObject = GM._.tmc.HitObject;

            UpdateUI(Enum.Layer.SwitchTower);
        }
        else {
            GM._.gui.ShowMsgError("위치변경을 전부 사용했습니다.");
        }
    }

    public void OnClickDeleteIconBtn() {
        SM._.SfxPlay(SM.SFX.DeleteTowerSFX);
        GM._.tmc.DeleteTile();
    }

    public void OnClickExitIconBtn() {
        GM._.tmc.SelectedTileMap.ClearAllTiles();
        PanelObj.SetActive(false);
        GM._.tmc.Reset();

        //* 位置変更モードキャンセルなら
        if(IsSwitchMode) {
            SwitchModeOff();
        }
    }
#endregion

#region FUNC
    /// <summary>
    /// タイルの設置が完了するまで待ち、次のフレームで道が詰まったことを確認
    /// </summary>
    IEnumerator CoCheckPathFind(int layer) {
        //* (BUG) 待機時間がないと、BoardとCCタワーが設置する前に、経路を探索する間違いになる
        yield return Util.Time0_1;

        //* 道が詰まるエラー
        if(!GM._.pfm.PathFinding()) {
            GM._.gui.ShowMsgError("길을 막으면 안됩니다!");

            //* ボードなら
            if(layer == Enum.Layer.Board) {
                if(FreeBoardCnt >= 0) { // 無料カウントが有ったら
                    FreeBoardCnt++;
                    boardPriceTxt.text = $"<color=green>무료 {FreeBoardCnt}</color>";
                }
                else {
                    GM._.SetMoney(Config.G_PRICE.BOARD); // 返金
                }
            }
            //* CCTowerなら
            else if(layer == Enum.Layer.CCTower) {
                GM._.SetMoney(Config.G_PRICE.CCTOWER); // 返金
                SetCCTowerCntTxt(-1); 
            }
            
            Destroy(GM._.tmc.HitObject);
        }
        //* アクションバー切り替え
        else {
            //! (BUG) ボードの道が詰まった場合、無料ボード回数が０なら、＋１増える
            if(FreeBoardCnt == 0)
                FreeBoardCnt = -1; //* 全て使ったことが確認できたら、-1にして無料カウント増えないようにする

            //* 新しい経路表示
            GM._.pfm.PathFinding(isShowPath: true);
            //* 表示
            UpdateUI(layer);
            GM._.tmc.SelectLayer = layer;
            //* ビルドEF
            GM._.gef.ShowEF(GameEF.BuildTowerEF, (Vector3)GM._.tmc.getCurSelectedPos());
        }
    }

    private void clearIcons() {
        for(int i = 0; i < IconBtns.Length - 1; i++)
            IconBtns[i].gameObject.SetActive(false);
    }

    public void SwitchModeOff() {
        IsSwitchMode = false;
        GM._.tmc.SwitchBefHitObject = null;
        GM._.gui.ShowMsgInfo(isActive: false);
    }

    public void SetCCTowerCntTxt(int val) {
        GM._.tm.CCTowerCnt += val;
        CCTowerCntTxt.text = $"CC : {GM._.tm.CCTowerCnt}/{GM._.tm.CCTowerMax}";
        //* Maxになったら、赤い文字にする
        CCTowerCntTxt.color = (GM._.tm.CCTowerCnt == GM._.tm.CCTowerMax)? Color.red : Color.white;
    }

    private bool MergeTower(TowerKind kind = TowerKind.None) {
        bool isSuccess = false;
        Tower tower = GM._.tmc.HitObject.GetComponentInChildren<Tower>();
        //* タワーのタイプによってマージ
        switch(tower.Kind) {
            case TowerKind.Warrior:
                var warrior = tower as WarriorTower;
                isSuccess = warrior.Merge(kind);
                break;
            case TowerKind.Archer:
                var archer = tower as ArcherTower;
                isSuccess = archer.Merge(kind);
                break;
            case TowerKind.Magician:
                var magician = tower as MagicianTower;
                isSuccess = magician.Merge(kind);
                break;
        }
        //* サウンド
        if(isSuccess) {
            switch(tower.Lv) {
                case 1: SM._.SfxPlay(SM.SFX.Merge1SFX); break;
                case 2: SM._.SfxPlay(SM.SFX.Merge2SFX); break;
                case 3: SM._.SfxPlay(SM.SFX.Merge3SFX); break;
                case 4: SM._.SfxPlay(SM.SFX.Merge4SFX); break;
                case 5: SM._.SfxPlay(SM.SFX.Merge5SFX); break;
            }
        }

        return isSuccess;
    }

    /// <summary>
    /// アクションバーのアイコン表示
    /// </summary>
    /// <param name="layer">選択したタイルのレイアタイプ</param>
    public void UpdateUI(int layer) {
        //* リセット
        clearIcons();
        GM._.gui.tsm.WindowObj.SetActive(false);
        SuccessionIconBtn.gameObject.SetActive(false);
        ChangeTypeIconBtn.gameObject.SetActive(false);

        //* 表示
        switch(layer) {
            case Enum.Layer.Wall: {
                IconBtns[(int)ICON.Break].gameObject.SetActive(true);
                break;
            }
            case Enum.Layer.Board: {
                IconBtns[(int)ICON.Tower].gameObject.SetActive(true);
                IconBtns[(int)ICON.Delete].gameObject.SetActive(true);
                IconBtns[(int)ICON.Delete].GetComponentsInChildren<TextMeshProUGUI>()[ICON_PRICE_IDX].text = $"100%";
                IconBtns[(int)ICON.Switch].gameObject.SetActive(true);
                break;
            }
            case Enum.Layer.CCTower: {
                //* タワー情報UI 表示
                Tower tower = GM._.tmc.HitObject.GetComponent<Tower>();
                GM._.gui.tsm.ShowTowerStateUI(tower.InfoState());

                //* MaxLv チェック
                bool isMaxLv = false;
                switch (tower.Type) {
                    case TowerType.CC_IceTower:
                        isMaxLv = tower.Lv >= GM._.tm.IceTowers.Length;
                        break;
                    case TowerType.CC_StunTower:
                        isMaxLv = tower.Lv >= GM._.tm.StunTowers.Length;
                        break;
                }

                //* アイコン表示
                IconBtns[(int)ICON.Upgrade].gameObject.SetActive(!isMaxLv);
                IconBtns[(int)ICON.Delete].gameObject.SetActive(true);
                IconBtns[(int)ICON.Delete].GetComponentsInChildren<TextMeshProUGUI>()[ICON_PRICE_IDX].text = $"{Config.G_PRICE.DELETE_REFUND_PER * 100}%";
                IconBtns[(int)ICON.Switch].gameObject.SetActive(true);
                break;
            }
            case Enum.Layer.Tower: {
                GM._.mgb.Mergable();

                //* タワー情報UI 表示
                Tower tower = GM._.tmc.HitObject.GetComponentInChildren<Tower>();
                Debug.Log($"HitObject.name= {GM._.tmc.HitObject}, tower.name= {tower.name}");
                GM._.gui.tsm.ShowTowerStateUI(tower.InfoState());

                //* MaxLv チェック
                bool isMaxLv = false;

                //* マージUIアイコン 初期化
                Image iconImg = IconBtns[(int)ICON.Merge].GetComponent<Image>();
                iconImg.sprite = MergeOffSpr;
                MergePossibleMark.SetActive(false);

                switch(tower.Kind) {
                    case TowerKind.Warrior:
                        isMaxLv = tower.Lv >= GM._.tm.Warriors.Length;
                        var warrior = tower as WarriorTower;
                        warrior.CheckMergeUI();
                        break;
                    case TowerKind.Archer:
                        isMaxLv = tower.Lv >= GM._.tm.Archers.Length;
                        var archer = tower as ArcherTower;
                        archer.CheckMergeUI();
                        break;
                    case TowerKind.Magician:
                        isMaxLv = tower.Lv >= GM._.tm.Magicians.Length;
                        var magician = tower as MagicianTower;
                        magician.CheckMergeUI();
                        break;
                }

                //* アイコン表示
                bool isTowerLvMax = tower.Lv >= TowerManager.RANDOM_TOWER_LV_MAX;
                SuccessionIconBtn.gameObject.SetActive(SuccessionTicket >= tower.Lv && !isTowerLvMax);
                SuccessionTicketCntTxt.text = $"{SuccessionTicket}";
                SuccessionConsumeTxt.text = $"{tower.Lv}개 소비";
                ChangeTypeIconBtn.gameObject.SetActive(ChangeTypeTicket >= tower.Lv && !isTowerLvMax);
                ChangeTypeTicketCntTxt.text = $"{ChangeTypeTicket}";
                ChangeTypeConsumeTxt.text = $"{tower.Lv}개 소비";
                IconBtns[(int)ICON.Merge].gameObject.SetActive(!isMaxLv);
                IconBtns[(int)ICON.Delete].gameObject.SetActive(tower.Lv < 5); //* レベル５以上は削除できないように
                IconBtns[(int)ICON.Delete].GetComponentsInChildren<TextMeshProUGUI>()[ICON_PRICE_IDX].text = $"{Config.G_PRICE.DELETE_REFUND_PER * 100}%";
                IconBtns[(int)ICON.Switch].gameObject.SetActive(true);
                break;
            }
            case Enum.Layer.SwitchTower:

                break;
            default: {
                IconBtns[(int)ICON.Board].gameObject.SetActive(true);
                IconBtns[(int)ICON.IceTower].gameObject.SetActive(true);
                IconBtns[(int)ICON.ThunderTower].gameObject.SetActive(true);
                break;
            }
        }
    }
#endregion
}
