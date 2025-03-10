using System.Collections.Generic;
using Latios;
using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring.Enemies
{
    public class DeathAudioClipsAuthoring : MonoBehaviour
    {
        [SerializeField] List<GameObject> audioPrefabs;
        
        private class DeathAudioClipsAuthoringBaker : Baker<DeathAudioClipsAuthoring>
        {
            public override void Bake(DeathAudioClipsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var buffer = AddBuffer<DeathClipsBufferElement>(entity);
                foreach (var audioPrefab in authoring.audioPrefabs)
                {
                    buffer.Add(new DeathClipsBufferElement
                    {
                        AudioPrefab = GetEntity(audioPrefab, TransformUsageFlags.Dynamic)
                    });
                }
            }
        }
    }
    
    public struct DeathClipsBufferElement : IBufferElementData
    {
        public EntityWith<Prefab> AudioPrefab;
    }
}

