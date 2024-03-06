using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using TMPro;

public enum GameEF {
    //* Object
    DmgTxtEF,
    //* UI
}

public class GameEffectManager : MonoBehaviour {
    List<ObjectPool<GameObject>> pool = new List<ObjectPool<GameObject>>();

    //* Pool Type
    [field:SerializeField] public GameObject DmgTxtEF;

    //* Active Type

    void Awake() {
        pool.Add(InitEF(DmgTxtEF, max: 10));
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
        ef.GetComponentInChildren<TextMeshPro>().text = dmg.ToString();
        yield return Util.Time0_75;
        pool[idx].Release(ef);
    }
#endregion
}
