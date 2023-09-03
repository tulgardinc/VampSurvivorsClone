using Unity.Entities;
using UnityEngine;

public class OrbSpawnerAuthoring : MonoBehaviour
{

    public GameObject highXpOrbPrefab;
    public GameObject lowXpOrbPrefab;
    public GameObject mediumXpOrbPrefab;
    public float orbSpawnInitialSpeed;
    public float orbSpawnDeceleration;

    public class OrbSpawnerPrefabsBaker : Baker<OrbSpawnerAuthoring>
    {

        public override void Bake(OrbSpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                         new OrbSpawner
                         {
                             highXpOrbPrefab =
                                     GetEntity(authoring.highXpOrbPrefab, TransformUsageFlags.Dynamic),
                             lowXpOrbPrefab =
                                     GetEntity(authoring.lowXpOrbPrefab, TransformUsageFlags.Dynamic),
                             mediumXpOrbPrefab =
                                     GetEntity(authoring.mediumXpOrbPrefab, TransformUsageFlags.Dynamic),
                             random = Unity.Mathematics.Random.CreateFromIndex((uint)Random.Range(0, 9999)),
                             orbSpawnInitialSpeed = authoring.orbSpawnInitialSpeed,
                             orbSpawnDeceleration = authoring.orbSpawnDeceleration
                         });
        }

    }

}
