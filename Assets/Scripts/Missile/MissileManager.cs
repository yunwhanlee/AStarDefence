using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
// using UnityEditor.UIElements;

public enum MissileIdx {
    PassArrowRed, // LV 5
    PassArrowBlue, // LV 6
    ArrowRain,

    MagicCirclePurple, // LV 4
    MagicCircleBlue, // LV 5
    MagicCircleRed, // LV 6
    LaserBlue, // LV 5
    LaserRed, // LV 6
    BigBang, // LV 6
}

public class MissileManager : MonoBehaviour {
    const int CREATE_MAX_CNT = 100;
    public Transform missileObjGroup;
    public Missile missilePf;
    public PassArrow passArrowRedPf;
    public PassArrow passArrowBluePf;
    public ArrowRain arrowRainPf;

    public MagicCircle magicCirclePurplePf;
    public MagicCircle MagicCircleBluePf;
    public MagicCircle MagicCircleRedPf;
    public Laser LaserBluePf;
    public Laser LaserRedPf;

    IObjectPool<Missile> pool;    public IObjectPool<Missile> Pool {get => pool;}
    List<IObjectPool<GameObject>> poolList;     public List<IObjectPool<GameObject>> PoolList {get => poolList;}

    void Awake() {
        pool = new ObjectPool<Missile>(create, onGet, onRelease, onDestroyBlock, maxSize: CREATE_MAX_CNT);
        poolList = new List<IObjectPool<GameObject>>(); //* リスト 初期化
        poolList.Add(InitPassArrow(passArrowRedPf.gameObject, 2));
        poolList.Add(InitPassArrow(passArrowBluePf.gameObject, 1));
        poolList.Add(InitPassArrow(arrowRainPf.gameObject, 1));
        poolList.Add(InitPassArrow(magicCirclePurplePf.gameObject, 1));
        poolList.Add(InitPassArrow(MagicCircleBluePf.gameObject, 1));
        poolList.Add(InitPassArrow(MagicCircleRedPf.gameObject, 1));
        poolList.Add(InitPassArrow(LaserBluePf.gameObject, 1));
        poolList.Add(InitPassArrow(LaserRedPf.gameObject, 1));
    } 

#region OBJECT POOL
    private ObjectPool<GameObject> InitPassArrow(GameObject passArrowPf, int max) {
        return new ObjectPool<GameObject>(() => 
            create(passArrowPf), //* 生成
            onGet, //* 呼出
            onRelease, //* 戻し
            onDestroyBlock, 
            maxSize : max //* 最大生成回数
        );
    }

    //* 生成
    private Missile create() => Instantiate(missilePf, missileObjGroup);
    private GameObject create(GameObject obj) => Instantiate(obj, missileObjGroup);
    //* 使う
    private void onGet(Missile missile) => missile.gameObject.SetActive(true);
    private void onGet(GameObject obj) => obj.gameObject.SetActive(true);
    //* 戻す
    private void onRelease(Missile missile) => missile.gameObject.SetActive(false);
    private void onRelease(GameObject obj) => obj.gameObject.SetActive(false);
    //* 破壊
    private void onDestroyBlock(Missile missile) => Destroy(missile);
    private void onDestroyBlock(GameObject obj) => Destroy(obj);
#endregion

#region FUNC
    public Missile CreateMissile() => pool.Get();
    public GameObject CreateMissile(int idx) => poolList[idx].Get();
#endregion
}
