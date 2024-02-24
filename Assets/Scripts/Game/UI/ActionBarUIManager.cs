using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarUIManager : MonoBehaviour {
    public enum ICON {
        Break, Board, Tower, IceTower, ThunderTower, Upgrade, Merge, Delete, Exit
    };

    [field: SerializeField] public int CCTowerMax {get; private set;} = 5;
    [field: SerializeField] public int CCTowerCnt {get; private set;}

    [Header("UI")]
    public GameObject PanelObj;
    [field: SerializeField] public TextMeshProUGUI CCTowerCntTxt {get; set;}
    [field: SerializeField] public Button[] IconBtns {get; set;}

    void Start() {
        PanelObj.SetActive(false);
        SetCCTowerCntTxt(0);
    }

#region EVENT BUTTON
    public void onClickBoardIconBtn() {
        GM._.tmc.InstallBoardTile();
        StartCoroutine(CoCheckPathFind(Enum.Layer.Board));
    }
    public void onClickRandomTowerIconBtn() {
        GM._.tm.CreateTower(TowerType.Random);
    }
    public void onClickIceTowerIconBtn() {
        if(CCTowerCnt >= CCTowerMax) {
            StartCoroutine(GM._.gui.CoShowMsgError($"CC타워는 {CCTowerMax}개까지 가능합니다."));
            return;
        }
        SetCCTowerCntTxt(+1);
        GM._.tm.CreateTower(TowerType.CC_IceTower);
        StartCoroutine(CoCheckPathFind(Enum.Layer.CCTower));
    }
    public void onClickStunTowerIconBtn() {
        if(CCTowerCnt >= CCTowerMax) {
            StartCoroutine(GM._.gui.CoShowMsgError($"CC타워는 {CCTowerMax}개까지 가능합니다."));
            return;
        }
        SetCCTowerCntTxt(+1);
        GM._.tm.CreateTower(TowerType.CC_StunTower);
        StartCoroutine(CoCheckPathFind(Enum.Layer.CCTower));
    }
    public void onClickDeleteIconBtn() {
        GM._.tmc.DeleteTile();
        GM._.tmc.SelectedTileMap.ClearAllTiles();
        PanelObj.SetActive(false);
    }
    public void onClickExitIconBtn() {
        GM._.tmc.SelectedTileMap.ClearAllTiles();
        PanelObj.SetActive(false);
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
            ActiveIconsByLayer(layer);
            GM._.tmc.SelectLayer = layer;
        }
    }

    private void clearIcons() {
        for(int i = 0; i < IconBtns.Length - 1; i++)
            IconBtns[i].gameObject.SetActive(false);
    }

    public void SetCCTowerCntTxt(int val) {
        CCTowerCnt += val;
        CCTowerCntTxt.text = $"CC : {CCTowerCnt}/{CCTowerMax}";
    }

    /// <summary>
    /// アクションバーのアイコン表示
    /// </summary>
    /// <param name="layer">選択したタイルのレイアタイプ</param>
    public void ActiveIconsByLayer(int layer) {
        //* リセット
        clearIcons();

        //* 表示
        switch(layer) {
            case Enum.Layer.Wall:
                IconBtns[(int)ICON.Break].gameObject.SetActive(true);
                break;
            case Enum.Layer.Board:
                IconBtns[(int)ICON.Tower].gameObject.SetActive(true);
                IconBtns[(int)ICON.Delete].gameObject.SetActive(true);                
                break;
            case Enum.Layer.CCTower:
                IconBtns[(int)ICON.Upgrade].gameObject.SetActive(true);
                IconBtns[(int)ICON.Delete].gameObject.SetActive(true);
                break;
            default:
                IconBtns[(int)ICON.Board].gameObject.SetActive(true);
                IconBtns[(int)ICON.IceTower].gameObject.SetActive(true);
                IconBtns[(int)ICON.ThunderTower].gameObject.SetActive(true);
                break;
        }
    }
#endregion
}
