using Unity.Entities;
using UnityEngine;

public class SpeedAuthoring : MonoBehaviour
{

    public float speed;
    public float maxSpeed;

}

public class SpeedBaker : Baker <SpeedAuthoring>
{

    public override void Bake(SpeedAuthoring authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.Dynamic),
                     new Speed { speed = authoring.speed, maxSpeed = authoring.speed });
    }

}