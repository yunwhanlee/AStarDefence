using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType {
    Monster, Boss
}

[System.Serializable]
public abstract class Enemy : MonoBehaviour {
    // public EnemyType Type {get;}
    [field: SerializeField] public string Name {get; set;}
    [field: SerializeField] public int Lv {get; set;}
    [field: SerializeField] public int Hp {get; set;}
    [field: SerializeField] public int Speed {get; set;}
}
