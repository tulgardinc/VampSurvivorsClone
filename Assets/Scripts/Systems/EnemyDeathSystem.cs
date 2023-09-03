using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

public partial struct EnemyDeath : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var entityExperienceOrbLookup = SystemAPI.GetComponentLookup <ExperienceOrbDropperData>(true);

        foreach (var experienceOrbs in SystemAPI.Query <RefRO <OrbSpawnerPrefabs>>())
        {
            foreach (var (enemyHealth, enemyPosition, enemy) in SystemAPI
                                                                .Query <RefRO <Health>, RefRO <LocalTransform>>()
                                                                .WithEntityAccess())
            {
                //Guard close for enemy health
                if (!(enemyHealth.ValueRO.health <= 0))
                {
                    continue;
                }


                //Check if enemy has possible experience orb drops
                if (entityExperienceOrbLookup.HasComponent(enemy))
                {
                    //Get enemies experience orb data
                    var experienceOrbData = entityExperienceOrbLookup[enemy];

                    //Spawn small xp orbs
                    for (var i = 0; i < experienceOrbData.lowXpAmount; i++)
                    {
                        //Orb spawn range calculation
                        var orbPosition = new float3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)
                                                     , 0);
                        var orbDirection = math.normalize(orbPosition);

                        //Instantiate small xp orb
                        var orb = ecb.Instantiate(experienceOrbs.ValueRO.lowXpOrbPrefab);

                        //Move small xp orb to position
                        ecb.SetComponent(orb,
                                         LocalTransform.FromPosition(enemyPosition.ValueRO.Position + orbDirection));
                    }
                }

                ecb.DestroyEntity(enemy);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

}