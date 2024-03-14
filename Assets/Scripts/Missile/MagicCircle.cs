using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MagicCircle : MonoBehaviour {
    [field:SerializeField] public List<Enemy> EnemyList {get; private set;} = new List<Enemy>();

    // void Start() {
    //     Init(MyTower);
    //     int dmg = Mathf.RoundToInt(MyTower.Dmg * MagicianTower.SK2_MagicCircleDmgPers[MyTower.LvIdx]);
    //     StartCoroutine(CoDecreaseEnemyListHp(dmg));
    // }
    

    #region FUNC
    public void Init(Tower myTower) {
        EnemyList = new List<Enemy>();
        //* ディレイ削除
        StartCoroutine(CoDestroyMe(myTower as MagicianTower));
        //* 位置
        Vector2 tgPos = myTower.trc.CurTarget.position;
        transform.position = new Vector2(tgPos.x, tgPos.y);
        //* ダメージ
        int dmg = Mathf.RoundToInt(myTower.Dmg * MagicianTower.SK2_MagicCircleDmgPers[myTower.LvIdx]);
        StartCoroutine(CoDecreaseEnemyListHp(dmg));
    }

    IEnumerator CoDestroyMe(MagicianTower myTower) {
        yield return Util.Time5;
        GM._.mm.PoolList[(int)MissileIdx.MagicCirclePurple].Release(gameObject);
        myTower.IsMagicCircleActive = false;
    }

    IEnumerator CoDecreaseEnemyListHp(int dmg) {
        while(true) {
            for(int i = 0; i < EnemyList.Count; i++) {
                if(EnemyList[i].gameObject.activeSelf) 
                    EnemyList[i].DecreaseHp(dmg); //* 良きっていたら、ダメージ
                else
                    EnemyList.Remove(EnemyList[i]); //* もう死んだら、削除
            }
            yield return Util.Time0_5; //* 周期
        }
    }
    #endregion

    #region COLLIDE
    void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.layer == Enum.Layer.Enemy)
            EnemyList.Add(col.GetComponent<Enemy>()); //* 敵リスト追加
    }
    void OnTriggerExit2D(Collider2D col) {
        if(col.gameObject.layer == Enum.Layer.Enemy)
            EnemyList.Remove(col.GetComponent<Enemy>()); //* 敵リスト削除
    }
    #endregion
}
