using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring.Weapons
{
    public class AxeAuthoring : MonoBehaviour
    {
        private class AxeAuthoringBaker : Baker<AxeAuthoring>
        {
            public override void Bake(AxeAuthoring authoring)
            {
                var entiy = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<AxeComponent>(entiy);
            }
        }
    }
}

