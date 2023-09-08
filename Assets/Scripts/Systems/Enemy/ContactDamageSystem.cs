using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct ContactDamageSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate <PlayerTag>();
        state.RequireForUpdate <SimulationSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var player = SystemAPI.GetSingletonEntity <PlayerTag>();

        state.Dependency = new BulletCollisionJob
        {
            contactDamageLookup = SystemAPI.GetComponentLookup <ContactDamage>(),
            healthLookup = SystemAPI.GetComponentLookup <Health>(),
            player = player
        }.Schedule(
                   SystemAPI.GetSingleton <SimulationSingleton>(), state.Dependency);

        new ContactCooldownJob
        {
            deltaTime = Time.deltaTime
        }.ScheduleParallel();
    }

    private struct BulletCollisionJob : ITriggerEventsJob
    {

        public ComponentLookup <ContactDamage> contactDamageLookup;
        public ComponentLookup <Health> healthLookup;
        public Entity player;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            Entity playerTrigger;
            Entity enemyTrigger;

            if (entityA.Equals(player) && HasContactDamage(entityB))
            {
                playerTrigger = entityA;
                enemyTrigger = entityB;
            }
            else if (entityB.Equals(player) && HasContactDamage(entityA))
            {
                playerTrigger = entityB;
                enemyTrigger = entityA;
            }
            else
            {
                return;
            }

            var enemyDamage = GetContactDamage(enemyTrigger).ValueRO.damage;

            if (GetContactDamage(enemyTrigger).ValueRW.attackTimer <= 0)
            {
                GetHealth(playerTrigger).ValueRW.health -= enemyDamage;
                GetContactDamage(enemyTrigger).ValueRW.attackTimer += 1;
            }
        }

        private RefRW <ContactDamage> GetContactDamage(Entity entity) { return contactDamageLookup.GetRefRW(entity); }

        private RefRW <Health> GetHealth(Entity entity) { return healthLookup.GetRefRW(entity); }
        private bool HasContactDamage(Entity entity) { return contactDamageLookup.HasComponent(entity); }

    }


    [StructLayout(LayoutKind.Auto)]
    public partial struct ContactCooldownJob : IJobEntity
    {

        public float deltaTime;

        private void Execute(ref ContactDamage contactDamage)
        {
            if (contactDamage.attackTimer > 0)
            {
                contactDamage.attackTimer -= deltaTime;
            }
        }

    }

}