using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
    [field:SerializeField] public GameObject EffectObj {get; set;}
    [field:SerializeField] public Tower MyTower {get; set;}
    [field:SerializeField] public Transform Target {get; set;}

    private Vector2 dir;
    private const float OffsetY = 0.15f;
    private const float speed = 55;

    public void Init(Tower myTower) {
        MyTower = myTower;
        transform.position = new Vector2(MyTower.transform.position.x, MyTower.transform.position.y + OffsetY);
        Target = MyTower.trc.CurTarget;

        if(Target == null) return;

        dir = Target.position - transform.position;
        dir = dir.normalized;

        float degree = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);

        EffectObj.SetActive(true);
    }

    void Update() {
        transform.Translate(speed * Time.deltaTime * Vector2.right);
    }

#region COLLIDE
    void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.layer == Enum.Layer.Enemy) {
            Enemy enemy = col.GetComponent<Enemy>(); //* 敵リスト追加

            int dmg = Mathf.RoundToInt(MyTower.Dmg * MagicianTower.SK3_LaserDmgPers[MyTower.LvIdx]);
            enemy.DecreaseHp(dmg);
        }
    }
#endregion
}