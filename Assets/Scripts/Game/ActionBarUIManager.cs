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

    #region FUNC
        private void clearIcons() {
            for(int i = 0; i < IconBtns.Length - 1; i++) {
                IconBtns[i].gameObject.SetActive(false);
            }
        }

        public void activeIconsByLayer(string layerName) {
            //* リセット
            clearIcons();

            //* 表示
            if(layerName == "Land") {
                IconBtns[(int)ICON.Board].gameObject.SetActive(true);
                IconBtns[(int)ICON.IceTower].gameObject.SetActive(true);
                IconBtns[(int)ICON.ThunderTower].gameObject.SetActive(true);
                IconBtns[(int)ICON.Delete].gameObject.SetActive(true);
            }
            else if(layerName == Enum.Layer.Wall.ToString()) {
                IconBtns[(int)ICON.Break].gameObject.SetActive(true);
            }
            else if(layerName == Enum.Layer.Board.ToString()) {
                IconBtns[(int)ICON.Break].gameObject.SetActive(true);
                IconBtns[(int)ICON.Delete].gameObject.SetActive(true);
            }
            else if(layerName == Enum.Layer.Tower.ToString()) {
            }
        }
    #endregion
}
