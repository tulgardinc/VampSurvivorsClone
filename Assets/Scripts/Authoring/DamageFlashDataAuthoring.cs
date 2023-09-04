using Unity.Entities;
using UnityEngine;

public class DamageFlashDataAuthoring : MonoBehaviour
{
    public Material defaultMaterial;
    public Material flashMaterial;
    public float flashDuration;

    public class Baker : Baker<DamageFlashDataAuthoring>
    {
        public override void Bake(DamageFlashDataAuthoring authoring)
        {
            var e = GetEntity(TransformUsageFlags.None);
            AddComponentObject(e, new
            DamageFlashData
            {
                defaultMaterial = authoring.defaultMaterial,
                flashMaterial = authoring.flashMaterial,
                flashDuration = authoring.flashDuration,
            });
        }
    }
}
