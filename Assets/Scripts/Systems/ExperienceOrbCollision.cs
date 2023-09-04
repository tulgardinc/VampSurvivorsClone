using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct ExperienceOrbCollision : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate <SimulationSingleton>();
        state.RequireForUpdate <EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new BulletCollisionJob
        {
            experienceLookup = SystemAPI.GetComponentLookup <Experience>(true),
            levelLookup = SystemAPI.GetComponentLookup <LevellingData>(),
            commandBuffer = SystemAPI.GetSingleton <EndSimulationEntityCommandBufferSystem.Singleton>()
                                     .CreateCommandBuffer(state.WorldUnmanaged)
        }.Schedule(
                   SystemAPI.GetSingleton <SimulationSingleton>(), state.Dependency);
    }

    private struct BulletCollisionJob : ITriggerEventsJob
    {

        [ReadOnly] public ComponentLookup <Experience> experienceLookup;
        public ComponentLookup <LevellingData> levelLookup;
        public EntityCommandBuffer commandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            Entity experience;
            Entity player;

            if (IsExperience(entityA) && IsPlayer(entityB))
            {
                experience = entityA;
                player = entityB;
            }
            else if (IsExperience(entityB) && IsPlayer(entityA))
            {
                experience = entityB;
                player = entityA;
            }
            else
            {
                return;
            }

            var experienceValue = GetExperience(experience);
            var playerExperience = GetLevellingData(player);

            playerExperience.ValueRW.currentXP += experienceValue;

            commandBuffer.DestroyEntity(experience);
        }

        private bool IsExperience(Entity entity) { return experienceLookup.HasComponent(entity); }

        private bool IsPlayer(Entity entity) { return levelLookup.HasComponent(entity); }

        private float GetExperience(Entity entity) { return experienceLookup[entity].experience; }

        private RefRW <LevellingData> GetLevellingData(Entity entity) { return levelLookup.GetRefRW(entity); }

    }

}