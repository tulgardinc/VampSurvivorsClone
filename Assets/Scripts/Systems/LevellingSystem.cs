using Unity.Burst;
using Unity.Entities;

public partial struct LevellingSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (playerLevel, _) in SystemAPI.Query <RefRW <LevellingData>, RefRO <PlayerTag>>())
        {
            if (!(playerLevel.ValueRW.currentXP >= playerLevel.ValueRW.xpToNextLevel))
            {
                continue;
            }

            playerLevel.ValueRW.currentLevel += 1;
            playerLevel.ValueRW.currentXP -= playerLevel.ValueRW.xpToNextLevel;
            playerLevel.ValueRW.xpToNextLevel *= playerLevel.ValueRW.requiredXPMult;
        }
    }

}