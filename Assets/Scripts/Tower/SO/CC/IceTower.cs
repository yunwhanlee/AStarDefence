using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTower : Tower
{
    public override void CheckMergeUI() => throw new System.NotImplementedException();
    public override bool Merge(TowerKind kind) => throw new System.NotImplementedException();

    public override void Upgrade() {
        GM._.tm.CreateTower(TowerType.CC_IceTower, Lv++);
        //* 自分を削除
        Destroy(gameObject);
    }
}
