using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ActionBarUIManager : MonoBehaviour {
    public enum ICON {
        Break, Board, Tower, IceTower, ThunderTower, Upgrade, Merge, Switch, Delete, Exit
    };

    //* Value
    [field: SerializeField] public bool IsSwitchMode {get; set;}
    [field: SerializeField] public int SwitchCnt {get; set;}
    [field: SerializeField] public int SuccessionTicket {get; set;}
    [field: SerializeField] public int ChangeTypeTicket {get; set;}

    [Header("UI")]
    public GameObject PanelObj;
    //* 左
    [field: SerializeField] public TextMeshProUGUI CCTowerCntTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI SuccessionTicketCntTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI ChangeTypeTicketCntTxt {get; set;}
    //* 中央
    [field: SerializeField] public TextMeshProUGUI SwitchCntTxt {get; set;}
    [field: SerializeField] public Button[] IconBtns {get; set;}
    [field: SerializeField] public Sprite MergeOffSpr {get; set;}
    [field: SerializeField] public Sprite MergeOnSpr {get; set;}

    void Start() {
        PanelObj.SetActive(false);
        SetCCTowerCntTxt(0);

        IsSwitchMode = false;
        SwitchCnt = 2;
        SuccessionTicket = 0;
        ChangeTypeTicket = 0;
        SwitchCntTxt.text = SwitchCnt.ToString();
        SuccessionTicketCntTxt.text = SuccessionTicket.ToString();
        ChangeTypeTicketCntTxt.text = ChangeTypeTicket.ToString();

        const int PRICE_IDX = 1;
        IconBtns[(int)ICON.Break].GetComponentsInChildren<TextMeshProUGUI>()[PRICE_IDX].text = $"{Config.PRICE.BREAK}";
        IconBtns[(int)ICON.Board].GetComponentsInChildren<TextMeshProUGUI>()[PRICE_IDX].text = $"{Config.PRICE.BOARD}";
        IconBtns[(int)ICON.Tower].GetComponentsInChildren<TextMeshProUGUI>()[PRICE_IDX].text = $"{Config.PRICE.TOWER}";
        IconBtns[(int)ICON.IceTower].GetComponentsInChildren<TextMeshProUGUI>()[PRICE_IDX].text = $"{Config.PRICE.CCTOWER}";
        IconBtns[(int)ICON.ThunderTower].GetComponentsInChildren<TextMeshProUGUI>()[PRICE_IDX].text = $"{Config.PRICE.CCTOWER}";
        IconBtns[(int)ICON.Upgrade].GetComponentsInChildren<TextMeshProUGUI>()[PRICE_IDX].text = $"{Config.PRICE.CC_UPG}";
        IconBtns[(int)ICON.Merge].GetComponentsInChildren<TextMeshProUGUI>()[PRICE_IDX].text = $"{Config.PRICE.MERGE}";
        IconBtns[(int)ICON.Delete].GetComponentsInChildren<TextMeshProUGUI>()[PRICE_IDX].text = $"{(1 - Config.PRICE.DELETE_REFUND_PER) * 100}%";
    }

#region EVENT BUTTON
    public void OnClickBreakIconBtn() {
        if(GM._.gui.ShowErrMsgCreateTowerAtPlayState())
            return;
        else if(!GM._.CheckMoney(Config.PRICE.BREAK))
            return;

        GM._.tmc.BreakWallTile();
        GM._.tmc.SelectedTileMap.ClearAllTiles();
        PanelObj.SetActive(false);
    }

    public void OnClickBoardIconBtn() {
        if(GM._.gui.ShowErrMsgCreateTowerAtPlayState())
            return;
        else if(!GM._.CheckMoney(Config.PRICE.BOARD))
            return;

        GM._.tm.InstallBoard();
        StartCoroutine(CoCheckPathFind(Enum.Layer.Board));
    }

    public void OnClickRandomTowerIconBtn() {
        if(!GM._.CheckMoney(Config.PRICE.TOWER))
            return;

        GM._.tm.CreateTower(TowerType.Random);
        UpdateUI(Enum.Layer.Tower);
    }

    public void OnClickIceTowerIconBtn() {
        if(GM._.gui.ShowErrMsgCreateTowerAtPlayState())
            return;
        else if(GM._.gui.ShowErrMsgCCTowerLimit())
            return;
        else if(!GM._.CheckMoney(Config.PRICE.CCTOWER))
            return;

        SetCCTowerCntTxt(+1);
        GM._.tm.CreateTower(TowerType.CC_IceTower);
        StartCoroutine(CoCheckPathFind(Enum.Layer.CCTower));
    }

    public void OnClickStunTowerIconBtn() {
        if(GM._.gui.ShowErrMsgCreateTowerAtPlayState())
            return;
        else if(GM._.gui.ShowErrMsgCCTowerLimit())
            return;
        else if(!GM._.CheckMoney(Config.PRICE.CCTOWER))
            return;

        SetCCTowerCntTxt(+1);
        GM._.tm.CreateTower(TowerType.CC_StunTower);
        StartCoroutine(CoCheckPathFind(Enum.Layer.CCTower));
    }

    public void OnClickUpgradeIconBtn() {
        Debug.Log($"OnClickUpgradeIconBtn():: HitObject= {GM._.tmc.HitObject}");

        if(!GM._.CheckMoney(Config.PRICE.CC_UPG))
            return;

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
        if(!GM._.CheckMoney(Config.PRICE.MERGE))
            return;

        bool isSuccess = false;
        Tower tower = GM._.tmc.HitObject.GetComponentInChildren<Tower>();
        //* タワーのタイプによってマージ
        switch(tower.Kind) {
            case TowerKind.Warrior:
                var warrior = tower as WarriorTower;
                isSuccess = warrior.Merge();
                break;
            case TowerKind.Archer:
                var archer = tower as ArcherTower;
                isSuccess = archer.Merge();
                break;
            case TowerKind.Magician:
                var magician = tower as MagicianTower;
                isSuccess = magician.Merge();
                break;
        }
        //* エラーメッセージ
        if(!isSuccess) {
            GM._.gui.ShowMsgError("합성할 같은 타워가 없습니다.");
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
            GM._.gui.StartCoroutine("위치변경을 전부 사용했습니다.");
        }
    }

    public void OnClickDeleteIconBtn() {
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
        yield return null;
        //* 道が詰まるエラー
        if(!GM._.pfm.PathFinding()) {
            GM._.gui.StartCoroutine("길을 막으면 안됩니다!");
            //* タイル除去
            SetCCTowerCntTxt(-1);
            Destroy(GM._.tmc.HitObject);
        }
        //* アクションバー切り替え
        else {
            //* 表示
            UpdateUI(layer);
            GM._.tmc.SelectLayer = layer;
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

    /// <summary>
    /// アクションバーのアイコン表示
    /// </summary>
    /// <param name="layer">選択したタイルのレイアタイプ</param>
    public void UpdateUI(int layer) {
        //* リセット
        clearIcons();
        GM._.gui.tsm.WindowObj.SetActive(false);

        //* 表示
        switch(layer) {
            case Enum.Layer.Wall: {
                IconBtns[(int)ICON.Break].gameObject.SetActive(true);
                break;
            }
            case Enum.Layer.Board: {
                IconBtns[(int)ICON.Tower].gameObject.SetActive(true);
                IconBtns[(int)ICON.Delete].gameObject.SetActive(true);
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
                IconBtns[(int)ICON.Switch].gameObject.SetActive(true);
                break;
            }
            case Enum.Layer.Tower: {
                //* タワー情報UI 表示
                Tower tower = GM._.tmc.HitObject.GetComponentInChildren<Tower>();
                Debug.Log($"HitObject.name= {GM._.tmc.HitObject}, tower.name= {tower.name}");
                GM._.gui.tsm.ShowTowerStateUI(tower.InfoState());

                //* MaxLv チェック
                bool isMaxLv = false;
                switch (tower.Kind) {
                    case TowerKind.Warrior:
                        isMaxLv = tower.Lv >= GM._.tm.Warriors.Length;
                        break;
                    case TowerKind.Archer:
                        isMaxLv = tower.Lv >= GM._.tm.Archers.Length;
                        break;
                    case TowerKind.Magician:
                        isMaxLv = tower.Lv >= GM._.tm.Magicians.Length;
                        break;
                }

                //* マージ可能UI 表示
                Image iconImg = IconBtns[(int)ICON.Merge].GetComponent<Image>();
                iconImg.sprite = MergeOffSpr;
                switch(tower.Kind) {
                    case TowerKind.Warrior:
                        var warrior = tower as WarriorTower;
                        warrior.CheckMergeUI();
                        break;
                    case TowerKind.Archer:
                        var archer = tower as ArcherTower;
                        archer.CheckMergeUI();
                        break;
                    case TowerKind.Magician:
                        var magician = tower as MagicianTower;
                        magician.CheckMergeUI();
                        break;
                }

                //* アイコン表示
                IconBtns[(int)ICON.Merge].gameObject.SetActive(!isMaxLv);
                IconBtns[(int)ICON.Delete].gameObject.SetActive(true);
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
