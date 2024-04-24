using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
/// <summary>
/// 敵SOデータのInspectorViewカスタマイズ
/// </summary>
[CustomEditor(typeof(SettingEnemyData))]
public class SettingEnemyDataEditor : Editor {
    #region HP_TEMPLATE
    readonly int[] tempEnemyHpDts = {
        10,
        15,
        23,
        31,
        43,
        55,
        71,
        87,
        107,
        3810,
        // 11 Wave
        130,
        143,
        158,
        174,
        192,
        212,
        234,
        258,
        284,
        12640,
        // 21 Wave
        313,
        345,
        380,
        418,
        460,
        506,
        557,
        613,
        675,
        35800,
        // 31 Wave
        743,
        818,
        900,
        990,
        1089,
        1198,
        1318,
        1450,
        1595,
        100440,
        // 41 Wave
        1755,
        1931,
        2125,
        2338,
        2572,
        2830,
        3114,
        3426,
        3769,
        274680,
        // 51 Wave
        4089,
        4261,
        4440,
        4618,
        4803,
        4996,
        5206,
        5571,
        5833,
        490000,
        // 61 Wave
        6125,
        6389,
        6651,
        6918,
        7188,
        7462,
        7739,
        8018,
        8307,
        730830,
        // 71 Wave
        8598,
        8891,
        9194,
        9516,
        9831,
        10146,
        10471,
        10796,
        11120,
        1145400
    };
    #endregion

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        SettingEnemyData dt = (SettingEnemyData)target;

        //* レベル昇順適用ボタン
        if(GUILayout.Button("レベル昇順適用")) {
            int i = 0;
            foreach (var enemyDt in dt.Waves) {
                enemyDt.Lv = ++i;
                EditorUtility.SetDirty(dt); // 변경 내용을 저장
            }
        }

        //* 체력 비율 적용 버튼
        if (GUILayout.Button("HP 比率適用")) {
            int i = 0;
            foreach (var enemyDt in dt.Waves) {
                // 입력된 값에 따라 체력을 비례적으로 증가시킴
                enemyDt.Hp = Mathf.RoundToInt(tempEnemyHpDts[i++] * dt.HpRatio);
                EditorUtility.SetDirty(dt); // 변경 내용 저장
            }
        }
    }
}
#endif