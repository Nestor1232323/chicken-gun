using System.Collections;
using UnityEngine;

public class InventoryInputController : MonoBehaviour
{
	public GameObject selectItemBtn;

	public GameObject deselectItemBtn;

	public GameObject removeItemBtn;

	public RectTransform plusMinusButtonsPanel;

	public GameObject rotationPanel;

	public LongTapButton throwBtn;

	public PlayerInventaryManager inventoryManager;

	private Vector2 rotationMouseDelta;

	private IEnumerator distCoroutine;

	private int fingerID = -1;

	private Vector3 prevTouchPos = Vector3.zero;

	private Vector3 curTouchPos = Vector3.zero;

	private IEnumerator Start()
	{
		throwBtn.OnLongTapUpIncomplete.AddListener(OnThrowBtn);
		ShowDeselectBtn(show: false);
		while (GameController.instance.OurPlayer == null)
		{
			yield return null;
		}
	}

	private void Update()
	{
		UpdateRotationMouseDeltaMove();
	}

	public void SelectBtnClick()
	{
		if (inventoryManager.SelectItemInFront())
		{
			selectItemBtn.SetActive(value: false);
			deselectItemBtn.SetActive(value: true);
		}
	}

	public void DeselectBtnClick()
	{
		inventoryManager.DeselectCurrentItem();
		selectItemBtn.SetActive(value: true);
		deselectItemBtn.SetActive(value: false);
	}

	public void PlusBtnDown()
	{
		distCoroutine = DistanceCrt(1);
		StartCoroutine(distCoroutine);
	}

	public void PlusBtnUp()
	{
		MonoBehaviour.print("PlusBtnUp");
		StopCoroutine(distCoroutine);
	}

	public void MinusBtnDown()
	{
		distCoroutine = DistanceCrt(-1);
		StartCoroutine(distCoroutine);
	}

	public void MinusBtnUp()
	{
		StopCoroutine(distCoroutine);
	}

	private IEnumerator DistanceCrt(int sign)
	{
		while (true)
		{
			yield return null;
			inventoryManager.UpdateDistance(sign);
		}
	}

	public void RemoveSelectedItemBtnClick()
	{
		inventoryManager.RemoveSelectedItem();
	}

	public void ShowSelectBtn(bool show)
	{
		selectItemBtn.SetActive(show);
	}

	public void ShowDeselectBtn(bool show)
	{
		deselectItemBtn.SetActive(show);
	}

	public void ShowDistanceButtons(bool show)
	{
	}

	public void ShowRemoveItemButton(bool show)
	{
		removeItemBtn.gameObject.SetActive(show);
		throwBtn.gameObject.SetActive(show);
	}

	public void ShowRotationPanel(bool show)
	{
		rotationPanel.gameObject.SetActive(show);
	}

	public void OnItemSelected(bool selected)
	{
		selectItemBtn.SetActive(!selected);
		deselectItemBtn.SetActive(selected);
		rotationPanel.SetActive(selected);
		removeItemBtn.gameObject.SetActive(selected);
	}

	public void RotationAreaDown()
	{
		MonoBehaviour.print("GeneralListenerAreaTouchDown");
		if (GameController.isMobile)
		{
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				Touch touch = touches[i];
				if (touch.phase == TouchPhase.Began)
				{
					fingerID = touch.fingerId;
					curTouchPos = touch.position;
					prevTouchPos = curTouchPos;
				}
			}
		}
		else
		{
			fingerID = 1;
			curTouchPos = Input.mousePosition;
			prevTouchPos = curTouchPos;
		}
	}

	public void RotationAreaUp()
	{
		if (GameController.isMobile)
		{
			Touch[] touches = Input.touches;
			foreach (Touch touch in touches)
			{
				if (touch.fingerId == fingerID)
				{
					fingerID = -1;
				}
			}
		}
		else
		{
			fingerID = -1;
		}
		prevTouchPos = curTouchPos;
	}

	public void UpdateRotationMouseDeltaMove()
	{
		if (!rotationPanel.activeSelf)
		{
			return;
		}
		if (GameController.isMobile)
		{
			if (fingerID != -1)
			{
				Touch[] touches = Input.touches;
				for (int i = 0; i < touches.Length; i++)
				{
					Touch touch = touches[i];
					if (touch.fingerId == fingerID)
					{
						prevTouchPos = curTouchPos;
						curTouchPos = touch.position;
					}
				}
			}
			else
			{
				prevTouchPos = curTouchPos;
			}
		}
		else if (fingerID == 1)
		{
			prevTouchPos = curTouchPos;
			curTouchPos = Input.mousePosition;
		}
		else
		{
			rotationMouseDelta = Vector2.zero;
		}
		rotationMouseDelta = curTouchPos - prevTouchPos;
		inventoryManager.SetRotationDelta(rotationMouseDelta);
	}

	public void OnThrowBtn(float strengthK)
	{
		MonoBehaviour.print("throw " + strengthK);
		inventoryManager.ThrowItem(strengthK);
	}
}
