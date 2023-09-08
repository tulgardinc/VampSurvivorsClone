using System.Runtime.InteropServices;
using Unity.Entities;
using UnityEngine;

public partial struct CooldownSystem : ISystem
{

    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new CooldownJob
        {
            deltaTime = Time.deltaTime
        }.ScheduleParallel(state.Dependency);
    }


    [StructLayout(LayoutKind.Auto)]
    public partial struct CooldownJob : IJobEntity
    {

        public float deltaTime;

        public void Execute(ref Cooldown cooldown)
        {
            if (cooldown.timer > 0)
            {
                cooldown.timer -= deltaTime;
            }
        }

    }

}