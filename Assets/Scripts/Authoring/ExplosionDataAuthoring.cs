using Unity.Entities;
using UnityEngine;

public class ExplosionDataAuthoring : MonoBehaviour
{

    public float explosionTimer;
    public float explosionRange;
    public float explosionDamage;
    public float explosionKnockback;

    public class ExplosionDataBaker : Baker <ExplosionDataAuthoring>
    {

        public override void Bake(ExplosionDataAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                         new ExplosionData
                         {
                             explosionTimer = authoring.explosionTimer,
                             explosionRange = authoring.explosionRange,
                             explosionDamage = authoring.explosionDamage,
                             explosionKnockback = authoring.explosionKnockback
                         });
        }

    }

}