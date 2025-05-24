using UnityEngine;

namespace DG.Tweening
{
	public static class DOTweenAnimationExtensions
	{
		public static bool IsSameOrSubclassOf<T>(this Component t)
		{
			return t is T;
		}
	}
}
