using Unity.Entities;
using UnityEngine;

public class ExplosionControllerAuthoring : MonoBehaviour
{

    public GameObject explosionPrefab;

    public class ExplosionControllerBaker : Baker <ExplosionControllerAuthoring>
    {

        public override void Bake(ExplosionControllerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                         new ExplosionController
                         {
                             explosionPrefab = GetEntity(authoring.explosionPrefab, TransformUsageFlags.Dynamic)
                         });
        }

    }

}