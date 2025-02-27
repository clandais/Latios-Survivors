using Latios.Kinemation;
using Unity.Entities;

namespace Survivors.Play.Components
{
	public struct Clips : IComponentData
	{
		public BlobAssetReference<SkeletonClipSetBlob> ClipSet;
	}
	public struct FourDirectionClipStates : IComponentData
	{
		public ClipState Center;
		public ClipState Down;
		public ClipState Up;
		public ClipState Left;
		public ClipState Right;
		public ClipState AxeThrow;
	}
	
	
	#region Player Animation Structs
	
	public enum EDirections
	{
		Center,
		Down,
		Up,
		Left,
		Right,
		AxeThrow
	}

	public struct ClipState
	{

		public float Time;
		public float SpeedMultiplier;
	}
	
	#endregion
}