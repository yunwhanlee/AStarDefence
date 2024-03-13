using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Missile : MonoBehaviour {
    const int LIMIT_X = 10, LIMIT_Y = 5;
    [field:SerializeField] public Tower MyTower {get; set;}
    [field:SerializeField] public Transform Target {get; set;}
    [field:SerializeField] public SpriteRenderer SprRdr {get; set;}

    //* 単一なのに複数が当たることを防止
    [field:SerializeField] public List<Collider2D> ColList {get; set;}
    [field:SerializeField] public bool IsMultiShot = false;
    private Vector2 dir;
    private const float speed = 10;

    // void Awake() => SprRdr = GetComponent<SpriteRenderer>();

    void Start() {
        ColList = new List<Collider2D>();
        Init(MyTower);
    }

#region COLLISION
    void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.layer == Enum.Layer.Enemy) {
            //* 単一衝突なのに、複数のオブジェクトが重なり衝突が何度も起こるバグ対応
            ColList.Add(col); // ⓵ 衝突した敵をColListへ追加
        }
    }
#endregion

    void Update() {
        transform.Translate(speed * Time.deltaTime * Vector2.right);

        //* 座標が領域を超えたら、消す
        if(transform.position.x < -LIMIT_X || transform.position.x > LIMIT_X
        || transform.position.y < -LIMIT_Y || transform.position.y > LIMIT_Y)
            GM._.mm.Pool.Release(this);
    }

    void LateUpdate() {
        Hit(); // ⓶ 最後のフレームで、ColListから当たった敵の単一処理を行う
    }

#region FUNC
    public void Init(Tower myTower, float extraDeg = 0) {
        //* マルチショットなのかチェック
        IsMultiShot = (extraDeg != 0);

        MyTower = myTower;
        transform.position = new Vector2(MyTower.transform.position.x, MyTower.transform.position.y + 0.15f);
        Target = MyTower.trc.CurTarget;

        if(Target == null) return;

        dir = Target.position - transform.position;
        dir = dir.normalized;
        
        float degree = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree + extraDeg);

        if(MyTower.MissileSpr)
            SprRdr.sprite = MyTower.MissileSpr;
    }

    private void Hit() {
        Debug.Log($"<color=white>Missile:: Hit():: ColList.Count= {ColList.Count}</color>");
        //* もし、複数の衝突が発生しても
        if(ColList.Count >= 1) {
            for(int i = 0; i < ColList.Count; i++) {
                if(i == 0) { //* ０番インデックスのみ処理してバグ対応
                    Enemy enemy = ColList[0].GetComponent<Enemy>();

                    //* Migician爆発 Skill
                    if(MyTower.Kind == TowerKind.Magician) {
                        MagicianTower magician = MyTower as MagicianTower;
                        magician.Skill1_Explosion(enemy);
                    }

                    //* クリティカル
                    bool isCritical = Util.CheckCriticalDmg(MyTower);
                    int totalDmg = MyTower.Dmg * (isCritical? 2 : 1);

                    //* マルチショットなら、ダメージ半分
                    if(IsMultiShot)
                        totalDmg /= 2;

                    enemy.DecreaseHp(totalDmg, isCritical);
                    GM._.mm.Pool.Release(this); //* 戻すは一つのみなのに、複数衝突して１回以上読みこむとエラー
                    ColList.Clear();
                    break;
                }
            }
        }
    }
#endregion
}
