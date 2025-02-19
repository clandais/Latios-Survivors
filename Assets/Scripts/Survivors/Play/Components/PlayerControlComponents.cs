using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Components
{
	public struct PlayerInputState : IComponentData
	{
		public float2 Direction;
	}
}