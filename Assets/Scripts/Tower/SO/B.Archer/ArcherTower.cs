using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using Random = UnityEngine.Random;

public class ArcherTower : Tower {
    public static readonly float[] SK1_CritIncPers = new float[6] {0, 0, 0.1f, 0.15f, 0.2f, 0.25f};
    public static readonly float[] SK2_MultiShotActivePers = new float[6] {0, 0, 0, 15, 20, 25};
    public static readonly int[] SK2_MultiShotCnts = new int[6] {0, 0, 0, 2, 4, 6};
    public static readonly int[] SK3_PassShotSpans = new int[6] {0, 0, 0, 0, 7, 4};
    public static readonly float[] SK3_PassShotDmgPers = new float[6] {0, 0, 0, 0, 10.0f, 15.0f};
    public static readonly float[] SK4_PerfectAimSpans = new float[6] {0, 0, 0, 0, 0, 10.0f};

    public override void CheckMergeUI() {
        Image mergeIcon = GM._.actBar.IconBtns[(int)ActionBarUIManager.ICON.Merge].GetComponent<Image>();

        //* 選んだタワーとTowerGroupの子リストと同じレベルの数を確認
        var towers = GM._.tm.ArcherGroup.GetComponentsInChildren<ArcherTower>();
        var sameLvTower = Array.FindAll(towers, tower => this.Lv == tower.Lv);

        //* １個以上があったら、マージ可能表示
        if(sameLvTower.Length > 1)
            mergeIcon.sprite = GM._.actBar.MergeOnSpr;
    }

    public override bool Merge(TowerKind kind = TowerKind.None) {
        Image mergeIcon = GM._.actBar.IconBtns[(int)ActionBarUIManager.ICON.Merge].GetComponent<Image>();

        //* マージが可能であれば
        if(mergeIcon.sprite == GM._.actBar.MergeOnSpr) {
            var towers = GM._.tm.ArcherGroup.GetComponentsInChildren<ArcherTower>();
            var another = Array.Find(towers, tower => this != tower && Lv == tower.Lv);

            //* 自分以外同じタワーを削除
            var anotherBoard = another.GetComponentInParent<Board>();
            anotherBoard.IsTowerOn = false;
            anotherBoard.transform.SetParent(GM._.tm.BoardGroup);

            DestroyImmediate(another.gameObject);

            //* 次のレベルタワーランダムで生成
            GM._.tm.CreateTower(Type, Lv++, kind);

            //* 自分を削除
            DestroyImmediate(gameObject);

            return true;
        }
        return false;
    }

    public override void Upgrade() {
        Debug.Log("Upgrade()::");
        StartCoroutine(GetComponent<CharacterControls>().CoSpawnAnim());
        int cardLv = GM._.tm.TowerCardUgrLvs[(int)Kind];

        //* 追加タメージDictionaryへ追加
        if(ExtraDmgDic.ContainsKey(DIC_UPGRADE)) ExtraDmgDic.Remove(DIC_UPGRADE);
        ExtraDmgDic.Add(DIC_UPGRADE, ExtraCardDmg(cardLv));
    }

    private int ExtraCardDmg(int cardLv) {
        //* タワーLV * カードLV * タイプのダメージアップ単位
        return cardLv >= 1? Lv * cardLv * TowerManager.ARCHER_CARD_DMG_UP : 0;
    }

    private void SetMultiShot(int cnt) {
        for(int i = 0; i < cnt; i++) {
            Missile ms = GM._.mm.CreateMissile();
            int sign = (i % 2 == 0)? -1 : 1;
            int degree = 10 * (1 + i / 2);
            ms.Init(this, sign * degree);
        }
    }

    public void Skill2_MultiShot() {
        int rand = Random.Range(0, 100);
        if(rand < SK2_MultiShotActivePers[Lv - 1]) {
            switch(Lv) {
                case 4: SetMultiShot(2);
                break;
                case 5: SetMultiShot(4);
                break;
                case 6: SetMultiShot(6);
                break;
            }
        }
    }
}
