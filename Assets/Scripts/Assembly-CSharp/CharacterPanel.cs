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
		foreach (Camera item in cameras.OrderBy((Camera s) => s.name))
		{
			CreateCameraButton(item);
		}
		camerasPanel.GetChild(0).GetComponent<Button>().onClick.Invoke();
	}

	private void CreateWeaponButton(string name)
	{
		CreateButton(name, weaponsPanel).onClick.AddListener(delegate
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
		CreateButton(name, actionsPanel).onClick.AddListener(delegate
		{
			actions.SendMessage(message, SendMessageOptions.DontRequireReceiver);
		});
	}

	private void CreateCameraButton(Camera c)
	{
		CreateButton(c.name, camerasPanel).onClick.AddListener(delegate
		{
			ShowCamera(c);
		});
	}

	private Button CreateButton(string name, Transform group)
	{
		GameObject obj = Object.Instantiate(buttonPrefab.gameObject);
		obj.name = name;
		obj.transform.SetParent(group);
		obj.transform.localScale = Vector3.one;
		obj.transform.GetChild(0).GetComponent<Text>().text = name;
		return obj.GetComponent<Button>();
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
