using UnityEngine;

public class Skin : MonoBehaviour
{
	public enum MeshType
	{
		Ball = 0,
		Cube = 1,
		Disk = 2,
		Mesh = 3
	}

	public Texture texture;

	public bool canWearEyes;

	public bool canWearSmile;

	public bool canWearHat;

	public MeshType meshType;

	public GameObject MeshSkin;
}
