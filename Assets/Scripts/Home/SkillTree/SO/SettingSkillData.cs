using UnityEngine;

[System.Serializable]
public class SkillTreeData {
    [field:SerializeField] public string Name {get; set;}
    [field:SerializeField] public string Description {get; set;}
    [field:SerializeField] public int Lv {get; set;}
    [field:SerializeField] public int Cost {get; set;}
    [field:SerializeField] public float Val {get; set;}

    public SkillTreeData(string name, string description, int lv, int cost, float val) {
        Name = name;
        Description = description;
        Lv = lv;
        Cost = cost;
        Val = val;
    }
}

[CreateAssetMenu(fileName = "SkillTreeDataSO", menuName = "Scriptable Object/Setting SkillData/SkillTreeData")]
public class SettingSkillTreeData : ScriptableObject {
    [field:SerializeField] public SkillTreeData[] Datas {get; set;}

}
