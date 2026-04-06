using UnityEngine;

public class RootMotionOff : StateMachineBehaviour
{
	public float ColliderTestTime;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.applyRootMotion = false;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.applyRootMotion = true;
	}
}
