using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
/// <summary>
/// 敵SOデータのInspectorViewカスタマイズ
/// </summary>
[CustomEditor(typeof(SettingEnemyData))]
public class SettingEnemyDataEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SettingEnemyData dt = (SettingEnemyData)target;

        //* レベル昇順適用ボタン
        if(GUILayout.Button("レベル昇順適用")) {
            int i = 0;
            foreach (var enemyDt in dt.EnemyDatas) {
                enemyDt.Lv = ++i;
                EditorUtility.SetDirty(dt); // 변경 내용을 저장
            } 
        }

    }
}
#endif