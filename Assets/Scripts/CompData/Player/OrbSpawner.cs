using System;
using Unity.Entities;

public struct OrbSpawner : IComponentData
{

    public Entity highXpOrbPrefab;
    public Entity lowXpOrbPrefab;
    public Entity mediumXpOrbPrefab;
    public Unity.Mathematics.Random random;
    public float orbSpawnInitialSpeed;
    public float orbSpawnDeceleration;
    public float maxSpeed;
    public float acceleration;
}