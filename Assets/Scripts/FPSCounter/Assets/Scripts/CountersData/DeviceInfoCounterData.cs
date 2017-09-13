using System;
using System.Text;
using CodeStage.AdvancedFPSCounter.Labels;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter.CountersData
{
	/// <summary>
	/// Shows additional device information.
	/// </summary>
	[Serializable]
	public class DeviceInfoCounterData: BaseCounterData
	{
		[HideInInspector]
		public string lastValue = "";

		[SerializeField]
		private bool cpuModel = true;

		[SerializeField]
		private bool gpuModel = true;

		[SerializeField]
		private bool ramSize = true;

		[SerializeField]
		private bool screenData = true;

		private bool inited;

		internal DeviceInfoCounterData()
		{
			color = new Color32(172, 172, 172, 255);
			anchor = LabelAnchor.LowerLeft;
		}

		#region properties
		/// <summary>
		/// Shows CPU model name and maximum supported threads count.
		/// </summary>
		public bool CpuModel
		{
			get { return cpuModel; }
			set
			{
				if (cpuModel == value || !Application.isPlaying) return;
				cpuModel = value;
				if (!enabled) return;

				Refresh();
			}
		}

		/// <summary>
		/// Shows GPU model name, supported shader model (if possible) and total Video RAM size (if possible).
		/// </summary>
		public bool GpuModel
		{
			get { return gpuModel; }
			set
			{
				if (gpuModel == value || !Application.isPlaying) return;
				gpuModel = value;
				if (!enabled) return;

				Refresh();
			}
		}

		/// <summary>
		/// Shows total RAM size.
		/// </summary>
		public bool RamSize
		{
			get { return ramSize; }
			set
			{
				if (ramSize == value || !Application.isPlaying) return;
				ramSize = value;
				if (!enabled) return;

				Refresh();
			}
		}

		/// <summary>
		/// Shows screen resolution, size and DPI (if possible).
		/// </summary>
		public bool ScreenData
		{
			get { return screenData; }
			set
			{
				if (screenData == value || !Application.isPlaying) return;
				screenData = value;
				if (!enabled) return;

				Refresh();
			}
		}
		#endregion

		protected override void CacheCurrentColor()
		{
			colorCached = "<color=#" + AFPSCounter.Color32ToHex(color) + ">";
		}

		internal override void Activate()
		{
			if (!enabled || inited || !HasData()) return;
			base.Activate();

			inited = true;

			if (main.OperationMode == AFPSCounterOperationMode.Normal)
			{
				if (colorCached == null)
				{
					colorCached = "<color=#" + AFPSCounter.Color32ToHex(color) + ">";
				}
			}

			if (text == null)
			{
				text = new StringBuilder();
			}
			else
			{
				text.Remove(0, text.Length);
			}
			
			UpdateValue();
		}

		internal override void Deactivate()
		{
			if (!inited) return;
			base.Deactivate();

			if (text != null) text.Length = 0;
			main.MakeDrawableLabelDirty(anchor);

			inited = false;
		}

		internal override void UpdateValue(bool force)
		{
			if (!inited && (HasData()))
			{
				Activate();
				return;
			}

			if (inited && (!HasData()))
			{
				Deactivate();
				return;
			}

			if (!enabled) return;

			bool needNewLine = false;

			text.Remove(0, text.Length);
			
			if (cpuModel)
			{
				text.Append("CPU: ").Append(SystemInfo.processorType).Append(" (").Append(SystemInfo.processorCount).Append(" threads)");
				needNewLine = true;
			}

			if (gpuModel)
			{
				if (needNewLine) text.Append(AFPSCounter.NEW_LINE);
				text.Append("GPU: ").Append(SystemInfo.graphicsDeviceName);

				bool showSm = false;
				int sm = SystemInfo.graphicsShaderLevel;
				if (sm == 20)
				{
					text.Append(" (SM: 2.0");
					showSm = true;
				}
				else if (sm == 30)
				{
					text.Append(" (SM: 3.0");
					showSm = true;
				}
				else if (sm == 40)
				{
					text.Append(" (SM: 4.0");
					showSm = true;
				}
				else if (sm == 41)
				{
					text.Append(" (SM: 4.1");
					showSm = true;
				}
				else if (sm == 50)
				{
					text.Append(" (SM: 5.0");
					showSm = true;
				}

				int vram = SystemInfo.graphicsMemorySize;
				if (vram > 0)
				{
					if (showSm)
					{
						text.Append(", VRAM: ").Append(vram).Append(" MB)");
					}
					else
					{
						text.Append("(VRAM: ").Append(vram).Append(" MB)");
					}
				}
				else if (showSm)
				{
					text.Append(")");
				}
				needNewLine = true;
			}

			if (ramSize)
			{
				if (needNewLine) text.Append(AFPSCounter.NEW_LINE);

				int ram = SystemInfo.systemMemorySize;

				if (ram > 0)
				{
					text.Append("RAM: ").Append(ram).Append(" MB");
					needNewLine = true;
				}
			}

			if (screenData)
			{
				if (needNewLine) text.Append(AFPSCounter.NEW_LINE);
				Resolution res = Screen.currentResolution;

				text.Append("Screen: ").Append(res.width).Append("x").Append(res.height).Append("@").Append(res.refreshRate).Append("Hz (window size: ").Append(Screen.width).Append("x").Append(Screen.height);
				float dpi = Screen.dpi;
				if (dpi <= 0)
				{
					text.Append(")");
				}
				else
				{
					text.Append(", DPI: ").Append(dpi).Append(")");
				}
			}

			lastValue = text.ToString();

			if (main.OperationMode == AFPSCounterOperationMode.Normal)
			{
				text.Insert(0,colorCached);
				text.Append("</color>");
			}
			else
			{
				text.Length = 0;
			}

			dirty = true;
		}

		private bool HasData()
		{
			return cpuModel || gpuModel || ramSize || screenData;
		}
	}
}
