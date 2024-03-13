using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using Random = UnityEngine.Random;

public class WarriorTower : Tower {
    public static readonly int[] SK1_RageActivePers = new int[6] {0, 0, 5, 10, 15, 20};
    public static readonly float[] SK1_RageDmgSpdIncPers = new float[6] {0, 0, 0.15f, 0.2f, 0.3f, 0.4f};
    public static readonly float[] SK1_RageTime = new float[6] {0, 0, 2.5f, 3, 3.5f, 4};

    public static readonly float[] SK2_WheelwindActivePers = new float[6] {0, 0, 0, 10, 12, 15};
    public static readonly float[] SK2_WheelwindDmgPers = new float[6] {0, 0, 0, 0.5f, 0.7f, 0.9f};

    public static readonly float[] SK3_CheerUpSpans = new float[6] {0, 0, 0, 0, 12, 10};
    public static readonly float[] SK3_CheerUpDmgSpdIncPers = new float[6] {0, 0, 0, 0, 0.2f, 0.4f};

    public static readonly float[] SK4_RoarSpans = new float[6] {0, 0, 0, 0, 0, 15};
    public static readonly float[] SK4_RoarDmgPers = new float[6] {0, 0, 0, 0, 0, 5.0f};

    public GameObject RageAuraEF;
    public GameObject WheelwindEF;
    
    public Coroutine CorSkill1ID;
    [field:SerializeField] public int RageDmgUp {get; private set;}
    [field:SerializeField] public float RageSpdUp {get; private set;}

    bool isDrawGizmos;
    Vector2 gizmosPos;

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

        //* 追加タメージDictionaryへ追加
        if(ExtraDmgDic.ContainsKey(DIC_UPGRADE)) ExtraDmgDic.Remove(DIC_UPGRADE);
        ExtraDmgDic.Add(DIC_UPGRADE, ExtraCardDmg(cardLv));
    }

    private int ExtraCardDmg(int cardLv) {
        //* タワーLV * カードLV * タイプのダメージアップ単位
        return cardLv >= 1? Lv * cardLv * TowerManager.WARRIOR_CARD_DMG_UP : 0;
    }

    public void Skill1_Rage() {
        if(CorSkill1ID != null)
            return;

        int rand = Random.Range(0, 100);
        if(rand < SK1_RageActivePers[Lv - 1]) {
            CorSkill1ID = StartCoroutine(CoSkill1_Rage());
        }
    }

    IEnumerator CoSkill1_Rage() {
        Debug.Log("CoSkill1_Rage()::");
        const string RAGE = "RAGE";

        RageDmgUp = (int)(TowerData.Dmg * SK1_RageDmgSpdIncPers[Lv]);
        RageSpdUp = (float)(TowerData.AtkSpeed * SK1_RageDmgSpdIncPers[Lv]);

        //* 追加タメージ
        if(ExtraDmgDic.ContainsKey(RAGE)) ExtraDmgDic.Remove(RAGE);
        ExtraDmgDic.Add(RAGE, RageDmgUp);
        //* 追加スピード
        if(ExtraSpdDic.ContainsKey(RAGE)) ExtraSpdDic.Remove(RAGE);
        ExtraSpdDic.Add(RAGE, RageSpdUp);

        RageAuraEF.SetActive(true);
        GM._.gui.tsm.ShowTowerStateUI(InfoState());

        yield return new WaitForSeconds(2.5f);
        RageAuraEF.SetActive(false);
        CorSkill1ID = null;
        ExtraDmgDic.Remove(RAGE);
        ExtraSpdDic.Remove(RAGE);
        GM._.gui.tsm.ShowTowerStateUI(InfoState());
    }

    public void Skill2_Wheelwind() {
        int lvIdx = Lv - 1;

        //* 発動％にならなかったら、終了
        int rand = Random.Range(0, 100);
        if(rand >= SK2_WheelwindActivePers[Lv - 1])
            return;

        StartCoroutine(CoActiveGizmos(transform.position));

        int layerMask = 1 << Enum.Layer.Enemy;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, AtkRange, layerMask);
        Debug.Log("アタック！ ROUND:: colliders= " + colliders.Length);

        StartCoroutine(CoShowWheelwindEF());

        //* ホイールウィンド！
        for(int i = 0; i < colliders.Length; i++) {
            Enemy enemy = colliders[i].GetComponent<Enemy>();

            //* ダメージ
            enemy.DecreaseHp(Mathf.RoundToInt(Dmg * SK2_WheelwindDmgPers[lvIdx]));

            //* 気絶(きぜつ)
            if(enemy.gameObject.activeSelf)
                enemy.Stun(0.5f);
        }
    }

    IEnumerator CoShowWheelwindEF() {
        WheelwindEF.SetActive(true);
        yield return Util.Time1_5;
        WheelwindEF.SetActive(false);
    }

    IEnumerator CoActiveGizmos(Vector2 pos) {
        gizmosPos = pos;
        isDrawGizmos = true;
        yield return Util.Time1;
        isDrawGizmos = false;
    }
    
    void OnDrawGizmos() {
        if(isDrawGizmos) {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(new Vector3(gizmosPos.x, gizmosPos.y, 0), AtkRange);
        }
    }
}
