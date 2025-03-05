using Latios.Kinemation;
using Unity.Collections;
using Unity.Entities;

namespace Survivors.Play.Components
{
	public struct Clips : IComponentData
	{
		public BlobAssetReference<SkeletonClipSetBlob> ClipSet;
	}
	
	
	public struct DeathClips : IComponentData
	{
		public BlobAssetReference<SkeletonClipSetBlob> ClipSet;
	}
	
	public struct DeathClipsStates : IComponentData
	{
		public ClipState StateA;
		public ClipState StateB;
		public ClipState StateC;
		public int ChosenState;
	}
	
	public struct FourDirectionClipStates : IComponentData
	{
		public ClipState Center;
		public ClipState Down;
		public ClipState Up;
		public ClipState Left;
		public ClipState Right;
		public ClipState Attack;
	}
	
	
	#region Player Animation Structs
	
	public enum EDirections
	{
		Center,
		Down,
		Up,
		Left,
		Right,
		Attack
	}

	public struct ClipState
	{

		public float PreviousTime;
		public float Time;
		public float SpeedMultiplier;
		public int  EventHash;

		public void Update(float deltaTime)
		{
			PreviousTime =  Time;
			Time         += deltaTime;
		}
	}
	
	#endregion
}