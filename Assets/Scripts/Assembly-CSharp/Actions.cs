using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Actions : MonoBehaviour
{
	private Animator animator;

	private const int countOfDamageAnimations = 3;

	private int lastDamageAnimation = -1;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	public void Stay()
	{
		animator.SetBool("Aiming", value: false);
		animator.SetFloat("Speed", 0f);
	}

	public void Walk()
	{
		animator.SetBool("Aiming", value: false);
		animator.SetFloat("Speed", 0.5f);
	}

	public void Run()
	{
		animator.SetBool("Aiming", value: false);
		animator.SetFloat("Speed", 1f);
	}

	public void Attack()
	{
		Aiming();
		animator.SetTrigger("Attack");
	}

	public void Death()
	{
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
		{
			animator.Play("Idle", 0);
		}
		else
		{
			animator.SetTrigger("Death");
		}
	}

	public void Damage()
	{
		if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
		{
			int num;
			for (num = Random.Range(0, 3); num == lastDamageAnimation; num = Random.Range(0, 3))
			{
			}
			lastDamageAnimation = num;
			animator.SetInteger("DamageID", num);
			animator.SetTrigger("Damage");
		}
	}

	public void Jump()
	{
		animator.SetBool("Squat", value: false);
		animator.SetFloat("Speed", 0f);
		animator.SetBool("Aiming", value: false);
		animator.SetTrigger("Jump");
	}

	public void Aiming()
	{
		animator.SetBool("Squat", value: false);
		animator.SetFloat("Speed", 0f);
		animator.SetBool("Aiming", value: true);
	}

	public void Sitting()
	{
		animator.SetBool("Squat", !animator.GetBool("Squat"));
		animator.SetBool("Aiming", value: false);
	}
}
