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

    [Header("STAGE ENEMY DATA LIST")]
    public SettingEnemyData[] StageDatas;

    [Space(10)]
    public Transform enemyObjGroup;
    public Enemy enemyPf;
    public Enemy goblinEnemyPf;
    IObjectPool<Enemy> pool;    public IObjectPool<Enemy> Pool {get => pool;}
    [field:SerializeField] public int EnemyCnt {get; set;}
    [field:SerializeField] public int KillCnt {get; set;}
    [SerializeField] int spawnCnt;
    SkillTreeDB sktDb = DM._.DB.SkillTreeDB;

    void Awake() {
        EnemyCnt = MONSTER_CNT;
        pool = new ObjectPool<Enemy>(
            create, onGet, onRelease, onDestroyBlock, maxSize: 20
        );
    } 

#region OBJECT POOL
    private Enemy create() {
        Enemy enemy = Instantiate (
            DM._.SelectedStage == Config.Stage.STG_GOBLIN_DUNGEON? goblinEnemyPf : enemyPf, 
            enemyObjGroup
        );
        return enemy;
    }
    private void onGet(Enemy enemy) { //* 使う
        enemy.gameObject.SetActive(true);
    }
    private void onRelease(Enemy enemy) { //* 戻す
        enemy.gameObject.SetActive(false);
        enemy.Init(GM._.GetCurEnemyData());

        //* 死ぬカウント
        KillCnt++;

        //* Utilityスキル Lv３と５効果
        if(KillCnt % 10 == 0) {
            int extraMoney = 0;
            if(!sktDb.IsLockUtilitySTs[(int)SKT_UT.TEN_KILL_1MONEY])
                extraMoney += (int)sktDb.GetUtilityVal((int)SKT_UT.TEN_KILL_1MONEY);
            if(!sktDb.IsLockUtilitySTs[(int)SKT_UT.TEN_KILL_2MONEY])
                extraMoney += (int)sktDb.GetUtilityVal((int)SKT_UT.TEN_KILL_2MONEY);
            extraMoney += DM._.DB.EquipDB.BonusCoinBy10Kill;

            if(extraMoney > 0) {
                GM._.SetMoney(extraMoney);
                GM._.gef.ShowIconTxtEF(GM._.gui.MoneyTxt.transform.position, extraMoney, "Meat", isDown: true);
            }
        }
        

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
        bool isBoss = GM._.GetCurEnemyData().Type == EnemyType.Boss;
        EnemyCnt = isBoss? BOSS_CNT : MONSTER_CNT;
        spawnCnt = isBoss? BOSS_CNT : EnemyCnt;

        if(isBoss) {
            EnemyData bossData = GM._.GetCurEnemyData();
            GM._.esm.ShowBossSpawnAnim(bossData);
            GM._.esm.ShowBossHpBar(bossData);
        }

        //* 生成
        for(int i = 0; i < EnemyCnt; i++) {
            Init(i); //* データ初期化
            spawnCnt--;
            GM._.gui.EnemyCntTxt.text = $"{spawnCnt} / {EnemyCnt}"; //* 敵スピーン数を表示
            yield return Util.Time0_5;
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
