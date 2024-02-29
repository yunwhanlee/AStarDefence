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
    [field: SerializeField] public int CCTowerMax {get; private set;}
    [field: SerializeField] public int CCTowerCnt {get; private set;}
    [field: SerializeField] public bool IsSwitchMode {get; set;}
    [field: SerializeField] public int SwitchCnt {get; set;}

    [Header("UI")]
    public GameObject PanelObj;
    [field: SerializeField] public TextMeshProUGUI CCTowerCntTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI SwitchCntTxt {get; set;}
    [field: SerializeField] public Button[] IconBtns {get; set;}
    [field: SerializeField] public Sprite MergeOffSpr {get; set;}
    [field: SerializeField] public Sprite MergeOnSpr {get; set;}

    void Start() {
        PanelObj.SetActive(false);
        SetCCTowerCntTxt(0);

        IsSwitchMode = false;
        CCTowerMax = 5;
        SwitchCnt = 2;
        SwitchCntTxt.text = SwitchCnt.ToString();
    }

#region EVENT BUTTON
    public void OnClickBreakIconBtn() {
        if(GM._.gui.ShowErrMsgCreateTowerAtPlayState())
            return;
        GM._.tmc.BreakWallTile();
        GM._.tmc.SelectedTileMap.ClearAllTiles();
        PanelObj.SetActive(false);
    }
    public void OnClickBoardIconBtn() {
        if(GM._.gui.ShowErrMsgCreateTowerAtPlayState())
            return;
        GM._.tm.InstallBoard();
        StartCoroutine(CoCheckPathFind(Enum.Layer.Board));
    }
    public void OnClickRandomTowerIconBtn() {
        if(GM._.gui.ShowErrMsgCreateTowerAtPlayState())
            return;
        GM._.tm.CreateTower(TowerType.Random);
        UpdateUI(Enum.Layer.Tower);
    }
    public void OnClickIceTowerIconBtn() {
        if(GM._.gui.ShowErrMsgCreateTowerAtPlayState())
            return;
        else if(GM._.gui.ShowErrMsgCCTowerLimit())
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
        SetCCTowerCntTxt(+1);
        GM._.tm.CreateTower(TowerType.CC_StunTower);
        StartCoroutine(CoCheckPathFind(Enum.Layer.CCTower));
    }
    public void OnClickUpgradeIconBtn() {
        Debug.Log($"OnClickUpgradeIconBtn():: HitObject= {GM._.tmc.HitObject}");
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
            StartCoroutine(GM._.gui.CoShowMsgError("합성할 같은 타워가 없습니다."));
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
            StartCoroutine(GM._.gui.CoShowMsgError("위치변경을 전부 사용했습니다."));
        }
    }
    public void OnClickDeleteIconBtn() {
        GM._.tmc.DeleteTile();
        GM._.tmc.SelectedTileMap.ClearAllTiles();
        PanelObj.SetActive(false);
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
            StartCoroutine(GM._.gui.CoShowMsgError("길을 막으면 안됩니다!"));
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
        CCTowerCnt += val;
        CCTowerCntTxt.text = $"CC : {CCTowerCnt}/{CCTowerMax}";
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
