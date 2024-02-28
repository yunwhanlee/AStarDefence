using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Missile : MonoBehaviour {
    [field:SerializeField] public Tower MyTower {get; set;}
    [field:SerializeField] public Transform Target {get; set;}

    //* 単一なのに複数が当たることを防止
    [field:SerializeField] public List<Collider2D> ColList {get; set;}

    private Vector2 dir;
    private const float speed = 10;

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
    }

    void LateUpdate() {
        Hit(); // ⓶ 最後のフレームで、ColListから当たった敵の単一処理を行う
    }

#region FUNC
    public void Init(Tower myTower) {
        MyTower = myTower;
        transform.position = MyTower.transform.position;
        Target = MyTower.trc.CurTarget;
        dir = Target.position - transform.position;
        dir = dir.normalized;
        
        float degree = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private void Hit() {
        Debug.Log($"<color=white>Missile:: Hit():: ColList.Count= {ColList.Count}</color>");
        //* もし、複数の衝突が発生しても
        if(ColList.Count >= 1) {
            for(int i = 0; i < ColList.Count; i++) {
                if(i == 0) { //* ０番インデックスのみ処理してバグ対応
                    Enemy enemy = ColList[0].GetComponent<Enemy>();
                    enemy.DecreaseHp(MyTower.Dmg);
                    GM._.mm.Pool.Release(this); //* 戻すは一つのみなのに、複数衝突して１回以上読みこむとエラー
                    ColList.Clear();
                    break;
                }
            }
        }
    }
#endregion
}
