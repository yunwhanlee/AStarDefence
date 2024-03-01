using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;
using Unity.VisualScripting;

public class EnemyManager : MonoBehaviour {
    public readonly static int CREATE_CNT = 15;

    [Header("STAGE ENEMY DATA LIST")]
    public SettingEnemyData[] StageDatas;

    [Space(10)]
    public Transform enemyObjGroup;
    public Enemy enemyPf;
    IObjectPool<Enemy> pool;    public IObjectPool<Enemy> Pool {get => pool;}
    private int spawnCnt;

    void Awake() => pool = new ObjectPool<Enemy>(
        create, onGet, onRelease, onDestroyBlock, maxSize: 20
    );

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
        enemy.Init(GetCurEnemyData());

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
            GM._.FinishRaid(); //* レイド終了
        }

        Debug.Log("isEnemyExist= " + isEnemyExist);
    }
    private void onDestroyBlock(Enemy enemy) => Destroy(enemy);
#endregion

#region FUNC
    public IEnumerator CoCreateEnemy() {
        //* スポーンカウント リセット
        spawnCnt = CREATE_CNT;
        //* 生成
        for(int i = 0; i < CREATE_CNT; i++) {
            Init(i); //* データ初期化
            GM._.gui.EnemyCntTxt.text = $"{--spawnCnt} / {CREATE_CNT}"; //* 敵スピーン数を表示
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void Init(int i) {
        //* 呼出
        Enemy enemy = pool.Get();

        //* データ設定（ScriptableObject敵リストから）
        enemy.Init(GetCurEnemyData());

        if(i == 0) GM._.gui.esm.ShowEnemyStateUI(enemy);
        enemy.name = $"enemy{i}";
        enemy.transform.position = new Vector2(GM._.pfm.startPos.x, GM._.pfm.startPos.y);
    }

    private EnemyData GetCurEnemyData() => StageDatas[GM._.Map].EnemyDatas[GM._.Stage - 1];
#endregion
}
