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
					if (t.active)
					{
						return !t.IsComplete();
					}
					return false;
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
					if (t.active)
					{
						if (t.playedOnce)
						{
							return t.position * (float)(t.CompletedLoops() + 1) > 0f;
						}
						return true;
					}
					return false;
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

			public override bool keepWaiting => t.active;

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
					if (t.active)
					{
						return t.CompletedLoops() < elapsedLoops;
					}
					return false;
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
					if (t.active)
					{
						return t.position * (float)(t.CompletedLoops() + 1) < position;
					}
					return false;
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
					if (t.active)
					{
						return !t.playedOnce;
					}
					return false;
				}
			}

			public WaitForStart(Tween tween)
			{
				t = tween;
			}
		}
	}
}
