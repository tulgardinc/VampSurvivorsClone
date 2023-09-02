using Unity.Entities;

public partial struct EnemyDeath : ISystem
{

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer();

        foreach (var (enemyHealth, enemy) in SystemAPI.Query <RefRO <Health>>().WithEntityAccess())
        {
            if (enemyHealth.ValueRO.health <= 0)
            {
                ecb.DestroyEntity(enemy);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

}