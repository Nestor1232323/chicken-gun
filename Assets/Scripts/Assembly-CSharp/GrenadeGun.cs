public class GrenadeGun : BaseWeaponScript
{
	protected override void CreateBullet(int bulletPivotIndex = 0)
	{
		base.CreateBullet(bulletPivotIndex);
		(currentBullet as BaseGrenade).Throw(20f, 0f);
		(currentBullet as BaseGrenade).explodeOnContact = true;
	}
}
