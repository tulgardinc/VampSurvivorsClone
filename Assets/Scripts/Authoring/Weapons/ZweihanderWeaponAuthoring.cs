using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ZweihanderAuthoring : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] float sweepingAngle;
    [SerializeField] float damage;
    [SerializeField] float knockbackAmount;

    public class ZweihanderBaker : Baker<ZweihanderAuthoring>
    {
        public override void Bake(ZweihanderAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.None), new ZweihanderWeapon
            {
                radius = authoring.radius,
                sweepingAngle = authoring.sweepingAngle,
                damage = authoring.damage,
                knockbackAmount = authoring.knockbackAmount
            });
        }
    }
}
