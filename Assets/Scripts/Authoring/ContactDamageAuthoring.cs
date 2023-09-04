using Unity.Entities;
using UnityEngine;

public class ContactDamageAuthoring : MonoBehaviour
{

    public float damage;
    public float attackTimer;

    public class ContactDamageBaker : Baker <ContactDamageAuthoring>
    {

        public override void Bake(ContactDamageAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ContactDamage { damage = authoring.damage, attackTimer = authoring.attackTimer });
        }

    }

}