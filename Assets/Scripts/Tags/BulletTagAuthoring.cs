using Unity.Entities;
using UnityEngine;

public class BulletTagAuthoring : MonoBehaviour
{

}

public class BulletTagBaker : Baker<BulletTagAuthoring>
{
    public override void Bake (BulletTagAuthoring authoring)
    {
        AddComponent(this.GetEntity(TransformUsageFlags.Dynamic), new BulletTag { });
    }
}