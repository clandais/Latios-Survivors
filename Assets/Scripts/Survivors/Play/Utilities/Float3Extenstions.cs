using Unity.Mathematics;

namespace Survivors.Play.Utilities
{
	public static class Float3Extensions
	{
		public static float3 MoveTowards(this float3 current, float3 target, float maxDistanceDelta)
		{
			float3 a = target - current;
			float magnitude = math.length(a);
			if (magnitude <= maxDistanceDelta || magnitude < float.Epsilon)
			{
				return target;
			}
			return current + a / magnitude * maxDistanceDelta;
		}
	}

}