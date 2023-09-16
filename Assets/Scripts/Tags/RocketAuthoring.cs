using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class RocketAuthoring : MonoBehaviour
{
    public float amplitude;
    public float amplitudeSpeed;

    public class RocketBaker : Baker<RocketAuthoring>
    {
        public override void Bake(RocketAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new Rocket
            {
                amplitude = authoring.amplitude,
                amplitudeSpeed = authoring.amplitudeSpeed,
            });
        }
    }
}
