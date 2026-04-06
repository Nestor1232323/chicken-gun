using UnityEngine;

public class InventoryConnectionLine : MonoBehaviour
{
	[SerializeField]
	private LineRenderer lineRenderer;

	[SerializeField]
	public Transform startPos;

	[SerializeField]
	public Vector3 endPos;

	private CharacterMotor myPlayer;

	private void Start()
	{
		myPlayer = GetComponentInParent<CharacterMotor>();
		startPos = myPlayer.playerWeaponManager.FirstBulletPivot();
	}

	private void LateUpdate()
	{
		lineRenderer.SetPosition(0, myPlayer.playerWeaponManager.FirstBulletPivot().position);
		lineRenderer.SetPosition(1, endPos);
	}

	public void SetEndPos(Transform endPos)
	{
		this.endPos = endPos.position;
	}

	public void SetEndPos(Vector3 endPos)
	{
		this.endPos = endPos;
	}

	public void Show(bool show)
	{
		base.gameObject.SetActive(show);
	}
}
