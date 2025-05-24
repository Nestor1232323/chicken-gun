using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace DG.Tweening
{
	public static class DOTweenModuleUtils
	{
		public static class Physics
		{
			public static void SetOrientationOnPath(PathOptions options, Tween t, Quaternion newRot, Transform trans)
			{
				if (options.isRigidbody)
				{
					((Rigidbody)t.target).rotation = newRot;
				}
				else
				{
					trans.rotation = newRot;
				}
			}

			public static bool HasRigidbody2D(Component target)
			{
				return target.GetComponent<Rigidbody2D>() != null;
			}

			public static bool HasRigidbody(Component target)
			{
				return target.GetComponent<Rigidbody>() != null;
			}

			public static TweenerCore<Vector3, Path, PathOptions> CreateDOTweenPathTween(MonoBehaviour target, bool tweenRigidbody, bool isLocal, Path path, float duration, PathMode pathMode)
			{
				Rigidbody rigidbody = ((!tweenRigidbody) ? null : target.GetComponent<Rigidbody>());
				if (tweenRigidbody && rigidbody != null)
				{
					if (isLocal)
					{
						return rigidbody.DOLocalPath(path, duration, pathMode);
					}
					return rigidbody.DOPath(path, duration, pathMode);
				}
				if (isLocal)
				{
					return target.transform.DOLocalPath(path, duration, pathMode);
				}
				return target.transform.DOPath(path, duration, pathMode);
			}
		}

		private static bool _initialized;

		public static void Init()
		{
			if (!_initialized)
			{
				_initialized = true;
				DOTweenExternalCommand.SetOrientationOnPath += Physics.SetOrientationOnPath;
			}
		}
	}
}
