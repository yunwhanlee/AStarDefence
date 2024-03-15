using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRain : MonoBehaviour {
    [field:SerializeField] public List<Enemy> EnemyList {get; private set;} = new List<Enemy>();

#region FUNC
    public void Init(Tower myTower) {
        EnemyList = new List<Enemy>();
        //* ディレイ削除
        StartCoroutine(CoDestroyMe());
        //* 位置
        transform.position = new Vector2(3, 5);
        //* ダメージ
        int dmg = Mathf.RoundToInt(myTower.Dmg);
        StartCoroutine(CoDecreaseEnemyListHp(dmg));
    }

    IEnumerator CoDestroyMe() {
        yield return Util.Time5;
        GM._.mm.PoolList[(int)MissileIdx.ArrowRain].Release(gameObject);
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
