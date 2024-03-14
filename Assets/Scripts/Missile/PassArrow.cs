using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassArrow : MonoBehaviour {
    const int LIMIT_X = 10, LIMIT_Y = 5;
    [field:SerializeField] public PassArrowIdx PassArrowIdx {get; set;}
    [field:SerializeField] public Tower MyTower {get; set;}
    [field:SerializeField] public Transform Target {get; set;}
    private Vector2 dir;
    private const float speed = 8;

    public void Init(Tower myTower, float extraDeg = 0) {
        MyTower = myTower;
        transform.position = new Vector2(MyTower.transform.position.x, MyTower.transform.position.y + 0.15f);
        Target = MyTower.trc.CurTarget;

        if(Target == null) return;

        dir = Target.position - transform.position;
        dir = dir.normalized;
        
        float degree = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree + extraDeg);
    }

    void Update() {
        transform.Translate(speed * Time.deltaTime * Vector2.right);

        //* 座標が領域を超えたら、消す
        if(transform.position.x < -LIMIT_X || transform.position.x > LIMIT_X
        || transform.position.y < -LIMIT_Y || transform.position.y > LIMIT_Y)
            GM._.mm.PassArrowPoolList[(int)PassArrowIdx].Release(this);
    }

    void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.layer == Enum.Layer.Enemy) {
            Enemy enemy = col.GetComponent<Enemy>();
            
            int dmg = (int)(MyTower.Dmg * ArcherTower.SK3_PassShotDmgPers[MyTower.LvIdx]);
            enemy.DecreaseHp(dmg);
        }
    }
}
