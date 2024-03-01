using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class EnemyStateUIManager : MonoBehaviour {
    const int FRAME_GRAY = 0, FRAME_RED = 1, FRAME_YELLOW = 2;
    [field: SerializeField] public GameObject WindowObj {get; set;}
    [field: SerializeField] public Sprite[] FrameSprs {get; set;}
    [field: SerializeField] public Image Frame {get; set;}
    [field: SerializeField] public Image IconImg {get; set;}
    [field: SerializeField] public TextMeshProUGUI LvTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI NameTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI HpTxt {get; set;}
    [field: SerializeField] public TextMeshProUGUI SpeedTxt {get; set;}

    public void ShowEnemyStateUI(Enemy enemy) {
        Debug.Log($"ShowEnemyStateUI():: ");
        EnemyType type = enemy.Type;
        bool isMonster = type == EnemyType.Land || type == EnemyType.Flight;
        bool isBoss = type == EnemyType.Boss;
        bool isGoblin = type == EnemyType.Goblin;

        //* Frame Sprite
        Frame.sprite = isMonster? FrameSprs[FRAME_GRAY]
            : isBoss? FrameSprs[FRAME_RED] 
            : isGoblin? FrameSprs[FRAME_YELLOW] : null;
        IconImg.sprite = enemy.SprRdr.sprite;

        //* 情報表示
        LvTxt.text = $"LV {enemy.Lv}";
        NameTxt.text = $"{enemy.Name}";
        HpTxt.text = $"{enemy.Hp}";
        SpeedTxt.text = $"{enemy.Speed}";

        WindowObj.SetActive(true);
    }
}
