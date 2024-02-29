using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MagicianTower : Tower {
    public readonly static int CARD_UPG_DMG_UP = 3;

    public override void CheckMergeUI() {
        Image mergeIcon = GM._.actBar.IconBtns[(int)ActionBarUIManager.ICON.Merge].GetComponent<Image>();

        //* 選んだタワーとTowerGroupの子リストと同じレベルの数を確認
        var towers = GM._.tm.MagicianGroup.GetComponentsInChildren<MagicianTower>();
        var sameLvTower = Array.FindAll(towers, tower => this.Lv == tower.Lv);

        //* １個以上があったら、マージ可能表示
        if(sameLvTower.Length > 1)
            mergeIcon.sprite = GM._.actBar.MergeOnSpr;
    }

    public override bool Merge() {
        Image mergeIcon = GM._.actBar.IconBtns[(int)ActionBarUIManager.ICON.Merge].GetComponent<Image>();

        //* マージが可能であれば
        if(mergeIcon.sprite == GM._.actBar.MergeOnSpr) {
            var towers = GM._.tm.MagicianGroup.GetComponentsInChildren<MagicianTower>();
            var another = Array.Find(towers, tower => this != tower && Lv == tower.Lv);
            
            //* 自分以外同じタワーを削除
            var anotherBoard = another.GetComponentInParent<Board>();
            anotherBoard.IsTowerOn = false;
            anotherBoard.transform.SetParent(GM._.tm.BoardGroup);

            DestroyImmediate(another.gameObject);

            //* 次のレベルタワーランダムで生成
            GM._.tm.CreateTower(Type, Lv++);

            //* 自分を削除
            DestroyImmediate(gameObject);

            return true;
        }

        return false;
    }

    public override void Upgrade() {
        Dmg = TowerData.Dmg + Lv * CARD_UPG_DMG_UP;;
    }

    public override string[] InfoState() {
        Debug.Log($"Tower:: InfoState():: Name={Name}, Lv= {Lv}");

        //* カードアップグレードデータ反映
        int cardLv = GM._.tm.TowerCardUgrLvs[(int)Kind];
        string extraDmg = cardLv > 0? $"<color=green>(+{Lv * CARD_UPG_DMG_UP * cardLv})</color>" : "";

        string[] states = new string[9];
        int i = 0;
        states[i++] = Lv.ToString(); //* Gradeラベルとして表示
        states[i++] = $"{Dmg}{extraDmg}";
        states[i++] = $"{AtkSpeed}";
        states[i++] = $"{AtkRange}";
        states[i++] = $"{SplashRange}";
        states[i++] = $"{CritPer}";
        states[i++] = $"{CritDmgPer}";
        states[i++] = $"{SlowPer}";
        states[i++] = $"{StunSec}";
        return states;
    }
}
