using UnityEngine;

public class BaseBulletScript : PooledObject
{
	public float speed;

	public float damage = 5f;

	public int parentViewID;

	public BulletType type;

	public bool isCollider;

	public bool isOriginal;

	public float rayLength = 1.5f;

	public float lifeTime = 2f;

	private float lifeCounter;

	public AudioSource audioSource;

	public LayerMask targetLayers;

	public PhotonView parentView;

	protected virtual void Update()
	{
		UpdatePosition();
		CheckLifeTime();
	}

	protected virtual void UpdatePosition()
	{
		base.transform.position += base.transform.forward * Time.deltaTime * speed;
	}

	protected void CheckLifeTime()
	{
		lifeCounter += Time.deltaTime;
		if (lifeCounter >= lifeTime)
		{
			base.gameObject.Recycle();
			lifeCounter = 0f;
		}
	}

	protected virtual void OnTriggerEnter(Collider collider)
	{
		bool flag = true;
		if (isOriginal)
		{
		}
		if (flag)
		{
			ShowBulletBoom();
		}
	}

	protected void DamageTarget(IDamageReciver targetDamageReciver)
	{
		targetDamageReciver.Damage(damage, parentViewID);
	}

	public virtual void ShowBulletBoom()
	{
		OnRecycle();
	}

	public override void OnSpawn()
	{
		base.OnSpawn();
		lifeCounter = 0f;
		if (!isCollider)
		{
			RaycastForTarget();
		}
	}

	protected virtual void RaycastForTarget()
	{
	}

	public virtual void SetDamage(float val)
	{
		damage = val;
	}
}
