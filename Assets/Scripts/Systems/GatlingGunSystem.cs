

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct GatlingGunSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (playerTransform, playerDirection, _) in SystemAPI
                         .Query<RefRO<LocalTransform>, RefRO<Direction>, RefRO<PlayerTag>>())

        {
            foreach (var (shooter, cooldown) in SystemAPI.Query<RefRW<GatlingGunWeapon>, RefRW<Cooldown>>())
            {
                if (cooldown.ValueRO.timer <= 0)
                {
                    var bullet = state.EntityManager.Instantiate(shooter.ValueRO.projectile);

                    var bulletPosition = playerTransform.ValueRO.Position;

                    var randomRotation = quaternion.AxisAngle(new float3(0, 0, -1), shooter.ValueRW.random.NextFloat(
                             math.radians(-shooter.ValueRO.spread), math.radians(shooter.ValueRO.spread)
                        ));




                    var bulletDirection =
                            math.normalizesafe(new float3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                                                          Camera.main.ScreenToWorldPoint(Input.mousePosition).y,
                                                          playerTransform.ValueRO.Position.z) -
                                               playerTransform.ValueRO.Position);

                    var rotatedDirection = math.mul(randomRotation, bulletDirection);

                    state.EntityManager.SetComponentData(bullet, LocalTransform.FromPosition(bulletPosition));
                    state.EntityManager.SetComponentData(bullet, new Direction { direction = rotatedDirection });


                    float difToMax = (cooldown.ValueRO.cooldownTime - shooter.ValueRO.minCooldown);

                    float newCooldown = cooldown.ValueRO.cooldownTime - shooter.ValueRO.cooldownDecrease * difToMax;
                    if (newCooldown >= shooter.ValueRO.minCooldown)
                    {
                        cooldown.ValueRW.cooldownTime = newCooldown;
                        cooldown.ValueRW.timer = newCooldown;
                    }

                    float newSpread = shooter.ValueRO.maxSpread * (1 - (difToMax / cooldown.ValueRO.initialCooldown));

                    if (newSpread <= shooter.ValueRO.maxSpread)
                    {
                        shooter.ValueRW.spread = newSpread;
                    }
                }
            }
        }
    }

}
