using Unity.Entities;
using UnityEngine;

public class ExperienceOrbDropperDataAuthoring : MonoBehaviour
{

    public float highXpAmount;
    public float lowXpAmount;
    public float mediumXpAmount;

    public class ExperienceOrbDropperDataBaker : Baker <ExperienceOrbDropperDataAuthoring>
    {

        public override void Bake(ExperienceOrbDropperDataAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                         new ExperienceOrbDropperData
                         {
                             highXpAmount = authoring.highXpAmount,
                             lowXpAmount = authoring.lowXpAmount,
                             mediumXpAmount = authoring.mediumXpAmount
                         });
        }

    }

}