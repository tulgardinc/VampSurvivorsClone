using Unity.Entities;
using UnityEngine;

public class DamageFlashData : IComponentData
{
    public Material defaultMaterial;
    public Material flashMaterial;
    public float flashDuration;
}