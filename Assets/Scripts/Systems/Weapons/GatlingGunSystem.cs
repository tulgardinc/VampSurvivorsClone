using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct GatlingGunSystem : ISystem
{

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (playerTransform, playerSpeed, _) in SystemAPI
                         .Query <RefRO <LocalTransform>, RefRW <Speed>, RefRO <PlayerTag>>())

        {
            foreach (var (shooter, cooldown) in SystemAPI.Query <RefRW <GatlingGunWeapon>, RefRW <Cooldown>>())
            {
                if (Input.GetMouseButton(0))
                {
                    if (cooldown.ValueRO.timer <= 0)
                    {
                        var bullet = state.EntityManager.Instantiate(shooter.ValueRO.projectile);

                        var bulletPosition = playerTransform.ValueRO.Position;

                        var randomRotation = quaternion.AxisAngle(new float3(0, 0, -1),
                                                                  shooter.ValueRW.random.NextFloat(
                                                                       math.radians(-shooter.ValueRO.spread),
                                                                       math.radians(shooter.ValueRO.spread)
                                                                      ));


                        if (Camera.main != null)
                        {
                            var bulletDirection =
                                    math.normalizesafe(new float3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                                                                  Camera.main.ScreenToWorldPoint(Input.mousePosition).y,
                                                                  playerTransform.ValueRO.Position.z) -
                                                       playerTransform.ValueRO.Position);

                            var rotatedDirection = math.mul(randomRotation, bulletDirection);

                            state.EntityManager.SetComponentData(bullet, LocalTransform.FromPosition(bulletPosition));
                            state.EntityManager.SetComponentData(bullet,
                                                                 new Direction { direction = rotatedDirection });
                        }


                        var difToMax = cooldown.ValueRO.cooldownTime - shooter.ValueRO.minCooldown;

                        var newCooldown = cooldown.ValueRO.cooldownTime - shooter.ValueRO.cooldownDecrease * difToMax;
                        if (newCooldown >= shooter.ValueRO.minCooldown)
                        {
                            cooldown.ValueRW.cooldownTime = newCooldown;
                            cooldown.ValueRW.timer = newCooldown;
                        }

                        var newSpread = shooter.ValueRO.maxSpread * (1 - difToMax / cooldown.ValueRO.initialCooldown);

                        if (newSpread <= shooter.ValueRO.maxSpread)
                        {
                            shooter.ValueRW.spread = newSpread;
                        }


                        var linearInterpolation = (cooldown.ValueRO.cooldownTime - shooter.ValueRO.minCooldown) /
                                                  (cooldown.ValueRO.initialCooldown - shooter.ValueRO.minCooldown);
                        var decreaseInMs = 3 * (1 - linearInterpolation);
                        playerSpeed.ValueRW.speed = playerSpeed.ValueRO.maxSpeed - decreaseInMs;
                    }
                }
                else
                {
                    cooldown.ValueRW.cooldownTime = cooldown.ValueRO.initialCooldown;
                    shooter.ValueRW.spread = shooter.ValueRO.minSpread;
                    playerSpeed.ValueRW.speed = playerSpeed.ValueRO.maxSpeed;
                }
            }
        }
    }

}