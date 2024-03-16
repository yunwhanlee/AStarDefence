using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using TMPro;

public enum GameEF {
    NULL = -1,
    //* Object
    DmgTxtEF,
    CritDmgTxtEF,
    RefundTxtEF,

    //* Magician
    ExplosionWindEF, // Lv 3
    ExplosionFireballPurpleEF, // Lv 4
    ExplosionFireballBlueEF, // Lv 5
    ExplosionFireballRedEF, // Lv 6

    //* UI
}

public class GameEffectManager : MonoBehaviour {
    List<ObjectPool<GameObject>> pool = new List<ObjectPool<GameObject>>();

    /* Pool Type */
    //* Text EF
    [field:SerializeField] public GameObject DmgTxtEF;
    [field:SerializeField] public GameObject CritDmgTxtEF;
    [field: SerializeField] public GameObject RefundTxtEF;

    //* Magician EF
    [field: SerializeField] public GameObject ExplosionWindEF;
    [field: SerializeField] public GameObject ExplosionFireballPurpleEF;
    [field: SerializeField] public GameObject ExplosionFireballBlueEF;
    [field: SerializeField] public GameObject ExplosionFireballRedEF;

    void Awake() {
        pool.Add(InitEF(DmgTxtEF, max: 75));
        pool.Add(InitEF(CritDmgTxtEF, max: 30));
        pool.Add(InitEF(RefundTxtEF, max: 3));
        pool.Add(InitEF(ExplosionWindEF, max: 3));
        pool.Add(InitEF(ExplosionFireballPurpleEF, max: 2));
        pool.Add(InitEF(ExplosionFireballBlueEF, max: 1));
        pool.Add(InitEF(ExplosionFireballRedEF, max: 1));
    }

#region POOL
    private ObjectPool<GameObject> InitEF(GameObject obj, int max) {
        return new ObjectPool<GameObject>(
            () => InstantiateEF(obj), //* 生成
            OnGetEF, //* 呼出
            OnReleaseEF, //* 戻し
            Destroy, 
            maxSize : max //* 最大生成回数
        );
    }
    private GameObject InstantiateEF(GameObject obj) => Instantiate(obj, transform);
    private void OnGetEF(GameObject obj) => obj.SetActive(true);
    private void OnReleaseEF(GameObject obj) => obj.SetActive(false);
#endregion

#region SHOW EFFECT
    public void ShowEF(GameEF efIdx, Vector2 pos, WaitForSeconds delay = null) {
        StartCoroutine(CoShowEF(efIdx, pos, delay));
    }
    IEnumerator CoShowEF(GameEF efIdx, Vector2 pos, WaitForSeconds delay) {
        if(delay == null)
            delay = Util.Time1_5;

        GameObject ef = pool[(int)efIdx].Get();
        ef.transform.position = pos;
        yield return delay;
        pool[(int)efIdx].Release(ef);
    }

    public void ShowDmgTxtEF(Vector2 pos, int dmg, bool isCritical = false) => StartCoroutine(CoShowDmgTxtEF(pos, dmg, isCritical));
    IEnumerator CoShowDmgTxtEF(Vector2 pos, int dmg, bool isCritical) {
        int idx = isCritical? (int)GameEF.CritDmgTxtEF : (int)GameEF.DmgTxtEF;
        GameObject ef = pool[idx].Get();
        ef.transform.position = pos;
        ef.GetComponentInChildren<TextMeshPro>().text = $"{dmg}";
        yield return Util.Time0_75;
        pool[idx].Release(ef);
    }

    public void ShowRefundTxtEF(Vector2 pos, int val) => StartCoroutine(CoShowRefundTxtEF(pos, val));
    IEnumerator CoShowRefundTxtEF(Vector2 pos, int val)
    { //bool isCritical) {
        int idx = (int)GameEF.RefundTxtEF;
        GameObject ef = pool[idx].Get();
        ef.transform.position = pos;
        ef.GetComponentInChildren<TextMeshPro>().text = $"<sprite name=Meat>+{val}";
        yield return Util.Time0_75;
        pool[idx].Release(ef);
    }

    #endregion
}
