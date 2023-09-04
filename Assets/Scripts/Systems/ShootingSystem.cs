using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct ShootingSystem : ISystem
{

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (playerTransform, playerDirection, _) in SystemAPI
                         .Query <RefRO <LocalTransform>, RefRO <Direction>, RefRO <PlayerTag>>())

        {
            foreach (var shooter in SystemAPI.Query <RefRW <BulletSpawner>>())
            {
                if (shooter.ValueRO.fireTimer < SystemAPI.Time.ElapsedTime)
                {
                    var bullet = state.EntityManager.Instantiate(shooter.ValueRO.bullet);

                    var bulletPosition = playerTransform.ValueRO.Position;

                    var bulletDirection =
                            math.normalizesafe(new float3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                                                          Camera.main.ScreenToWorldPoint(Input.mousePosition).y,
                                                          playerTransform.ValueRO.Position.z) -
                                               playerTransform.ValueRO.Position);


                    //var bulletDirection = playerDirection.ValueRO.direction;
                    //
                    // if (bulletDirection.Equals(float3.zero))
                    // {
                    //     bulletDirection = playerDirection.ValueRO.previousDirection;
                    // }
                    //
                    // if (bulletDirection.Equals(float3.zero))
                    // {
                    //     bulletDirection = math.right();
                    // }
                    state.EntityManager.SetComponentData(bullet, LocalTransform.FromPosition(bulletPosition));
                    state.EntityManager.SetComponentData(bullet, new Direction { direction = bulletDirection });


                    shooter.ValueRW.fireTimer = (float)SystemAPI.Time.ElapsedTime + shooter.ValueRO.fireInterval;
                }
            }
        }
    }

}