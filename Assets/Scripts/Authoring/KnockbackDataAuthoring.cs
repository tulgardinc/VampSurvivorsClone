using Unity.Entities;
using UnityEngine;

public class KnockbackDataAuthoring : MonoBehaviour
{

    public float knockbackAmount;

    public class KnockbackDataBaker : Baker <KnockbackDataAuthoring>
    {

        public override void Bake(KnockbackDataAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new KnockbackData { knockbackAmount = authoring.knockbackAmount });
        }

    }

}