using Unity.Entities;
using UnityEngine;

public class SpeedAuthoring : MonoBehaviour
{
    public float speed;
}

public class SpeedBaker : Baker<SpeedAuthoring>
{
    public override void Bake (SpeedAuthoring authoring)
    {
        AddComponent(this.GetEntity(TransformUsageFlags.Dynamic), new Speed { speed = authoring.speed });
    }
}

