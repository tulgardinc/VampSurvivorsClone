using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class WillBeKnockedBackAuthoring : MonoBehaviour
{

    public float totalKnockbackAmount;
    public float3 knockbackDirection;

    public class WillBeKnockedBackBaker : Baker <WillBeKnockedBackAuthoring>
    {

        public override void Bake(WillBeKnockedBackAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                         new WillBeKnockedBack
                         {
                             totalKnockbackAmount = authoring.totalKnockbackAmount,
                             knockbackDirection = authoring.knockbackDirection
                         });
        }

    }

}