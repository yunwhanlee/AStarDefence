using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;
using Unity.VisualScripting;
using System.Linq;

public class EnemyManager : MonoBehaviour {
    const int MONSTER_CNT = 30;
    const int BOSS_CNT = 1;
    const int SEMIBOSS_CNT = 2;

    [Header("STAGE ENEMY DATA LIST")]
    public SettingEnemyData[] StageDatas;

    [Space(10)]
    public Transform enemyObjGroup;
    public Enemy enemyPf;
    IObjectPool<Enemy> pool;    public IObjectPool<Enemy> Pool {get => pool;}
    [field:SerializeField] public int EnemyCnt {get; set;}
    int spawnCnt;

    void Awake() {
        EnemyCnt = MONSTER_CNT;
        pool = new ObjectPool<Enemy>(
            create, onGet, onRelease, onDestroyBlock, maxSize: 20
        );
    } 

#region OBJECT POOL
    private Enemy create() {
        Enemy enemy = Instantiate(enemyPf, enemyObjGroup);
        return enemy;
    }
    private void onGet(Enemy enemy) { //* 使う
        enemy.gameObject.SetActive(true);
    }
    private void onRelease(Enemy enemy) { //* 戻す
        enemy.gameObject.SetActive(false);
        enemy.Init(GM._.GetCurEnemyData());

        //* レイド終了をチェック
        // 敵のスポーンが終わらないと以下の処理しない
        if(spawnCnt > 0) 
            return;

        // 敵が存在しているのかを確認
        bool isEnemyExist = false;
        for(int i = 0; i < enemyObjGroup.childCount; i++) {
            if(enemyObjGroup.GetChild(i).gameObject.activeSelf) {
                isEnemyExist = true;
                break;
            }
        }

        //* 敵が存在しなかったら、
        if(!isEnemyExist) {
            GM._.FinishWave(); //* レイド終了
        }

        Debug.Log("isEnemyExist= " + isEnemyExist);
    }
    private void onDestroyBlock(Enemy enemy) => Destroy(enemy);
#endregion

#region FUNC
    public IEnumerator CoCreateEnemy() {
        //* スポーンカウント リセット
        Debug.Log($"GM._.em.GetCurEnemyData().Type= {GM._.GetCurEnemyData().Type}");
        bool isTypeBoss = GM._.GetCurEnemyData().Type == EnemyType.Boss;
        spawnCnt = isTypeBoss? BOSS_CNT : EnemyCnt;
        EnemyCnt = isTypeBoss? BOSS_CNT : MONSTER_CNT;

        //* 生成
        for(int i = 0; i < EnemyCnt; i++) {
            Init(i); //* データ初期化
            GM._.gui.EnemyCntTxt.text = $"{--spawnCnt} / {EnemyCnt}"; //* 敵スピーン数を表示
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void Init(int i) {
        //* 呼出
        Enemy enemy = pool.Get();

        //* データ設定（ScriptableObject敵リストから）
        enemy.Init(GM._.GetCurEnemyData());

        //* 敵の情報UI表示
        if(i == 0)
            GM._.gui.esm.ShowEnemyStateUI();

        enemy.name = $"enemy{i}";
        enemy.transform.position = new Vector2(GM._.pfm.startPos.x, GM._.pfm.startPos.y);
    }
#endregion
}
