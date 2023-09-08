using Unity.Entities;
using UnityEngine;

public class ExperienceAuthoring : MonoBehaviour
{

    public float experience;

    public class ExperienceBaker : Baker <ExperienceAuthoring>
    {

        public override void Bake (ExperienceAuthoring authoring)
        {
            var entity = GetEntity (TransformUsageFlags.Dynamic);
            AddComponent (entity, new Experience { experience = authoring.experience });
        }

    }

}