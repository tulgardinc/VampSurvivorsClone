using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class LinearProjectileTagAuthoring : MonoBehaviour
{

    public class LinearProjectileTagBaker : Baker<LinearProjectileTagAuthoring>
    {
        public override void Bake(LinearProjectileTagAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new LinearProjectileTag { });
        }
    }
}
