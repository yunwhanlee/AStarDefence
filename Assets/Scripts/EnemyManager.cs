using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour {
    public Transform enemyObjGroup;
    public GameObject enemyPf;
    IObjectPool<GameObject> pool;    public IObjectPool<GameObject> Pool {get => pool;}

    void Awake() => pool = new ObjectPool<GameObject>(
        create, onGet, onRelease, onDestroyBlock, maxSize: 20
    );

    void Start() {
        StartCoroutine(coCreateEnemy());
    }

#region OBJECT POOL
    private GameObject create() {
        GameObject enemy = Instantiate(enemyPf, enemyObjGroup);
        return enemy;
    }
    private void onGet(GameObject obj) { //* 使う
        obj.SetActive(true);
    }
    private void onRelease(GameObject obj) { //* 戻す
        obj.SetActive(false);
    }
    private void onDestroyBlock(GameObject obj) => Destroy(obj);
#endregion

#region FUNC
    public IEnumerator coCreateEnemy() {
        for(int i = 0; i < 10; i++) {
            init();
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void init() {
        //* 呼出
        GameObject obj = pool.Get();
        obj.name = "enemy";
        var startPos = GM._.pfm.startPos;
        obj.transform.localPosition = new Vector2(startPos.x, startPos.y);
    }
#endregion
}
