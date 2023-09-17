using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Transforms;

public partial struct RocketExplosionSystem : ISystem
{

    private EntityQuery rocketQuery;

    public void OnCreate(ref SystemState state)
    {
        rocketQuery = state.GetEntityQuery(ComponentType.ReadWrite <LocalTransform>(), ComponentType.ReadOnly <Speed>(),
                                           ComponentType.ReadOnly <LifeTime>(),
                                           ComponentType.ReadOnly <RocketRandomShift>(),
                                           ComponentType.ReadOnly <Direction>(),
                                           ComponentType.ReadOnly <Rocket>());
    }

    public void OnUpdate(ref SystemState state) { new RocketExplosionJob().ScheduleParallel(rocketQuery); }


    [StructLayout(LayoutKind.Auto)]
    private partial struct RocketExplosionJob : IJobEntity
    {

        public float deltaTime;

        private static void Execute() { }

    }

}