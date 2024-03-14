using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using Random = UnityEngine.Random;
using UnityEditor.PackageManager;

public class MagicianTower : Tower {
    public static readonly int[] SK1_ExplosionLvActivePers = new int[6] {0, 0, 20, 25, 30, 35};
    public static readonly int[] SK2_IgniteActivePers = new int[6] {0, 0, 0, 10, 15, 20};
    public static readonly float[] SK2_IgniteDmgPers = new float[6] {0, 0, 0, 0.1f, 0.15f, 0.2f};
    public static readonly float[] SK3_LaserSpans = new float[6] {0, 0, 0, 0, 7, 5};
    public static readonly float[] SK3_LaserDmgPers = new float[6] {0, 0, 0, 0, 1.0f, 2.0f};
    public static readonly float[] SK4_MeteorSpans = new float[6] {0, 0, 0, 0, 0, 12};
    public static readonly float[] SK4_MeteorDmgs = new float[6] {0, 0, 0, 0, 0, 10.0f};

    bool isDrawGizmos;
    Vector2 gizmosPos;

    public override void CheckMergeUI() {
        Image mergeIcon = GM._.actBar.IconBtns[(int)ActionBarUIManager.ICON.Merge].GetComponent<Image>();

        //* 選んだタワーとTowerGroupの子リストと同じレベルの数を確認
        var towers = GM._.tm.MagicianGroup.GetComponentsInChildren<MagicianTower>();
        var sameLvTower = Array.FindAll(towers, tower => Lv == tower.Lv);

        //* １個以上があったら、マージ可能表示
        if(sameLvTower.Length > 1)
            mergeIcon.sprite = GM._.actBar.MergeOnSpr;
    }

    public override bool Merge(TowerKind kind = TowerKind.None) {
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
        return cardLv >= 1? Lv * cardLv * TowerManager.MAGICIAN_CARD_DMG_UP : 0;
    }

    public void Skill1_Explosion(Enemy target) {
        const float RADIUS = 1;

        //* 発動％にならなかったら、終了
        int rand = Random.Range(0, 100);
        if(rand >= SK1_ExplosionLvActivePers[LvIdx])
            return;

        StartCoroutine(CoActiveGizmos(target.transform.position));
        GM._.gef.ShowEF(GameEF.ExplosionFireballEF, target.transform.position);

        int layerMask = 1 << Enum.Layer.Enemy;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(target.transform.position, RADIUS, layerMask);
        Debug.Log("アタック！ ROUND:: colliders= " + colliders.Length);
        foreach(Collider2D col in colliders) {
            Enemy enemy = col.GetComponent<Enemy>();

            //* ターゲットの敵はダメージ計算が重なるので除外
            if(target == enemy)
                continue;

            //* ダメージ５０％ (クリティカル 無し)
            enemy.DecreaseHp(Dmg / 2);
        }
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
            Gizmos.DrawWireSphere(new Vector3(gizmosPos.x, gizmosPos.y, 0), 1);
        }
    }
}
