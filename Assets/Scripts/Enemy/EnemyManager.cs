using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour {
    const int CREATE_MAX = 20;
    const int CREATE_CNT = 15;

    public Transform enemyObjGroup;
    public Enemy enemyPf;
    IObjectPool<Enemy> pool;    public IObjectPool<Enemy> Pool {get => pool;}

    void Awake() => pool = new ObjectPool<Enemy>(
        create, onGet, onRelease, onDestroyBlock, maxSize: CREATE_MAX
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
        enemy.Init();
    }
    private void onDestroyBlock(Enemy enemy) => Destroy(enemy);
#endregion

#region FUNC
    public IEnumerator CoCreateEnemy() {
        for(int i = 0; i < CREATE_CNT; i++) {
            Init(i);
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void Init(int i) {
        //* 呼出
        Enemy enemy = pool.Get();
        enemy.name = $"enemy{i}";
        enemy.transform.position = new Vector2(GM._.pfm.startPos.x, GM._.pfm.startPos.y);
    }
#endregion
}
