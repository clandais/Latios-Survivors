using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Components
{

	public struct DeadTag : IComponentData { }

	public struct HitInfos : IComponentData
	{
		public float3 Position;
		public float3 Normal;
		public Entity Entity;
	}
}