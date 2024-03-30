using UnityEngine;

[System.Serializable]
public class SkillTreeData {
        [field:SerializeField] public string Name {get; private set;}
		[field:SerializeField] public string Description {get; private set;}
		[field:SerializeField] public int Lv {get; private set;}
		[field:SerializeField] public int Cost {get; private set;}
        [field:SerializeField] public float Val {get; private set;}
}

[CreateAssetMenu(fileName = "SkillTreeDataSO", menuName = "Scriptable Object/Setting SkillData/SkillTreeData")]
public class SettingSkillTreeData : ScriptableObject {
    [field:SerializeField] public SkillTreeData[] Datas {get; private set;}

}
