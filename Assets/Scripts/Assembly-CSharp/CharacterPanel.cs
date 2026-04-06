using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
	public GameObject character;

	public Transform weaponsPanel;

	public Transform actionsPanel;

	public Transform camerasPanel;

	public Button buttonPrefab;

	public Slider motionSpeed;

	private Actions actions;

	private PlayerController controller;

	private Camera[] cameras;

	private void Start()
	{
		Initialize();
	}

	private void Initialize()
	{
		actions = character.GetComponent<Actions>();
		controller = character.GetComponent<PlayerController>();
		PlayerController.Arsenal[] arsenal = controller.arsenal;
		for (int i = 0; i < arsenal.Length; i++)
		{
			PlayerController.Arsenal arsenal2 = arsenal[i];
			CreateWeaponButton(arsenal2.name);
		}
		CreateActionButton("Stay");
		CreateActionButton("Walk");
		CreateActionButton("Run");
		CreateActionButton("Sitting");
		CreateActionButton("Jump");
		CreateActionButton("Aiming");
		CreateActionButton("Attack");
		CreateActionButton("Damage");
		CreateActionButton("Death Reset", "Death");
		cameras = Object.FindObjectsOfType<Camera>();
		IOrderedEnumerable<Camera> orderedEnumerable = cameras.OrderBy((Camera s) => s.name);
		foreach (Camera item in orderedEnumerable)
		{
			CreateCameraButton(item);
		}
		camerasPanel.GetChild(0).GetComponent<Button>().onClick.Invoke();
	}

	private void CreateWeaponButton(string name)
	{
		Button button = CreateButton(name, weaponsPanel);
		button.onClick.AddListener(delegate
		{
			controller.SetArsenal(name);
		});
	}

	private void CreateActionButton(string name)
	{
		CreateActionButton(name, name);
	}

	private void CreateActionButton(string name, string message)
	{
		Button button = CreateButton(name, actionsPanel);
		button.onClick.AddListener(delegate
		{
			actions.SendMessage(message, SendMessageOptions.DontRequireReceiver);
		});
	}

	private void CreateCameraButton(Camera c)
	{
		Button button = CreateButton(c.name, camerasPanel);
		button.onClick.AddListener(delegate
		{
			ShowCamera(c);
		});
	}

	private Button CreateButton(string name, Transform group)
	{
		GameObject gameObject = Object.Instantiate(buttonPrefab.gameObject);
		gameObject.name = name;
		gameObject.transform.SetParent(group);
		gameObject.transform.localScale = Vector3.one;
		Text component = gameObject.transform.GetChild(0).GetComponent<Text>();
		component.text = name;
		return gameObject.GetComponent<Button>();
	}

	private void ShowCamera(Camera cam)
	{
		Camera[] array = cameras;
		foreach (Camera camera in array)
		{
			camera.gameObject.SetActive(camera == cam);
		}
	}

	private void Update()
	{
		Time.timeScale = motionSpeed.value;
	}

	public void OpenPublisherPage()
	{
		Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/publisher/11008");
	}
}
