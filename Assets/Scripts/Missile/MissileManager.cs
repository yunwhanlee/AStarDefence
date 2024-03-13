using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Pool;

public enum PassArrowIdx {
    Red, Blue, None
}

public class MissileManager : MonoBehaviour {
    const int CREATE_MAX_CNT = 100;
    public Transform missileObjGroup;
    public Missile missilePf;
    public PassArrow passArrowRedPf;
    public PassArrow passArrowBluePf;

    IObjectPool<Missile> pool;    public IObjectPool<Missile> Pool {get => pool;}
    List<IObjectPool<PassArrow>> passArrowPoolList;     public List<IObjectPool<PassArrow>> PassArrowPoolList {get => passArrowPoolList;}

    void Awake() {
        passArrowPoolList = new List<IObjectPool<PassArrow>>(); //* リスト 初期化
        pool = new ObjectPool<Missile>(create, onGet, onRelease, onDestroyBlock, maxSize: CREATE_MAX_CNT);
        passArrowPoolList.Add(InitPassArrow(passArrowRedPf, 2));
        passArrowPoolList.Add(InitPassArrow(passArrowBluePf, 2));
    } 

#region OBJECT POOL
    private ObjectPool<PassArrow> InitPassArrow(PassArrow passArrowPf, int max) {
        return new ObjectPool<PassArrow>(() => 
            create2(passArrowPf), //* 生成
            onGet, //* 呼出
            onRelease, //* 戻し
            onDestroyBlock, 
            maxSize : max //* 最大生成回数
        );
    }

    //* 生成
    private Missile create() => Instantiate(missilePf, missileObjGroup);
    private PassArrow create2(PassArrow passArrowPf) => Instantiate(passArrowPf, missileObjGroup);
    //* 使う
    private void onGet(Missile missile) => missile.gameObject.SetActive(true);
    private void onGet(PassArrow passArrow) => passArrow.gameObject.SetActive(true);
    //* 戻す
    private void onRelease(Missile missile) => missile.gameObject.SetActive(false);
    private void onRelease(PassArrow passArrow) => passArrow.gameObject.SetActive(false);
    //* 破壊
    private void onDestroyBlock(Missile missile) => Destroy(missile);
    private void onDestroyBlock(PassArrow passArrow) => Destroy(passArrow);
#endregion

#region FUNC
    public Missile CreateMissile() => pool.Get();
    public PassArrow CreatePassArrow(PassArrowIdx passArrowEnum) => passArrowPoolList[(int)passArrowEnum].Get();
#endregion
}
