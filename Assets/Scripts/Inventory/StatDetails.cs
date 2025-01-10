using Sirenix.OdinInspector;
using UnityEngine;

public enum StatCategory {
    None,
    Combat,
    Defence,
    Gathering,
    Crafting,
    Activity,
    Health
}

[CreateAssetMenu(fileName = "Stat", menuName = "Stat")]
public class StatDetails : SerializedScriptableObject {
    [SerializeField, BoxGroup("Settings")] 
    public string SkillName;
    
    [SerializeField, BoxGroup("Settings")] 
    public StatCategory Category;

    [SerializeField, BoxGroup("Settings"), PreviewField]
    public Sprite StatIcon;
}