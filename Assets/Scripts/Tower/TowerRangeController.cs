using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TowerRangeController : MonoBehaviour {
    [field: SerializeField] public Tower Tower {get; set;}
    [field: SerializeField] public SpriteRenderer SprRdr {get; set;}
    [field: SerializeField] public Transform CurTarget {get; private set;}
    public List<Transform> TargetList; // 範囲内の敵の中で最も違うやつを狙うため
    private List<Transform> deadEnemyList; // ターゲットリストにあるのに、途中で死んだ敵を削除するため

    void Awake() {
        Tower = GetComponentInParent<Tower>();
        float atkRange = Tower.AtkRange;
        transform.localScale = new Vector3(atkRange, atkRange, atkRange);
        SprRdr = GetComponent<SpriteRenderer>();
        TargetList = new List<Transform>();
        deadEnemyList = new List<Transform>();
    }

    void OnTriggerEnter2D(Collider2D col) {
        //* 攻撃範囲内の敵をリストへ追加
        if(col.gameObject.layer == Enum.Layer.Enemy) {
            Debug.Log("TowerRangeController:: TriggerEnter::");
            TargetList.Add(col.transform);
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        //* 攻撃範囲内の敵をリストから消す
        if(col.gameObject.layer == Enum.Layer.Enemy)
            TargetList.Remove(col.transform);
    }

    void Update() {
        if(TargetList.Count > 0) {
            float closestDist = Mathf.Infinity;
            Transform closestTarget = null;

            //* もう死んだ敵を入れるリスト
            deadEnemyList = new List<Transform>(); 

            //* 一番違い敵を狙う
            foreach(Transform tg in TargetList) {
                // 敵が途中で死んだら、処置しない
                if(!tg.gameObject.activeSelf) {
                    Debug.Log($"Enemy Was Already Dead => {tg.name}");
                    deadEnemyList.Add(tg); // TargetList.Remove(tg);
                    continue;
                }

                // 一番近いターゲットにアップデート
                float distToTarget = Vector2.Distance(gameObject.transform.position, tg.position);
                if(distToTarget < closestDist) {
                    closestDist = distToTarget;
                    closestTarget = tg;
                }
            }

            //* 上のループが終わってから、死んだ敵をターゲットリストから削除
            deadEnemyList.ForEach(deadEnemy => {
                Debug.Log($"既に死んだ敵をターゲットリスから削除：{deadEnemy.name}");
                TargetList.Remove(deadEnemy);
            });

            //* 一番近いターゲットに更新
            if(closestTarget != null)
                CurTarget = closestTarget;
            else 
                CurTarget = null;
        }
        else {
            CurTarget = null;
        }

        //* 範囲にターゲットの色分け
        if(CurTarget) 
            SprRdr.color = new Color(1, 0, 0, 0.3f); // 赤
        else 
            SprRdr.color = new Color(1, 1, 0, 0.1f); // 黄
    }
}
