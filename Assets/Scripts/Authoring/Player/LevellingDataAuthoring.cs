using Unity.Entities;
using UnityEngine;

public class LevellingDataAuthoring : MonoBehaviour
{
    public float currentLevel;
    public float currentXP;
    public float xpToNextLevel;
    public float requiredXPMult;
}

public class LevellingBaker : Baker<LevellingDataAuthoring>
{
    public override void Bake (LevellingDataAuthoring authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.Dynamic), new LevellingData { currentLevel = authoring.currentLevel, currentXP = authoring.currentXP, requiredXPMult = authoring.requiredXPMult, xpToNextLevel = authoring.xpToNextLevel });
    }
}