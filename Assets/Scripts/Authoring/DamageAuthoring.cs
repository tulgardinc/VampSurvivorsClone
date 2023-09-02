
using Unity.Entities;
using UnityEngine;

public class DamageAuthoring : MonoBehaviour
{
    public float damage;
}

public class DamageBaker : Baker<DamageAuthoring>
{
    public override void Bake(DamageAuthoring authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.Dynamic), new Damage
        {
            damage = authoring.damage
        });
    }
}
