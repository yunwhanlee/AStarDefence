using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Enemy {
    public Monster(string name, int lv, int hp, int speed) {
        this.Name = name;
        this.Lv = lv;
        this.Hp = hp;
        this.Speed = speed;
    }
}
