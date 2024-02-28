using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Missile : MonoBehaviour {
    [field:SerializeField] public Tower MyTower {get; set;}
    [field:SerializeField] public Transform Target {get; set;}
    private Vector2 dir;
    private const float speed = 5;

    void Start() {
        dir = Target.position - transform.position;
        dir = dir.normalized;
    }
    void Update() {
        transform.Translate(dir * speed * Time.deltaTime);
    }

    #region COLLISION
        void OnTriggerEnter2D(Collider2D other) {
            if(other.gameObject.layer == Enum.Layer.Enemy) {
                Enemy enemy = other.GetComponent<Enemy>();
                Debug.Log("Enemy.Name= " + enemy.name);
                enemy.DecreaseHp(MyTower.Dmg);
                Destroy(gameObject);
            }
        }
    #endregion
}
