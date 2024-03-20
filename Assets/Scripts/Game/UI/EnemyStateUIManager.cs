using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EnemyInfoUI {
    [field: SerializeField] public GameObject Obj {get; private set;}
    [field: SerializeField] public Image IconFrame {get; set;}
    [field: SerializeField] public Image IconImg {get; set;}
    [field: SerializeField] public TextMeshProUGUI LvTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI NameTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI HpTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI SpeedTxt {get; set;}

    public void SetUI(EnemyData enemy, Sprite[] FrameSprs) {
        Debug.Log($"ShowEnemyStateUI():: SetUI()::");
        const int FRAME_GRAY = 0, FRAME_BLUE = 1, FRAME_RED = 2, FRAME_YELLOW = 3;
        EnemyType type = enemy.Type;

        //* フレームイメージ
        IconFrame.sprite = (type == EnemyType.Land)? FrameSprs[FRAME_GRAY]
            : (type == EnemyType.Flight)? FrameSprs[FRAME_BLUE]
            : (type == EnemyType.Boss)? FrameSprs[FRAME_RED]
            : (type == EnemyType.Goblin)? FrameSprs[FRAME_YELLOW] : null;

        //* 敵イメージ
        IconImg.sprite = enemy.Spr;

        //* 情報表示
        LvTxt.text = $"LV {enemy.Lv}";
        NameTxt.text = $"{enemy.Name}";
        HpTxt.text = $"{enemy.Hp}";
        SpeedTxt.text = $"{enemy.Speed}";

        Obj.SetActive(true);
    }
}

public class EnemyStateUIManager : MonoBehaviour {
    [field: SerializeField] public Sprite[] FrameSprs {get; set;}

    [field: SerializeField] public EnemyInfoUI CurEnemyInfoWindowUI;
    [field: SerializeField] public EnemyInfoUI NextEnemyInfoPopUpUI;

#region EVENT
    public void OnClickNextEnemyInfoFlagBtn() {
        GM._.gui.Pause();
        ShowNextEnemyStateUI();
    }
    public void OnClickCloseNextEnemyInfoPopUp() {
        GM._.gui.Play();
        NextEnemyInfoPopUpUI.Obj.SetActive(false);
    }
#endregion

#region FUNC
    public void ShowEnemyStateUI() {
        EnemyData curEnemyDt = GM._.GetCurEnemyData();
        CurEnemyInfoWindowUI.SetUI(curEnemyDt, FrameSprs);
    }
    public void ShowNextEnemyStateUI() {
        EnemyData nextEnemyDt = GM._.GetNextEnemyData();
        NextEnemyInfoPopUpUI.SetUI(nextEnemyDt, FrameSprs);
    }
#endregion
}
