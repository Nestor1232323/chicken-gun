using UnityEngine;

public class BodyPartDmgReceiver : DamageReciver2
{
	[SerializeField]
	protected float damageMultiplier = 1f;

	[SerializeField]
	public CharacterMotor myPlayer;

	[SerializeField]
	private BodyPart bodyPart;

	public override void Damage(float dmg, int fromWhom = 0)
	{
		myPlayer.ApplyDamage(dmg * damageMultiplier, bodyPart, fromWhom);
	}
}
