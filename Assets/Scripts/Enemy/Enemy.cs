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
    const int LIFE_DEC_BOSS = 5;
    const int LIFE_DEC_MONSTER = 1;

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
        Init();
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
            //* 敵がゴールまで届いた
            Release();
            Util._.Blink(GM._.gui.HeartFillImg);
            GM._.Life -= (Type == EnemyType.Boss)? LIFE_DEC_BOSS : LIFE_DEC_MONSTER;
            GM._.gui.HeartFillImg.fillAmount = (float)GM._.Life / GM._.MaxLife;
            GM._.gui.LifeTxt.text = GM._.Life.ToString();
            //* ゲームオーバ
            if(GM._.Life <= 0) {
                GM._.State = GameState.Gameover;
                GM._.Life = 0;
                Debug.Log("GAMEOVER");
            }
        }
    }

    #region FUNC
        private void Release() => GM._.em.Pool.Release(this); //* 戻す
        public void Init() {
            Speed = originSpd;
            Hp = maxHp;
            HpBar.value = (float)Hp / maxHp;
            NodeIdx = 0;
            Util._.SetDefMt(SprRdr);
        }
        public void DecreaseHp(int val) {
            Util._.Blink(SprRdr);
            Hp -= val;
            HpBar.value = (float)Hp / maxHp;
            if(Hp <= 0) {
                Hp = 0;
                Die();
            }
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
            yield return Util.Time2;
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
