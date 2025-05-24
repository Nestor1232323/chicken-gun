using System.Collections.Generic;
using UnityEngine;

public class Device
{
	private static Dictionary<string, int> perfomanceGPU = new Dictionary<string, int>
	{
		{ "Adreno (TM) 330", 17 },
		{ "PowerVR SGX 554MP", 15 },
		{ "Mali-T628", 15 },
		{ "Mali-T624", 15 },
		{ "PowerVR G6430", 15 },
		{ "PowerVR Rogue", 14 },
		{ "Mali-T604", 11 },
		{ "Adreno (TM) 320", 11 },
		{ "PowerVR SGX G6200", 10 },
		{ "PowerVR SGX 543MP", 8 },
		{ "PowerVR SGX 544MP", 8 },
		{ "Intel HD Graphics", 8 },
		{ "Mali-450 MP", 8 },
		{ "Vivante GC4000", 6 },
		{ "Adreno (TM) 305", 5 },
		{ "NVIDIA Tegra 3", 5 },
		{ "NVIDIA Tegra 3 / Chainfire3D", 5 },
		{ "Vivante GC2000", 5 },
		{ "GC2000 core / Chainfire3D", 5 },
		{ "Mali-400 MP", 4 },
		{ "MALI-400MP4", 4 },
		{ "Mali-400 MP / Chainfire3D", 4 },
		{ "Adreno (TM) 225", 4 },
		{ "VideoCore IV HW", 4 },
		{ "NVIDIA Tegra", 3 },
		{ "GC1000 core", 3 },
		{ "Adreno (TM) 220", 3 },
		{ "Adreno (TM) 220 / Chainfire3D", 3 },
		{ "Vivante GC1000", 3 },
		{ "PowerVR SGX 540", 2 },
		{ "PowerVR SGX 540 / Chainfire3D", 2 },
		{ "Adreno (TM) 203", 2 },
		{ "PowerVR SGX 531", 1 },
		{ "PowerVR SGX 531 / Chainfire3D", 2 },
		{ "Immersion.16", 1 },
		{ "Immersion.16 / Chainfire3D", 1 },
		{ "Bluestacks", 1 },
		{ "GC800 core", 1 },
		{ "GC800 core / Chainfire3D", 1 },
		{ "Adreno (TM) 200", 1 },
		{ "Adreno (TM) 200 / Chainfire3D", 1 },
		{ "Mali-300", 1 },
		{ "GC400 core", 1 },
		{ "S5 Multicore c", 1 },
		{ "PowerVR SGX 535", 1 },
		{ "PowerVR SGX 543", 1 }
	};

	public static List<string> LowDepthBuffer = new List<string>
	{
		"NVIDIA Tegra", "NVIDIA Tegra 2", "NVIDIA Tegra 3", "NVIDIA Tegra 3 / Chainfire3D", "Adreno (TM) 305", "Adreno (TM) 225", "Adreno (TM) 220", "Adreno (TM) 220 / Chainfire3D", "Adreno (TM) 203", "Adreno (TM) 200",
		"Adreno (TM) 200 / Chainfire3D"
	};

	private static bool _wasBufferChecked;

	private static bool _isLowBuffer;

	public static bool isWeakDevice
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return SystemInfo.systemMemorySize < 1800;
			}
			return false;
		}
	}

	public static bool IsLowDepthBuffer()
	{
		if (_wasBufferChecked)
		{
			return _isLowBuffer;
		}
		_isLowBuffer = LowDepthBuffer.Contains(SystemInfo.graphicsDeviceName);
		_wasBufferChecked = true;
		return _isLowBuffer;
	}

	private static bool HighPerfomanceVideoChip(int LevelPerfomance)
	{
		if (perfomanceGPU.ContainsKey(SystemInfo.graphicsDeviceName))
		{
			return perfomanceGPU[SystemInfo.graphicsDeviceName] >= LevelPerfomance;
		}
		return true;
	}
}
