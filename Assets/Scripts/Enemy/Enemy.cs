using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public enum EnemyType {
    Land, Flight, Boss, Goblin
}

[System.Serializable]
public abstract class Enemy : MonoBehaviour {
    Coroutine CorSlow;
    Coroutine CorStun;

    [field: SerializeField] public EnemyType Type {get; set;}
    [field: SerializeField] public SpriteRenderer SprRdr {get; set;}
    [field: SerializeField] public Slider HpBar {get; set;}

    [field: SerializeField] public string Name {get; set;}
    [field: SerializeField] public int Lv {get; set;}
    [field: SerializeField] public int Hp {get; set;}
    private int maxHp;
    [field: SerializeField] public float Speed {get; set;}
    private float originSpd;
    [field: SerializeField] public int NodeIdx {get; private set;}
    IObjectPool<Enemy> enemyPool;
    [field: SerializeField] public Material DefaultMt;
    [field: SerializeField] public Material BlinkMt;

    void Start() {
        originSpd = Speed;
        maxHp = Hp;
        SprRdr = GetComponent<SpriteRenderer>();
        HpBar = GetComponentInChildren<Slider>();
        HpBar.value = (float)Hp / maxHp;
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
            
            //* 長さが0.01以下なら、次のノードに進む
            if(vec.SqrMagnitude() < 0.01f) {
                // Debug.Log("distance= " + vec.SqrMagnitude());
                NodeIdx++;
            }
        }
        else {
            Release();
        }
    }

    #region FUNC
        private void Release() => enemyPool.Release(this); //* 戻す
        public void Regist(IObjectPool<Enemy> pool) => enemyPool = pool;
        public void DecreaseHp(int val) {
            StartCoroutine(BlinkCoroutine());
            Hp -= val;
            HpBar.value = (float)Hp / maxHp;
            if(Hp <= 0) {
                Hp = 0;
                Die();
            }
        }
        private IEnumerator BlinkCoroutine() {
            SprRdr.material = BlinkMt;
            yield return new WaitForSeconds(0.1f);
            SprRdr.material = DefaultMt;
        }
        public void Slow(float per) {
            if(CorSlow != null) {
                StopCoroutine(CorSlow);
                CorSlow = null;
            }
            CorSlow = StartCoroutine(CoSlow(per));
        } 
        IEnumerator CoSlow(float per) {
            Speed = originSpd * (1 - per);
            SprRdr.color = Color.blue;
            yield return Util.time2;
            Speed = originSpd;
            SprRdr.color = Color.white;
        }
        public void Stun(float sec) {
            if(CorStun != null) {
                StopCoroutine(CorStun);
                CorStun = null;
            }
            CorStun = StartCoroutine(CoStun(sec));
        }
        IEnumerator CoStun(float sec) {
            Speed = 0;
            SprRdr.color = Color.yellow;
            yield return new WaitForSeconds(sec);
            Speed = originSpd;
            SprRdr.color = Color.white;
        }
        public void Die() {
            Release();
        }
    #endregion
}
