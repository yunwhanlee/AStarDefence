using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TowerRangeController : MonoBehaviour {
    [field: SerializeField] public Tower Tower {get; set;}
    [field: SerializeField] public SpriteRenderer SprRdr {get; set;}
    public List<Transform> TargetList = new List<Transform>();
    [field: SerializeField] public Transform CurTarget {get; private set;}

    void Awake() {
        Tower = GetComponentInParent<Tower>();
        float atkRange = Tower.AtkRange;
        transform.localScale = new Vector3(atkRange, atkRange, atkRange);
        SprRdr = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        //* 攻撃範囲内の敵をリストへ追加
        if(other.CompareTag(Enum.Tag.Enemy.ToString()))
            TargetList.Add(other.transform);
    }

    void OnTriggerExit2D(Collider2D other) {
        //* 攻撃範囲内の敵をリストから消す
        if(other.CompareTag(Enum.Tag.Enemy.ToString()))
            TargetList.Remove(other.transform);
    }

    void Update() {
        if(TargetList.Count > 0) {
            float closestDist = Mathf.Infinity;
            Transform closestTarget = null;

            foreach(Transform tg in TargetList) {
                //* 敵が途中で死んだら、処置しない
                if(tg == null) {
                    TargetList.Remove(tg);
                    continue;
                }

                //* 一番近いターゲットにアップデート
                float distToTarget = Vector2.Distance(gameObject.transform.position, tg.position);
                if(distToTarget < closestDist) {
                    closestDist = distToTarget;
                    closestTarget = tg;
                }
            }

            if(closestTarget != null && closestDist <= GetComponentInParent<Tower>().AtkRange) {
                CurTarget = closestTarget;
            }
            else {
                CurTarget = null;
            }
        }
        else {
            CurTarget = null;
        }
    }
}
