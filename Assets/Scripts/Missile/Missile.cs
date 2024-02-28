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
    private const float speed = 5;

    void Start() {
        ColList = new List<Collider2D>();
        dir = Target.position - transform.position;
        dir = dir.normalized;
    }
    void Update() {
        transform.Translate(dir * speed * Time.deltaTime);
    }

#region COLLISION
    void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.layer == Enum.Layer.Enemy) {
            ColList.Add(col);
            //* 衝突が複数になるバグ対応
            if(ColList.Count > 1) {
                for(int i = 0; i < ColList.Count; i++) {
                    if(i == 0) {
                        Enemy enemy = ColList[0].GetComponent<Enemy>();
                        enemy.DecreaseHp(MyTower.Dmg);
                        GM._.mm.Pool.Release(this); //* 戻すは一つのみなのに、複数衝突して１回以上読みこむとエラー
                        ColList.Clear();
                        break;
                    }
                }
            }
        }
    }
#endregion

#region FUNC
    public void Init(Tower myTower) {
        MyTower = myTower;
        transform.position = MyTower.transform.position;
        Target = MyTower.trc.CurTarget;
    }
#endregion
}
