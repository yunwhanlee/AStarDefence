using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using Random = UnityEngine.Random;
// using UnityEditor.PackageManager;
// using UnityEditor;

public class MagicianTower : Tower {
    public static readonly int[] SK1_ExplosionLvActivePers = new int[6] {0, 0, 20, 25, 30, 35};
    public static readonly int[] SK2_MagicCircleActivePers = new int[6] {0, 0, 0, 10, 15, 20};
    public static readonly float[] SK2_MagicCircleDmgPers = new float[6] {0, 0, 0, 0.1f, 0.2f, 0.3f};

    public static readonly float[] SK3_LaserSpans = new float[6] {0, 0, 0, 0, 10, 8};
    public static readonly float[] SK3_LaserDmgPers = new float[6] {0, 0, 0, 0, 0.5f, 0.8f};

    public static readonly float[] SK4_BigbangSpans = new float[6] {0, 0, 0, 0, 0, 15};
    public static readonly float[] SK4_BigbangDmgs = new float[6] {0, 0, 0, 0, 0, 1.0f};

    public GameObject BigbangEF;
    [field:SerializeField] public bool IsMagicCircleOneTime {get; set;}
    [field:SerializeField] public bool IsLaserActive {get; set;}
    [field:SerializeField] public bool IsBigbangActive {get; set;}

    bool isDrawGizmos;
    Vector2 gizmosPos;

    void Start() {
        //* １回のみ生成トリガー
        IsMagicCircleOneTime = false; 
        //* 最初ONトリガー
        IsLaserActive = true; 
        IsBigbangActive = true;
    }

    public override void CheckMergeUI() {
        Image mergeIcon = GM._.actBar.IconBtns[(int)ActionBarUIManager.ICON.Merge].GetComponent<Image>();

        //* 選んだタワーとTowerGroupの子リストと同じレベルの数を確認
        var towers = GM._.tm.MagicianGroup.GetComponentsInChildren<MagicianTower>();
        var sameLvTower = Array.FindAll(towers, tower => Lv == tower.Lv);

        //* １個以上があったら、マージ可能表示
        if(sameLvTower.Length > 1) {
            mergeIcon.sprite = GM._.actBar.MergeOnSpr;
            GM._.actBar.MergePossibleMark.SetActive(true);
        }
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
        var efIdx = (Lv == 3)? GameEF.ExplosionWindEF
            : (Lv == 4)? GameEF.ExplosionFireballPurpleEF
            : (Lv == 5)? GameEF.ExplosionFireballBlueEF
            : (Lv == 6)? GameEF.ExplosionFireballRedEF : GameEF.NULL;
        GM._.gef.ShowEF(efIdx, target.transform.position);

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

    public void Skill2_MagicCircle() {
        //* 発動％にならなかったら、終了
        int rand = Random.Range(0, 100);
        if(rand >= SK2_MagicCircleActivePers[LvIdx])
            return;

        //* １キャラー当たり、１回のみ
        IsMagicCircleOneTime = true;

        int idx = Lv == 4? (int)MissileIdx.MagicCirclePurple
            : Lv == 5? (int)MissileIdx.MagicCircleBlue
            : Lv == 6? (int)MissileIdx.MagicCircleRed : -1;

        MagicCircle mc = GM._.mm.CreateMissile(idx).GetComponent<MagicCircle>();
        mc.Init(this);
    }

    public void Skill3_Laser() {
        if(IsLaserActive)
            StartCoroutine(CoSkill3_Laser());
    }
    IEnumerator CoSkill3_Laser() {
        const int WAIT_DESTROY_TIME = 2;

        IsLaserActive = false;
        int idx = Lv == 5? (int)MissileIdx.LaserBlue : Lv == 6? (int)MissileIdx.LaserRed : -1;

        Laser laser = GM._.mm.CreateMissile(idx).GetComponent<Laser>();
        laser.EffectObj.SetActive(false);
        laser.Init(this);

        yield return new WaitForSeconds(WAIT_DESTROY_TIME);
        GM._.mm.PoolList[idx].Release(laser.gameObject);

        yield return new WaitForSeconds(SK3_LaserSpans[LvIdx] - WAIT_DESTROY_TIME);
        IsLaserActive = true;
    }

    public void Skill4_Bigbang() {
        if(IsBigbangActive)
            StartCoroutine(CoSkill4_Bigbang());
    }

    IEnumerator CoSkill4_Bigbang() {
        const int WAIT_DELAY_TIME = 2;
        const int WAIT_SPAWN_TIME = 2;
        const int WAIT_DESTROY_TIME = 2;

        yield return new WaitForSeconds(WAIT_DELAY_TIME);

        //* スキルEF 表示
        IsBigbangActive = false;
        BigbangEF.SetActive(true);

        //* 全ての敵にダメージ
        yield return new WaitForSeconds(WAIT_SPAWN_TIME);
        Transform enemyGroup = GM._.em.enemyObjGroup;
        for(int i = 0; i < enemyGroup.childCount; i++) {
            Enemy enemy = enemyGroup.GetChild(i).GetComponent<Enemy>();
            int dmg = Mathf.RoundToInt(Dmg * SK4_BigbangDmgs[LvIdx]);
            Util._.ComboAttack(enemy, dmg, hitCnt: 5, Util.Time0_15);
        }
        
        //* スキルEF 非表示
        yield return new WaitForSeconds(WAIT_DESTROY_TIME);
        BigbangEF.SetActive(false);

        //* CoolTime
        yield return new WaitForSeconds(SK4_BigbangSpans[LvIdx] - WAIT_DELAY_TIME - WAIT_SPAWN_TIME - WAIT_DESTROY_TIME);
        IsBigbangActive = true;
    }


#region GIZMOS
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
#endregion
}
