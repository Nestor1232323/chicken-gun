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
					bL_Turret.Fire(_useRoundRobin: false);
				}
			}
		}
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo, 4000f))
		{
			BL_Turret[] array = playerControlledTurrets;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Aim(hitInfo.point);
			}
		}
	}
}
