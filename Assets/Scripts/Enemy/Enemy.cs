using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public enum EnemyType {
    Monster, Boss
}

[System.Serializable]
public abstract class Enemy : MonoBehaviour {
    [field: SerializeField] public string Name {get; set;}
    [field: SerializeField] public int Lv {get; set;}
    [field: SerializeField] public int Hp {get; set;}
    [field: SerializeField] public int Speed {get; set;}

    [field: SerializeField] public int NodeIdx {get; private set;}
    IObjectPool<Enemy> enemyPool;

    void Start() {
        NodeIdx = 0;
    }

    void Update() {
        var nodeList = GM._.pfm.FinalNodeList;
        int len = nodeList.Count - 1;
        if(NodeIdx < len) {
            var curNode = nodeList[NodeIdx];
            Vector2 tgVec = new Vector2(curNode.x, curNode.y);
            Vector2 myVec = new Vector2(transform.position.x, transform.position.y);
            Vector2 vec = tgVec - myVec;
            Vector2 dir = vec.normalized;

            transform.Translate(dir * Speed * Time.deltaTime);
            
            if(vec.SqrMagnitude() < 0.01f) {
                Debug.Log("distance= " + vec.SqrMagnitude());
                NodeIdx++;
            }
        }
        else {
            release();
        }
    }

    #region FUNC
        private void release() => enemyPool.Release(this); //* 戻す
        public void regist(IObjectPool<Enemy> pool) => enemyPool = pool;
    #endregion
}
