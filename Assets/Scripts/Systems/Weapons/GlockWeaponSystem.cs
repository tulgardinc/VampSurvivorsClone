using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct GlockWeaponSystem : ISystem
{

    public void OnUpdate(ref SystemState state)
    {

        foreach (var (playerTransform, playerDirection, _) in SystemAPI
                         .Query<RefRO<LocalTransform>, RefRO<Direction>, RefRO<PlayerTag>>())

        {
            foreach (var (shooter, cooldown) in SystemAPI.Query<RefRW<GlockWeapon>, RefRW<Cooldown>>())
            {
                if (cooldown.ValueRO.timer <= 0)
                {
                    var bullet = state.EntityManager.Instantiate(shooter.ValueRO.projectile);

                    var bulletPosition = playerTransform.ValueRO.Position;

                    var bulletDirection =
                            math.normalizesafe(new float3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                                                          Camera.main.ScreenToWorldPoint(Input.mousePosition).y,
                                                          playerTransform.ValueRO.Position.z) -
                                               playerTransform.ValueRO.Position);

                    state.EntityManager.SetComponentData(bullet, LocalTransform.FromPosition(bulletPosition));
                    state.EntityManager.SetComponentData(bullet, new Direction { direction = bulletDirection });


                    cooldown.ValueRW.timer = cooldown.ValueRO.cooldownTime;
                }
            }
        }
    }

}
