using UnityEngine;

public class HTLiquidSpriteSheet : MonoBehaviour
{
	private Texture2D[] _diffuseTexture;

	public int uvAnimationTileX;

	public int uvAnimationTileY;

	public int spriteCount;

	public int framesPerSecond;

	public Vector2 textureSize;

	public Vector2 scrollSpeed;

	private float _startTime;

	private Vector2 currentOffset;

	private void Start()
	{
		_diffuseTexture = new Texture2D[spriteCount];
		InitSpriteTexture();
		_startTime = Time.time;
	}

	private void Update()
	{
		GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(1f, 1f));
		float num = (Time.time - _startTime) * (float)framesPerSecond;
		num %= (float)(uvAnimationTileX * uvAnimationTileY);
		if (num == (float)spriteCount)
		{
			_startTime = Time.time;
			num = 0f;
		}
		GetComponent<Renderer>().material.SetTextureScale("_MainTex", textureSize);
		currentOffset += scrollSpeed * Time.deltaTime;
		GetComponent<Renderer>().material.SetTextureOffset("_MainTex", currentOffset);
		GetComponent<Renderer>().material.SetTexture("_MainTex", _diffuseTexture[(int)num]);
	}

	public void InitSpriteTexture()
	{
		Texture2D texture2D = (Texture2D)GetComponent<Renderer>().material.GetTexture("_MainTex");
		int num = GetComponent<Renderer>().material.mainTexture.width / uvAnimationTileX;
		int num2 = GetComponent<Renderer>().material.mainTexture.height / uvAnimationTileY;
		int num3 = 0;
		int num4 = 0;
		int num5 = uvAnimationTileY - 1;
		while (num5 >= 0 && num3 < spriteCount)
		{
			while (num4 < uvAnimationTileX && num3 < spriteCount)
			{
				Color[] pixels = texture2D.GetPixels(num * num4, num2 * num5, num, num2);
				_diffuseTexture[num3] = new Texture2D(num, num2);
				_diffuseTexture[num3].SetPixels(pixels);
				_diffuseTexture[num3].Apply();
				num4++;
				num3++;
			}
			num4 = 0;
			num5--;
		}
	}
}
