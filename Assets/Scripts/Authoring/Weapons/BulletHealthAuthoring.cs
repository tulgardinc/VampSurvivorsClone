using Unity.Entities;
using UnityEngine;

public class BulletHealthAuthoring : MonoBehaviour
{

    public float bulletHealth;

    public class BulletHealthBaker : Baker <BulletHealthAuthoring>
    {

        public override void Bake(BulletHealthAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BulletHealth { bulletHealth = authoring.bulletHealth });
        }

    }

}