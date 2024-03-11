using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using Random = UnityEngine.Random;

public class WarriorTower : Tower {
    public Coroutine CorSkill1ID;
    public GameObject RageAuraEF;
    [field:SerializeField] public int RageIncDmg {get; private set;}
    [field:SerializeField] public float RageDecSpd {get; private set;}

    public override void CheckMergeUI() {
        Image mergeIcon = GM._.actBar.IconBtns[(int)ActionBarUIManager.ICON.Merge].GetComponent<Image>();

        //* 選んだタワーとTowerGroupの子リストと同じレベルの数を確認
        var towers = GM._.tm.WarriorGroup.GetComponentsInChildren<WarriorTower>();
        var sameLvTower = Array.FindAll(towers, tower => this.Lv == tower.Lv);

        //* １個以上があったら、マージ可能表示
        if(sameLvTower.Length > 1)
            mergeIcon.sprite = GM._.actBar.MergeOnSpr;
    }

    public override bool Merge(TowerKind kind = TowerKind.None) {
        Image mergeIcon = GM._.actBar.IconBtns[(int)ActionBarUIManager.ICON.Merge].GetComponent<Image>();

        //* マージが可能であれば
        if(mergeIcon.sprite == GM._.actBar.MergeOnSpr) {
            var towers = GM._.tm.WarriorGroup.GetComponentsInChildren<WarriorTower>();
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

        Dmg = TowerData.Dmg 
            //* タワーレベル１以上なら、カードアップグレード値を掛ける
            + (cardLv >= 1? cardLv * TowerManager.WARRIOR_CARD_DMG_UP : 0);
    }

    public void Skill1_Rage() {
        if(CorSkill1ID != null)
            return;

        int[] lvPercents = new int[6] {0, 0, 80, 10, 15, 20};
        int rand = Random.Range(0, 100);
        if(rand < lvPercents[Lv - 1]) {
            CorSkill1ID = StartCoroutine(CoSkill1_Rage());
        }
    }

    IEnumerator CoSkill1_Rage() {
        Debug.Log("CoSkill1_Rage()::");
        RageAuraEF.SetActive(true);
        switch(Lv) {
            case 3:
                RageIncDmg = (int)(TowerData.Dmg * 0.1f);
                RageDecSpd = (int)(TowerData.AtkSpeed * 0.1f);
                break;
            case 4:
                RageIncDmg = (int)(TowerData.Dmg * 0.2f);
                RageDecSpd = (int)(TowerData.AtkSpeed * 0.2f);
                break;
            case 5:
                RageIncDmg = (int)(TowerData.Dmg * 0.3f);
                RageDecSpd = (int)(TowerData.AtkSpeed * 0.3f);
                break;
            case 6:
                RageIncDmg = (int)(TowerData.Dmg * 0.4f);
                RageDecSpd = (int)(TowerData.AtkSpeed * 0.4f);
                break;
        }
        // GM._.gui.tsm.ShowTowerStateUI(InfoState());

        yield return new WaitForSeconds(3);
        RageAuraEF.SetActive(false);
        CorSkill1ID = null;
        // GM._.gui.tsm.ShowTowerStateUI(InfoState());
    }
}
