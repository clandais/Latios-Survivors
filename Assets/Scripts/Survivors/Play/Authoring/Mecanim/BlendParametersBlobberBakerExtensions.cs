using Latios.Authoring;
using Unity.Entities;

namespace Survivors.Play.Authoring.Mecanim
{
	public static class BlendParametersBlobberBakerExtensions
	{
		public static SmartBlobberHandle<BlendParametersSetBlob> RequestCreateBlobAsset(this IBaker baker, float[] values)
		{
			return baker.RequestCreateBlobAsset<BlendParametersSetBlob, ParametersSmartBlobberRequestFilter>(new ParametersSmartBlobberRequestFilter
			{
				Value = values
			});
		}
	}
}