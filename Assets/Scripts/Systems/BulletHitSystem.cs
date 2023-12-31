using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

public partial struct BulletHitSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        foreach (var (bulletHit, health, entity) in
                 SystemAPI.Query <RefRO <BulletHit>, RefRW <Health>>().WithEntityAccess())
        {
            health.ValueRW.health -= bulletHit.ValueRO.damage;
            ecb.RemoveComponent <BulletHit>(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

}