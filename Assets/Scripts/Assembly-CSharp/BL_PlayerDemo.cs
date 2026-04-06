using UnityEngine;

public class BL_PlayerDemo : MonoBehaviour
{
	public Texture2D cursorTexture;

	public CursorMode cursorMode;

	public Vector2 hotSpot = new Vector2(8f, 8f);

	public BL_Turret[] playerControlledTurrets;

	public bool autoControlAllTurrets = true;

	private void Start()
	{
		if (autoControlAllTurrets)
		{
			playerControlledTurrets = Object.FindObjectsOfType<BL_Turret>();
		}
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
	}

	private void Update()
	{
		if (Input.GetButton("Fire1"))
		{
			BL_Turret[] array = playerControlledTurrets;
			foreach (BL_Turret bL_Turret in array)
			{
				if (!Input.GetKey(KeyCode.LeftShift))
				{
					bL_Turret.Fire();
				}
				else
				{
					bL_Turret.Fire(false);
				}
			}
		}
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 4000f))
		{
			BL_Turret[] array2 = playerControlledTurrets;
			foreach (BL_Turret bL_Turret2 in array2)
			{
				bL_Turret2.Aim(hitInfo.point);
			}
		}
	}
}
