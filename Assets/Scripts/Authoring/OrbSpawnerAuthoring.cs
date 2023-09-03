using Unity.Entities;
using UnityEngine;

public class OrbSpawnerPrefabsAuthoring : MonoBehaviour
{

    public GameObject highXpOrbPrefab;
    public GameObject lowXpOrbPrefab;
    public GameObject mediumXpOrbPrefab;

    public class OrbSpawnerPrefabsBaker : Baker<OrbSpawnerPrefabsAuthoring>
    {

        public override void Bake(OrbSpawnerPrefabsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                         new OrbSpawnerPrefabs
                         {
                             highXpOrbPrefab =
                                     GetEntity(authoring.highXpOrbPrefab, TransformUsageFlags.Dynamic),
                             lowXpOrbPrefab =
                                     GetEntity(authoring.lowXpOrbPrefab, TransformUsageFlags.Dynamic),
                             mediumXpOrbPrefab =
                                     GetEntity(authoring.mediumXpOrbPrefab, TransformUsageFlags.Dynamic),
                             random = Unity.Mathematics.Random.CreateFromIndex((uint)Random.Range(0, 9999))
                         });
        }

    }

}