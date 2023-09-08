using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public partial struct DamageFlashingSystem : ISystem
{

    public void OnStart(ref SystemState state) { state.RequireForUpdate <DamageFlashData>(); }

    public void OnUpdate(ref SystemState state)
    {
        var damageFlashData = SystemAPI.ManagedAPI.GetSingleton <DamageFlashData>();

        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        new DamageFlashingJob
        {
            damageFlashData = damageFlashData,
            ecb = ecb,
            deltaTime = Time.deltaTime
        }.Run();


        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [StructLayout(LayoutKind.Auto)]
    public partial struct DamageFlashingJob : IJobEntity
    {

        public DamageFlashData damageFlashData;
        public EntityCommandBuffer ecb;
        public float deltaTime;

        private void Execute(SpriteRenderer renderer, ref DamageFlashing flashing, Entity entity)
        {
            if (flashing.flashTimer >= damageFlashData.flashDuration)
            {
                renderer.material = damageFlashData.defaultMaterial;
                ecb.RemoveComponent <DamageFlashing>(entity);

                return;
            }

            if (!renderer.material.Equals(damageFlashData.flashMaterial))
            {
                renderer.material = damageFlashData.flashMaterial;
            }

            flashing.flashTimer += deltaTime;
        }

    }

}