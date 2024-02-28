using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Pool;

public class MissileManager : MonoBehaviour {
    const int CREATE_MAX_CNT = 100;

    public Transform missileObjGroup;
    public Missile missilePf;
    IObjectPool<Missile> pool;    public IObjectPool<Missile> Pool {get => pool;}

    void Awake() => pool = new ObjectPool<Missile>(
        create, onGet, onRelease, onDestroyBlock, maxSize: CREATE_MAX_CNT
    );

#region OBJECT POOL
    private Missile create() {
        Missile enemy = Instantiate(missilePf, missileObjGroup);
        return enemy;
    }
    private void onGet(Missile missile) { //* 使う
        missile.gameObject.SetActive(true);
    }
    private void onRelease(Missile missile) { //* 戻す
        missile.gameObject.SetActive(false);
    }
    private void onDestroyBlock(Missile missile) => Destroy(missile);
#endregion

#region FUNC
    public Missile CreateMissile() => pool.Get();
#endregion
}
