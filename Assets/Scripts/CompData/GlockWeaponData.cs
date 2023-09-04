using Unity.Entities;

public struct WeaponData : IComponentData
{
    public Entity owner;
    public Entity bullet;
    public float speed;
    public float fireRate;
}
