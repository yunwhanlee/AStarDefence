using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarUIManager : MonoBehaviour {
    public enum ICON {
        Break, Board, Tower, IceTower, ThunderTower, Upgrade, Merge, Delete, Exit
    };
    [field: SerializeField] public GameObject PanelObj {get; set;}
    [field: SerializeField] public Button[] IconBtns {get; set;}

    void Start() {
        PanelObj.SetActive(false);
    }

#region EVENT BUTTON
    public void onClickBoardIconBtn() {
        GM._.tmc.InstallBoard();
        StartCoroutine(CoCheckPathFind());
    }
    public void onClickRandomTowerIconBtn() {
        GM._.tm.CreateTower(TowerType.Random);
    }
    public void onClickIceTowerIconBtn() {
        GM._.tm.CreateTower(TowerType.CC_IceTower);
    }
    public void onClickStunTowerIconBtn() {
        GM._.tm.CreateTower(TowerType.CC_StunTower);
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
    IEnumerator CoCheckPathFind() {
        yield return null;
        //* 道が詰まったら
        if(!GM._.pfm.PathFinding()) {
            StartCoroutine(GM._.gui.CoShowMsgError("길을 막으면 안됩니다!"));
            var pos = new Vector3Int(GM._.tmc.CurSelectPos.y, GM._.tmc.CurSelectPos.x, 0);
            GM._.tmc.BoardTileMap.SetTile(pos, null);
        }
        //* Boardタイル選択後に進む
        else
            ActiveIconsByLayer(Enum.Layer.Board);
    }

    private void clearIcons() {
        for(int i = 0; i < IconBtns.Length - 1; i++)
            IconBtns[i].gameObject.SetActive(false);
    }

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
                IconBtns[(int)ICON.Delete].gameObject.SetActive(true);
                break;
        }
    }
#endregion
}
