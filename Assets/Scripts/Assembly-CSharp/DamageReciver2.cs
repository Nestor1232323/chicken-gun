using UnityEngine;

public class DamageReciver2 : MonoBehaviour, IDamageReciver
{
	public IDestroyable myTarget;

	protected virtual void Start()
	{
		myTarget = GetComponent<IDestroyable>();
	}

	public virtual void Damage(float dmg, int fromWhom = 0)
	{
		if (myTarget != null)
		{
			myTarget.ApplyDamage(dmg, fromWhom);
		}
	}

	public virtual void Heal(float val)
	{
		if (myTarget != null)
		{
			myTarget.ApplyHeal(val);
		}
	}
}
