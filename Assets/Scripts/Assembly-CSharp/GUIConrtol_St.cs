using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIConrtol_St : MonoBehaviour
{
	public GameObject[] chrModel;

	private SantaAnimatorControl chrCtrl;

	private int activeLodIdx;

	public TextAsset stateList;

	public TextAsset IMInfo;

	private string[] stateName;

	private int[] weaponPos;

	private string[] stateLabelName;

	public GameObject[] lightObj;

	public GameObject AnimationSelectUI;

	public GameObject InteractiveModeUI;

	public GameObject ModelInformationUI;

	public Slider ModelChangeSlider;

	private GameObject[] ASelectBtn = new GameObject[10];

	private Text[] ASelectLabel = new Text[10];

	private Text ASelectPage;

	private Text IMText;

	private bool viewerMode = true;

	private string meshInfoMsg;

	private string[] iModeMsg = new string[3];

	private void Start()
	{
		chrCtrl = chrModel[activeLodIdx].GetComponent<SantaAnimatorControl>();
		ModelInformationUI.GetComponentInChildren<Text>().text = "Lady Santa Cloth_00 (High)\n\n" + chrCtrl.MeshData();
		TextReaderState();
		GameObject gameObject = GameObject.Find("Window_AnimationSelect/gridLayout");
		for (int i = 0; i < ASelectBtn.Length; i++)
		{
			ASelectBtn[i] = gameObject.transform.GetChild(i).gameObject;
			ASelectLabel[i] = ASelectBtn[i].GetComponentInChildren<Text>();
		}
		ASelectPage = GameObject.Find("Window_AnimationSelect/pageNumber").GetComponentInChildren<Text>();
		TextReaderIMinfo();
		MotionControlBtn(1);
		ChangeAnimator(0);
	}

	public void ToggleUIVisibility(GameObject uiGrp)
	{
		uiGrp.SetActive(!uiGrp.activeSelf);
	}

	public void ModeChangeBtn(Text modeChangelabel)
	{
		if (viewerMode)
		{
			viewerMode = false;
			modeChangelabel.text = "Change to\nViewer Mode";
			AnimationSelectUI.SetActive(value: false);
			InteractiveModeUI.SetActive(value: true);
			if (IMText == null)
			{
				IMText = GameObject.Find("Window_InteractiveMode/InformationText").GetComponent<Text>();
				IMText.text = iModeMsg[0];
			}
			ChangeAnimator(1);
		}
		else if (!viewerMode)
		{
			viewerMode = true;
			modeChangelabel.text = "Change to\nInteractive Mode";
			AnimationSelectUI.SetActive(value: true);
			InteractiveModeUI.SetActive(value: false);
			ChangeAnimator(0);
		}
	}

	public void MotionControlBtn(int currentPage)
	{
		int num = 10;
		int num2 = Mathf.CeilToInt((float)stateName.Length / (float)num);
		if (currentPage == 0)
		{
			currentPage = num2;
		}
		else if (currentPage > num2)
		{
			currentPage = 1;
		}
		ASelectPage.name = currentPage.ToString();
		ASelectPage.text = currentPage.ToString("D3") + " / " + num2.ToString("D3");
		for (int i = 0; i < num; i++)
		{
			int num3 = (currentPage - 1) * num + i;
			if (num3 >= stateName.Length)
			{
				ASelectBtn[i].SetActive(value: false);
				continue;
			}
			ASelectBtn[i].SetActive(value: true);
			ASelectBtn[i].name = num3.ToString();
			ASelectLabel[i].text = stateLabelName[num3];
		}
	}

	public void PlayClipBtn(GameObject self)
	{
		int num = int.Parse(self.name);
		chrCtrl.PlayClip(stateName[num], weaponPos[num]);
	}

	public void TurnPage(bool isNextPage)
	{
		int num = int.Parse(ASelectPage.name);
		num = ((!isNextPage) ? (num - 1) : (num + 1));
		MotionControlBtn(num);
	}

	public void CameraZoomControlBtn()
	{
		base.gameObject.GetComponent<CamControl>().CamZoom();
	}

	public void CameraRotationControlBtn(Toggle tgle)
	{
		base.gameObject.GetComponent<CamControl>().RotateOption(tgle.isOn);
	}

	public void BackLightControlBtn(Toggle tgle)
	{
		GameObject[] array = lightObj;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(tgle.isOn);
		}
	}

	public void ChangeTextureBtn()
	{
		chrCtrl.ChangeTexture(isResetTex: false);
	}

	public void LODControlBtn(Text label)
	{
		int num;
		if (ModelChangeSlider.value == (float)activeLodIdx)
		{
			num = activeLodIdx + 1;
			if (num == chrModel.Length)
			{
				num = 0;
			}
			ModelChangeSlider.value = num;
		}
		else
		{
			num = (int)ModelChangeSlider.value;
		}
		string text = SetLabelName(num);
		label.text = "Model Change\n" + text;
		StartCoroutine(ChangeLOD("Lady Santa " + text, num));
	}

	public void LODControlSlider(Text label)
	{
		int labelName = (int)ModelChangeSlider.value;
		string text = SetLabelName(labelName);
		label.text = "Model Change\n" + text;
	}

	private string SetLabelName(int idx)
	{
		string[] array = new string[3] { " (High)", " (Low)", " (SD)" };
		return "Cloth_" + (idx / 3).ToString("D2") + array[idx % 3];
	}

	private IEnumerator ChangeLOD(string modelName, int selectedIdx)
	{
		int aIdx = activeLodIdx;
		activeLodIdx = selectedIdx;
		chrCtrl.PlayClip(stateName[0], weaponPos[0]);
		yield return new WaitForSeconds(0.1f);
		chrCtrl.ChangeTexture(isResetTex: true);
		for (int i = 0; i < chrModel.Length; i++)
		{
			if (i != activeLodIdx)
			{
				chrModel[i].SetActive(value: false);
			}
		}
		chrModel[activeLodIdx].SetActive(value: true);
		chrCtrl = chrModel[activeLodIdx].GetComponent<SantaAnimatorControl>();
		chrModel[activeLodIdx].transform.position = chrModel[aIdx].transform.position;
		chrModel[activeLodIdx].transform.rotation = chrModel[aIdx].transform.rotation;
		chrCtrl.PlayClip(stateName[0], weaponPos[0]);
		ModelInformationUI.GetComponentInChildren<Text>().text = modelName + "\n\n" + chrCtrl.MeshData();
		base.gameObject.GetComponent<CamControl>().target = chrModel[activeLodIdx].transform;
	}

	private void ChangeAnimator(int idx)
	{
		for (int i = 0; i < chrModel.Length; i++)
		{
			chrModel[i].GetComponent<SantaAnimatorControl>().ControllerChange(idx);
		}
	}

	private int GetSystemLanguage()
	{
		if (Application.systemLanguage == SystemLanguage.Japanese)
		{
			return 1;
		}
		if (Application.systemLanguage == SystemLanguage.Korean)
		{
			return 2;
		}
		return 0;
	}

	private void TextReaderState()
	{
		string[] array = stateList.text.Split('\n');
		stateName = new string[array.Length];
		weaponPos = new int[array.Length];
		stateLabelName = new string[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(',');
			stateName[i] = array2[0];
			weaponPos[i] = int.Parse(array2[1]);
			stateLabelName[i] = array2[2 + GetSystemLanguage()];
		}
	}

	private void TextReaderIMinfo()
	{
		string[] array = IMInfo.text.Split('/');
		iModeMsg[0] = array[GetSystemLanguage()];
	}

	public void LoadScene(int idx)
	{
		string sceneName = "DemoScene";
		if (idx == 1)
		{
			sceneName = "Module_DemoScene";
		}
		SceneManager.LoadScene(sceneName);
	}
}
