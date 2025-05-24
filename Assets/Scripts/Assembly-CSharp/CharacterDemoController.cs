using UnityEngine;

public class CharacterDemoController : MonoBehaviour
{
	private Animator animator;

	public GameObject floorPlane;

	public int WeaponState;

	public bool wasAttacking;

	private float rotateSpeed = 20f;

	public Vector3 movementTargetPosition;

	public Vector3 attackPos;

	public Vector3 lookAtPos;

	private RaycastHit hit;

	private Ray ray;

	public bool rightButtonDown;

	private void Start()
	{
		animator = GetComponentInChildren<Animator>();
		movementTargetPosition = base.transform.position;
	}

	private void Update()
	{
		Input.GetKey(KeyCode.LeftAlt);
		switch (Input.inputString)
		{
		case "0":
			WeaponState = 0;
			break;
		case "1":
			WeaponState = 1;
			break;
		case "2":
			WeaponState = 2;
			break;
		case "3":
			WeaponState = 3;
			break;
		case "4":
			WeaponState = 4;
			break;
		case "5":
			WeaponState = 5;
			break;
		case "6":
			WeaponState = 6;
			break;
		case "7":
			WeaponState = 7;
			break;
		case "8":
			WeaponState = 8;
			break;
		case "p":
			animator.SetTrigger("Pain");
			break;
		case "a":
			animator.SetInteger("Death", 1);
			break;
		case "b":
			animator.SetInteger("Death", 2);
			break;
		case "c":
			animator.SetInteger("Death", 3);
			break;
		case "n":
			animator.SetBool("NonCombat", value: true);
			break;
		}
		animator.SetInteger("WeaponState", WeaponState);
		if (!Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(1) && !rightButtonDown)
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (floorPlane.GetComponent<Collider>().Raycast(ray, out hit, 500f))
			{
				movementTargetPosition = base.transform.position;
				attackPos = hit.point;
				attackPos.y = base.transform.position.y;
				Vector3 vector = attackPos - base.transform.position;
				attackPos = base.transform.position + vector.normalized * 20f;
				animator.SetTrigger("Use");
				animator.SetBool("Idling", value: true);
				rightButtonDown = true;
				wasAttacking = true;
			}
		}
		if (Input.GetMouseButtonUp(1) && rightButtonDown)
		{
			rightButtonDown = false;
		}
		Debug.DrawLine(movementTargetPosition + base.transform.up * 2f, movementTargetPosition);
		Vector3 vector2 = movementTargetPosition - base.transform.position;
		if (!wasAttacking)
		{
			lookAtPos = base.transform.position + vector2.normalized * 2f;
			lookAtPos.y = base.transform.position.y;
		}
		else
		{
			lookAtPos = attackPos;
		}
		Quaternion rotation = base.transform.rotation;
		base.transform.LookAt(lookAtPos);
		Quaternion rotation2 = base.transform.rotation;
		base.transform.rotation = Quaternion.Slerp(rotation, rotation2, Time.deltaTime * rotateSpeed);
		if (Vector3.Distance(movementTargetPosition, base.transform.position) > 0.5f)
		{
			animator.SetBool("Idling", value: false);
		}
		else
		{
			animator.SetBool("Idling", value: true);
		}
	}

	private void OnGUI()
	{
		string text = "LMB=move RMB=attack p=pain abc=deaths 12345678 0=change weapons";
		GUI.Label(new Rect(10f, 5f, 1000f, 20f), text);
	}
}
