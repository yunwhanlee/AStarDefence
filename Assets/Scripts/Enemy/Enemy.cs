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
    public readonly static int LIFE_DEC_BOSS = 5;
    public readonly static int LIFE_DEC_MONSTER = 1;
    public readonly static int ORIGIN_SORTING_LAYER = 9;

    Coroutine CorSlow;
    Coroutine CorStun;

    [field: SerializeField] public EnemyType Type {get; set;}
    [field: SerializeField] public SpriteRenderer SprRdr {get; set;}
    [field: SerializeField] public Slider HpBar {get; set;}

    [field: SerializeField] public bool IsDie {get; set;}
    [field: SerializeField] public string Name {get; set;}
    [field: SerializeField] public int Lv {get; set;}
    [field: SerializeField] public int Hp {get; set;}
    private int maxHp;
    [field: SerializeField] public float Speed {get; set;}
    private float originSpd;
    [field: SerializeField] public int NodeIdx {get; private set;}

    void Awake() {
        SprRdr = GetComponent<SpriteRenderer>();
        HpBar = GetComponentInChildren<Slider>();
    }

    // void Start() {
    //     originSpd = Speed;
    //     maxHp = Hp;
    //     Init();
    // }

    void Update() {
        //* 飛ぶタイプ
        if(Type == EnemyType.Flight) {
            Vector2 goalDir = GM._.pfm.targetPos;
            transform.Translate(Speed * Time.deltaTime * goalDir.normalized);

            if(transform.position.x > GM._.pfm.targetPos.x
            && transform.position.y > GM._.pfm.targetPos.y) {
                Release();
                GM._.DecreaseLife(Type);
            }
            return;
        }

        //* 歩くタイプ
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
            //* 敵がゴールまで届いた
            Release();
            GM._.DecreaseLife(Type);
        }
    }

    #region FUNC
        private void Release() => GM._.em.Pool.Release(this); //* 戻す
        /// <summary>
        /// 初期化
        /// </summary>
        public void Init(EnemyData curEnemyDt) {
            SetData(curEnemyDt);
            HpBar.value = (float)Hp / maxHp;
            NodeIdx = 0;
            Util._.SetDefMt(SprRdr);
            SprRdr.color = Color.white;
            SprRdr.sortingOrder = (Type == EnemyType.Flight)? 15 : ORIGIN_SORTING_LAYER;
            transform.localScale = Vector2.one * (Type == EnemyType.Boss? 2 : 1);

        }
        /// <summary>
        /// 現在ステージに合わせてデータ設定
        /// </summary>
        private void SetData(EnemyData curEnemyDt) {
            Name = curEnemyDt.Name;
            Lv = curEnemyDt.Lv;
            Type = curEnemyDt.Type;
            SprRdr.sprite = curEnemyDt.Spr;
            maxHp = curEnemyDt.Hp;
            Hp = maxHp;
            originSpd = curEnemyDt.Speed;
            Speed = originSpd;
        }
        /// <summary>
        /// 敵のHP減る
        /// </summary>
        public void DecreaseHp(int val, bool isCritical = false) {
            if(!gameObject.activeSelf) {
                return;
            }

            GM._.gef.ShowDmgTxtEF(transform.position, val, isCritical);

            Util._.Blink(SprRdr);
            Hp -= val;
            HpBar.value = (float)Hp / maxHp;

            if(Hp <= 0) {
                Hp = 0;
                Die();
            }
        }
        private void Die() {
            Release();
            GM._.SetMoney(+1);
        }

        /// <summary>
        /// CCコルーチン更新：以前に適用された同じCCがあったら、終了して新しく更新
        /// </summary>
        /// <param name="corID">(*ref 参照) コルーチンID</param>
        private void UpdateCorID(ref Coroutine corID) {
            if(corID != null) {
                StopCoroutine(corID);
                corID = null;
            }
        }
        /// <summary>
        /// 敵のスロー
        /// </summary>
        public void Slow(float sec) {
            UpdateCorID(ref CorSlow);
            CorSlow = StartCoroutine(CoSlow(sec));
        }
        IEnumerator CoSlow(float sec) {
            Speed = originSpd * 0.5f; //* 減速
            SprRdr.color = Color.blue;
            yield return new WaitForSeconds(sec);
            Speed = originSpd; //* 戻す
            SprRdr.color = Color.white;
        }
        /// <summary>
        /// 敵のスタン
        /// </summary>
        public void Stun(float sec) {
            UpdateCorID(ref CorStun);
            CorStun = StartCoroutine(CoStun(sec));
        }
        IEnumerator CoStun(float sec) {
            Speed = 0;
            SprRdr.color = Color.yellow;
            yield return new WaitForSeconds(sec);
            Speed = originSpd;
            SprRdr.color = Color.white;
        }


    #endregion
}
