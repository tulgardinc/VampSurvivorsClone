using Unity.Entities;
using UnityEngine;

public class ExplodeOnImpactAuthoring : MonoBehaviour
{

    public class ExplodeOnImpactBaker : Baker <ExplodeOnImpactAuthoring>
    {

        public override void Bake(ExplodeOnImpactAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent <ExplodeOnImpact>(entity);
        }

    }

}