using Unity.Entities;
using UnityEngine;

public class LifeTimeAuthoring : MonoBehaviour
{
    public float totalLifeTime;
}

public class LifeTimeBaker : Baker<LifeTimeAuthoring>
{
    public override void Bake(LifeTimeAuthoring authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.Dynamic),
            new LifeTime
            {
                totalLifeTime = authoring.totalLifeTime,
                lifeTimeLeft = authoring.totalLifeTime,
            });
    }
}
