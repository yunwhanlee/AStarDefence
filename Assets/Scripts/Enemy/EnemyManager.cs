using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour {
    public Transform enemyObjGroup;
    public Enemy enemyPf;
    IObjectPool<Enemy> pool;    public IObjectPool<Enemy> Pool {get => pool;}

    void Awake() => pool = new ObjectPool<Enemy>(
        create, onGet, onRelease, onDestroyBlock, maxSize: 20
    );

#region OBJECT POOL
    private Enemy create() {
        Enemy enemy = Instantiate(enemyPf, enemyObjGroup);
        return enemy;
    }
    private void onGet(Enemy obj) { //* 使う
        obj.gameObject.SetActive(true);
    }
    private void onRelease(Enemy obj) { //* 戻す
        obj.gameObject.SetActive(false);
    }
    private void onDestroyBlock(Enemy obj) => Destroy(obj);
#endregion

#region FUNC
    public IEnumerator coCreateEnemy() {
        for(int i = 0; i < Config.CREATE_ENEMY_CNT; i++) {
            init(i);
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void init(int i) {
        //* 呼出
        Enemy enemy = pool.Get();
        enemy.Regist(pool);
        enemy.name = $"enemy{i}";
        enemy.transform.position = new Vector2(GM._.pfm.startPos.x, GM._.pfm.startPos.y);
    }
#endregion
}
