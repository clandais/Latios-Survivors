using Latios.Authoring;
using Latios.Authoring.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.LowLevel.Unsafe;

namespace Survivors.Play.Authoring.Mecanim
{
	[UpdateInGroup(typeof(SmartBlobberBakingGroup))]
	[BurstCompile]
	public partial struct BlendParametersSmartBlobberSystem : ISystem
	{
		public void OnCreate(ref SystemState state)
		{
			new SmartBlobberTools<BlendParametersSetBlob>().Register(state.World);
		}

		public void OnUpdate(ref SystemState state)
		{
			new Job().ScheduleParallel();
		}

		[WithOptions(EntityQueryOptions.IncludeDisabledEntities | EntityQueryOptions.IncludePrefab)]
		private partial struct Job : IJobEntity
		{
			public void Execute(ref SmartBlobberResult result, in DynamicBuffer<BlendParameterElementInput> bufferInput)
			{
				var                        builder = new BlobBuilder(Allocator.Temp);
				ref BlendParametersSetBlob root    = ref builder.ConstructRoot<BlendParametersSetBlob>();

				var typedBlob = builder.Allocate(ref root.Parameters, bufferInput.Length);
				for (int index = 0; index < bufferInput.Length; index++)
				{
					typedBlob[index] = new BlendParameter
					{
						Value = bufferInput[index].Value
					};
				}

				result.blob = UnsafeUntypedBlobAssetReference.Create(builder.CreateBlobAssetReference<BlendParameter>(Allocator.Persistent));
			}
		}
	}
}