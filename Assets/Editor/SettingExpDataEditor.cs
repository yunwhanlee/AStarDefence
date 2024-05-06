using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
/// <summary>
/// EXP SOデータのInspectorViewカスタマイズ
/// </summary>
[CustomEditor(typeof(SettingExpData))]
public class SettingExpDataEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        SettingExpData expDt = (SettingExpData)target;

        if(GUILayout.Button("レベルEXP適用")) {
            float standardRatio = 1.0f;
            for(int i = 0; i < expDt.Datas.Length; i++) {
                standardRatio += expDt.StandardDeviationIncRate;
                Debug.Log($"レベルEXP適用:: i({i}) standardRatio= {standardRatio}");
                int val = (i == 0)? expDt.Origin
                    : expDt.Datas[i - 1].Max + expDt.Deviation + Mathf.RoundToInt(standardRatio);
                expDt.Datas[i].Max = val;
                EditorUtility.SetDirty(expDt); // 변경 내용을 저장
            }
        }
    }
}
#endif