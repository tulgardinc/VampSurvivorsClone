using Unity.Entities;

public struct LevellingData : IComponentData
{
    public float currentLevel;
    public float currentXP;
    public float xpToNextLevel;
    public float requiredXPMult;
}
