using System;
using Unity.Entities;

public struct OrbSpawnerPrefabs : IComponentData
{

    public Entity highXpOrbPrefab;
    public Entity lowXpOrbPrefab;
    public Entity mediumXpOrbPrefab;
    public Unity.Mathematics.Random random;

}