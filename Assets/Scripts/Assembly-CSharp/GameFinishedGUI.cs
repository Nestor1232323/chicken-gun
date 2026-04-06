using UnityEngine;
using UnityEngine.UI;

public class GameFinishedGUI : MonoBehaviour
{
	private enum GUIState
	{
		undef = 0,
		otherCarsDemonstr = 1,
		countDown = 2,
		playing = 3,
		won = 4,
		lost = 5,
		interrupted = 6
	}

	public RawImage steeringWheel;

	public RawImage steeringRect;

	private int W = Screen.width;

	private int H = Screen.height;

	private Vector2 steeringWheelSize;

	private Vector3 steeringWheelPos;

	private Vector3 SteeringWheelRot;

	private Vector2 steeringWheelPointerPos;

	private bool steeringWheelActive;

	private float steeringAngle;

	private Vector3 steerRectPos;

	private Vector2 steerRectSize;

	public GameController gameController;

	private void Start()
	{
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
		initVars();
		steeringWheel.transform.position = steeringWheelPos;
		steeringWheel.transform.rotation = Quaternion.Euler(SteeringWheelRot);
		steeringWheel.rectTransform.sizeDelta = steeringWheelSize;
		steeringWheelPointerPos = Vector2.zero;
		steeringWheelActive = false;
		steeringRect.rectTransform.position = steerRectPos;
		steeringRect.rectTransform.sizeDelta = steerRectSize;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && Input.mousePosition.x > steerRectPos.x - steerRectSize.x / 2f && Input.mousePosition.x < steerRectPos.x + steerRectSize.x / 2f && Input.mousePosition.y > steerRectPos.y - steerRectSize.y / 2f && Input.mousePosition.y < steerRectPos.y + steerRectSize.y / 2f)
		{
			steeringWheelPointerPos = Input.mousePosition;
			steeringWheelActive = true;
		}
		if (steeringWheelActive)
		{
			steeringWheelPointerPos = Input.mousePosition;
			steeringAngle = (steeringWheelPointerPos.x - steerRectPos.x) / (steerRectSize.x * 0.5f);
			if (Mathf.Abs(steeringAngle) > 1f)
			{
				steeringAngle = Mathf.Abs(steeringAngle) * Mathf.Sign(steeringAngle);
			}
			SteeringWheelRot.z = steeringAngle;
			steeringWheel.transform.rotation = Quaternion.Euler(SteeringWheelRot);
		}
		if (Input.GetMouseButtonUp(0))
		{
			steeringWheelActive = false;
			SteeringWheelRot.z = 0f;
			steeringAngle = 0f;
		}
	}

	private void initVars()
	{
		steeringWheelSize = new Vector2((float)W * 0.2f, (float)W * 0.2f);
		steeringWheelPos = new Vector3((float)W * 0.052f + steeringWheelSize.x / 2f, steeringWheelSize.y / 2f, 0f);
		SteeringWheelRot = new Vector3(0f, 0f, 0f);
		steerRectSize = new Vector3((float)W * 0.3f, (float)H * 0.15f);
		steerRectPos = new Vector3((float)W * 0.2f, (float)H * 0.2f);
	}
}
