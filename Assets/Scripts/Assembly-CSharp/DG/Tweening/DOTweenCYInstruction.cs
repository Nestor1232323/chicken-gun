using UnityEngine;

namespace DG.Tweening
{
	public static class DOTweenCYInstruction
	{
		public class WaitForCompletion : CustomYieldInstruction
		{
			private readonly Tween t;

			public override bool keepWaiting
			{
				get
				{
					return t.active && !t.IsComplete();
				}
			}

			public WaitForCompletion(Tween tween)
			{
				t = tween;
			}
		}

		public class WaitForRewind : CustomYieldInstruction
		{
			private readonly Tween t;

			public override bool keepWaiting
			{
				get
				{
					return t.active && (!t.playedOnce || t.position * (float)(t.CompletedLoops() + 1) > 0f);
				}
			}

			public WaitForRewind(Tween tween)
			{
				t = tween;
			}
		}

		public class WaitForKill : CustomYieldInstruction
		{
			private readonly Tween t;

			public override bool keepWaiting
			{
				get
				{
					return t.active;
				}
			}

			public WaitForKill(Tween tween)
			{
				t = tween;
			}
		}

		public class WaitForElapsedLoops : CustomYieldInstruction
		{
			private readonly Tween t;

			private readonly int elapsedLoops;

			public override bool keepWaiting
			{
				get
				{
					return t.active && t.CompletedLoops() < elapsedLoops;
				}
			}

			public WaitForElapsedLoops(Tween tween, int elapsedLoops)
			{
				t = tween;
				this.elapsedLoops = elapsedLoops;
			}
		}

		public class WaitForPosition : CustomYieldInstruction
		{
			private readonly Tween t;

			private readonly float position;

			public override bool keepWaiting
			{
				get
				{
					return t.active && t.position * (float)(t.CompletedLoops() + 1) < position;
				}
			}

			public WaitForPosition(Tween tween, float position)
			{
				t = tween;
				this.position = position;
			}
		}

		public class WaitForStart : CustomYieldInstruction
		{
			private readonly Tween t;

			public override bool keepWaiting
			{
				get
				{
					return t.active && !t.playedOnce;
				}
			}

			public WaitForStart(Tween tween)
			{
				t = tween;
			}
		}
	}
}
