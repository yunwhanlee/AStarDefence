using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Enemy {
    public Monster(string name, int lv, int hp, float speed) {
        Name = name;
        Lv = lv;
        Hp = hp;
        Speed = speed;
    }
}
