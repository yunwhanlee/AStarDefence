using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using TMPro;

public enum GameEF {
    //* Object
    DmgTxtEF,
    RefundTxtEF,
    //* UI
}

public class GameEffectManager : MonoBehaviour {
    List<ObjectPool<GameObject>> pool = new List<ObjectPool<GameObject>>();

    //* Pool Type
    [field:SerializeField] public GameObject DmgTxtEF;
    [field: SerializeField] public GameObject RefundTxtEF;

    //* Active Type

    void Awake() {
        pool.Add(InitEF(DmgTxtEF, max: 10));
        pool.Add(InitEF(RefundTxtEF, max: 3));
    }

#region POOL
    private ObjectPool<GameObject> InitEF(GameObject obj, int max){
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
    public void ShowDmgTxtEF(Vector3 pos, int dmg) => StartCoroutine(CoShowDmgTxtEF(pos, dmg));
    IEnumerator CoShowDmgTxtEF(Vector3 pos, int dmg) { //bool isCritical) {
        int idx = (int)GameEF.DmgTxtEF;//isCritical? (int)IDX.CriticalDmgTxtEF : (int)IDX.DmgTxtEF;
        GameObject ef = pool[idx].Get();
        ef.transform.position = pos;
        ef.GetComponentInChildren<TextMeshPro>().text = $"{dmg}";
        yield return Util.Time0_75;
        pool[idx].Release(ef);
    }

    public void ShowRefundTxtEF(Vector3 pos, int val) => StartCoroutine(CoShowRefundTxtEF(pos, val));
    IEnumerator CoShowRefundTxtEF(Vector3 pos, int val)
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
