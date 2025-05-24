using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Wheel))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Effects/Tire Mark Creator", 0)]
public class TireMarkCreate : MonoBehaviour
{
	private Transform tr;

	private Wheel w;

	private Mesh mesh;

	private int[] tris;

	private Vector3[] verts;

	private Vector2[] uvs;

	private Color[] colors;

	private Vector3 leftPoint;

	private Vector3 rightPoint;

	private Vector3 leftPointPrev;

	private Vector3 rightPointPrev;

	private bool creatingMark;

	private bool continueMark;

	private GameObject curMark;

	private Transform curMarkTr;

	private int curEdge;

	private float gapDelay;

	private int curSurface = -1;

	private int prevSurface = -1;

	private bool popped;

	private bool poppedPrev;

	[Tooltip("How much the tire must slip before marks are created")]
	public float slipThreshold;

	private float alwaysScrape;

	[Tooltip("Materials in array correspond to indices in surface types in GroundSurfaceMaster")]
	public Material[] tireMarkMaterials;

	[Tooltip("Materials in array correspond to indices in surface types in GroundSurfaceMaster")]
	public Material[] rimMarkMaterials;

	[Tooltip("Particles in array correspond to indices in surface types in GroundSurfaceMaster")]
	public ParticleSystem[] debrisParticles;

	public ParticleSystem sparks;

	private float[] initialEmissionRates;

	private ParticleSystem.MinMaxCurve zeroEmission = new ParticleSystem.MinMaxCurve(0f);

	private void Start()
	{
		tr = base.transform;
		w = GetComponent<Wheel>();
		initialEmissionRates = new float[debrisParticles.Length + 1];
		for (int i = 0; i < debrisParticles.Length; i++)
		{
			initialEmissionRates[i] = debrisParticles[i].emission.rateOverTime.constantMax;
		}
		if ((bool)sparks)
		{
			initialEmissionRates[debrisParticles.Length] = sparks.emission.rateOverTime.constantMax;
		}
	}

	private void Update()
	{
		if (w.grounded)
		{
			alwaysScrape = ((!GroundSurfaceMaster.surfaceTypesStatic[w.contactPoint.surfaceType].alwaysScrape) ? 0f : (slipThreshold + Mathf.Min(0.5f, Mathf.Abs(w.rawRPM * 0.001f))));
		}
		else
		{
			alwaysScrape = 0f;
		}
		if (w.grounded && (Mathf.Abs(F.MaxAbs(w.sidewaysSlip, w.forwardSlip)) > slipThreshold || alwaysScrape > 0f) && w.connected)
		{
			prevSurface = curSurface;
			curSurface = ((!w.grounded) ? (-1) : w.contactPoint.surfaceType);
			poppedPrev = popped;
			popped = w.popped;
			if (!creatingMark)
			{
				prevSurface = curSurface;
				StartMark();
			}
			else if (curSurface != prevSurface || popped != poppedPrev)
			{
				EndMark();
			}
			if ((bool)curMark)
			{
				Vector3 vector = Quaternion.AngleAxis(90f, w.contactPoint.normal) * tr.right * ((!w.popped) ? w.tireWidth : w.rimWidth);
				leftPoint = curMarkTr.InverseTransformPoint(w.contactPoint.point + vector * w.suspensionParent.flippedSideFactor * Mathf.Sign(w.rawRPM) + w.contactPoint.normal * GlobalControl.tireMarkHeightStatic);
				rightPoint = curMarkTr.InverseTransformPoint(w.contactPoint.point - vector * w.suspensionParent.flippedSideFactor * Mathf.Sign(w.rawRPM) + w.contactPoint.normal * GlobalControl.tireMarkHeightStatic);
			}
		}
		else if (creatingMark)
		{
			EndMark();
		}
		if (curEdge < GlobalControl.tireMarkLengthStatic && creatingMark)
		{
			UpdateMark();
		}
		else if (creatingMark)
		{
			EndMark();
		}
		for (int i = 0; i < debrisParticles.Length; i++)
		{
			if (w.connected)
			{
				if (i == w.contactPoint.surfaceType)
				{
					if (GroundSurfaceMaster.surfaceTypesStatic[w.contactPoint.surfaceType].leaveSparks && w.popped)
					{
						ParticleSystem.EmissionModule emission = debrisParticles[i].emission;
						emission.rateOverTime = zeroEmission;
						if ((bool)sparks)
						{
							emission = sparks.emission;
							emission.rateOverTime = new ParticleSystem.MinMaxCurve(initialEmissionRates[debrisParticles.Length] * Mathf.Clamp01(Mathf.Abs(F.MaxAbs(w.sidewaysSlip, w.forwardSlip, alwaysScrape)) - slipThreshold));
						}
					}
					else
					{
						ParticleSystem.EmissionModule emission2 = debrisParticles[i].emission;
						emission2.rateOverTime = new ParticleSystem.MinMaxCurve(initialEmissionRates[i] * Mathf.Clamp01(Mathf.Abs(F.MaxAbs(w.sidewaysSlip, w.forwardSlip, alwaysScrape)) - slipThreshold));
						if ((bool)sparks)
						{
							emission2 = sparks.emission;
							emission2.rateOverTime = zeroEmission;
						}
					}
				}
				else
				{
					ParticleSystem.EmissionModule emission3 = debrisParticles[i].emission;
					emission3.rateOverTime = zeroEmission;
				}
			}
			else
			{
				ParticleSystem.EmissionModule emission4 = debrisParticles[i].emission;
				emission4.rateOverTime = zeroEmission;
				if ((bool)sparks)
				{
					emission4 = sparks.emission;
					emission4.rateOverTime = zeroEmission;
				}
			}
		}
	}

	private void StartMark()
	{
		creatingMark = true;
		curMark = new GameObject("Tire Mark");
		curMarkTr = curMark.transform;
		curMarkTr.parent = w.contactPoint.col.transform;
		curMark.AddComponent<TireMark>();
		MeshRenderer meshRenderer = curMark.AddComponent<MeshRenderer>();
		if (w.popped)
		{
			meshRenderer.material = rimMarkMaterials[Mathf.Min(w.contactPoint.surfaceType, rimMarkMaterials.Length - 1)];
		}
		else
		{
			meshRenderer.material = tireMarkMaterials[Mathf.Min(w.contactPoint.surfaceType, tireMarkMaterials.Length - 1)];
		}
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		mesh = curMark.AddComponent<MeshFilter>().mesh;
		verts = new Vector3[GlobalControl.tireMarkLengthStatic * 2];
		tris = new int[GlobalControl.tireMarkLengthStatic * 3];
		if (continueMark)
		{
			verts[0] = leftPointPrev;
			verts[1] = rightPointPrev;
			tris[0] = 0;
			tris[1] = 3;
			tris[2] = 1;
			tris[3] = 0;
			tris[4] = 2;
			tris[5] = 3;
		}
		uvs = new Vector2[verts.Length];
		uvs[0] = new Vector2(0f, 0f);
		uvs[1] = new Vector2(1f, 0f);
		uvs[2] = new Vector2(0f, 1f);
		uvs[3] = new Vector2(1f, 1f);
		colors = new Color[verts.Length];
		colors[0].a = 0f;
		colors[1].a = 0f;
		curEdge = 2;
		gapDelay = GlobalControl.tireMarkGapStatic;
	}

	private void UpdateMark()
	{
		if (gapDelay == 0f)
		{
			float a = (float)((curEdge < GlobalControl.tireMarkLengthStatic - 2 && curEdge > 5) ? 1 : 0) * Random.Range(Mathf.Clamp01(Mathf.Abs(F.MaxAbs(w.sidewaysSlip, w.forwardSlip, alwaysScrape)) - slipThreshold) * 0.9f, Mathf.Clamp01(Mathf.Abs(F.MaxAbs(w.sidewaysSlip, w.forwardSlip, alwaysScrape)) - slipThreshold));
			gapDelay = GlobalControl.tireMarkGapStatic;
			curEdge += 2;
			verts[curEdge] = leftPoint;
			verts[curEdge + 1] = rightPoint;
			for (int i = curEdge + 2; i < verts.Length; i++)
			{
				verts[i] = ((!Mathf.Approximately((float)i * 0.5f, Mathf.Round((float)i * 0.5f))) ? rightPoint : leftPoint);
				colors[i].a = 0f;
			}
			tris[curEdge * 3 - 3] = curEdge;
			tris[curEdge * 3 - 2] = curEdge + 3;
			tris[curEdge * 3 - 1] = curEdge + 1;
			tris[Mathf.Min(curEdge * 3, tris.Length - 1)] = curEdge;
			tris[Mathf.Min(curEdge * 3 + 1, tris.Length - 1)] = curEdge + 2;
			tris[Mathf.Min(curEdge * 3 + 2, tris.Length - 1)] = curEdge + 3;
			uvs[curEdge] = new Vector2(0f, (float)curEdge * 0.5f);
			uvs[curEdge + 1] = new Vector2(1f, (float)curEdge * 0.5f);
			colors[curEdge] = new Color(1f, 1f, 1f, a);
			colors[curEdge + 1] = colors[curEdge];
			mesh.vertices = verts;
			mesh.triangles = tris;
			mesh.uv = uvs;
			mesh.colors = colors;
			mesh.RecalculateBounds();
		}
		else
		{
			gapDelay = Mathf.Max(0f, gapDelay - Time.deltaTime);
			verts[curEdge] = leftPoint;
			verts[curEdge + 1] = rightPoint;
			for (int j = curEdge + 2; j < verts.Length; j++)
			{
				verts[j] = ((!Mathf.Approximately((float)j * 0.5f, Mathf.Round((float)j * 0.5f))) ? rightPoint : leftPoint);
				colors[j].a = 0f;
			}
			mesh.vertices = verts;
			mesh.RecalculateBounds();
		}
	}

	private void EndMark()
	{
		creatingMark = false;
		leftPointPrev = verts[Mathf.RoundToInt((float)verts.Length * 0.5f)];
		rightPointPrev = verts[Mathf.RoundToInt((float)verts.Length * 0.5f + 1f)];
		continueMark = w.grounded;
		curMark.GetComponent<TireMark>().fadeTime = GlobalControl.tireFadeTimeStatic;
		curMark.GetComponent<TireMark>().mesh = mesh;
		curMark.GetComponent<TireMark>().colors = colors;
		curMark = null;
		curMarkTr = null;
		mesh = null;
	}

	private void OnDestroy()
	{
		if (creatingMark && (bool)curMark)
		{
			EndMark();
		}
	}
}
