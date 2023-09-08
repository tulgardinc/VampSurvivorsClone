using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CooldownAuthoring : MonoBehaviour
{
    public float cooldown;

    public class CoolDownBaker : Baker<CooldownAuthoring>
    {
        public override void Bake(CooldownAuthoring authoring)
        {

            AddComponent(GetEntity(TransformUsageFlags.None), new Cooldown
            {
                initialCooldown = authoring.cooldown,
                cooldownTime = authoring.cooldown
            });
        }
    }
}


