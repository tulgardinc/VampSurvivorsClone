
using System.Collections.Generic;
using Unity.Entities;

public struct InCollisoinWith : IComponentData
{
    public List<Entity> entites;
}
