using Unity.Mathematics;

namespace Survivors.Play.Utilities
{
	public static class QuaternionExtensions
	{
		public static quaternion RotateTowards(this quaternion from, quaternion to, float maxDegreesDelta)
		{
			return math.slerp(from, to, math.radians(maxDegreesDelta));
		}
	}
}