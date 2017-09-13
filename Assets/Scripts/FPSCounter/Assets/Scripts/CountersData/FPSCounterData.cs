using System;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter.CountersData
{
	/// <summary>
	/// Shows frames per second counter.
	/// </summary>
	[Serializable]
	public class FPSCounterData: BaseCounterData
	{
		private const string COROUTINE_NAME = "UpdateFPSCounter";

		private const string FPS_TEXT_START = "<color=#{0}><b>FPS: ";
		private const string FPS_TEXT_END = "</b></color>";

		private const string MIN_TEXT_START = "\n<color=#{0}><b>MIN: ";
		private const string MIN_TEXT_END = "</b></color> ";

		private const string MAX_TEXT_START = "<color=#{0}><b>MAX: ";
		private const string MAX_TEXT_END = "</b></color>";

		private const string AVG_TEXT_START = " <color=#{0}><b>AVG: ";
		private const string AVG_TEXT_END = "</b></color>";

		/// <summary>
		/// If FPS will drop below this value, colorWarning will be used for counter text.
		/// </summary>
		public int warningLevelValue = 30;

		/// <summary>
		/// If FPS will be equal or less this value, colorCritical will be used for counter text.
		/// </summary>
		public int criticalLevelValue = 10;

		/// <summary>
		/// Average FPS counter accumulative data will be reset on new scene load if enabled.
		/// </summary>
		public bool resetAverageOnNewScene = false;

		/// <summary>
		/// Minimum and maximum FPS readings will be reset on new scene load if enabled.
		/// </summary>
		public bool resetMinMaxOnNewScene = false;

		/// <summary>
		/// Last calculated FPS value.
		/// </summary>
		[HideInInspector]
		public int lastValue = 0;

		/// <summary>
		/// Last calculated Average FPS value.
		/// </summary>
		[HideInInspector]
		public int lastAverageValue = 0;

		/// <summary>
		/// Last minimum FPS value.
		/// </summary>
		[HideInInspector]
		public int lastMinimumValue = -1;

		/// <summary>
		/// Last maximum FPS value.
		/// </summary>
		[HideInInspector]
		public int lastMaximumValue = -1;

		[SerializeField]
		[Range(0.1f, 10f)]
		private float updateInterval = 0.5f;

		[SerializeField]
		private bool showAverage = true;

		[SerializeField]
		[Range(0, 100)]
		private int averageFromSamples = 100;

		[SerializeField]
		private bool showMinMax = true;

		[SerializeField]
		private Color colorWarning = new Color32(236, 224, 88, 255);

		[SerializeField]
		private Color colorCritical = new Color32(249, 91, 91, 255);

		internal int newValue;

		private string colorCachedMin;
		private string colorCachedMax;
		private string colorCachedAvg;

		private string colorWarningCached;
		private string colorWarningCachedMin;
		private string colorWarningCachedMax;
		private string colorWarningCachedAvg;

		private string colorCriticalCached;
		private string colorCriticalCachedMin;
		private string colorCriticalCachedMax;
		private string colorCriticalCachedAvg;

		private bool inited;

		private int currentAverageSamples;
		private float currentAverageRaw;
		private float[] accumulatedAverageSamples;

		internal FPSCounterData()
		{
			color = new Color32(85, 218, 102, 255);
		}

		#region properties
		/// <summary>
		/// Counter's value update interval.
		/// </summary>
		public float UpdateInterval
		{
			get { return updateInterval; }
			set
			{
				if (Math.Abs(updateInterval - value) < 0.001f || !Application.isPlaying) return;

				updateInterval = value;
				if (!enabled) return;

				RestartCoroutine();
			}
		}

		/// <summary>
		/// Shows Average FPS calculated from specified #AverageFromSamples amount or since game / scene start, depending on %AverageFromSamples value and #resetAverageOnNewScene toggle.
		/// </summary>
		public bool ShowAverage
		{
			get { return showAverage; }
			set
			{
				if (showAverage == value || !Application.isPlaying) return;
				showAverage = value;
				if (!enabled) return;
				if (!showAverage) ResetAverage();

				Refresh();
			}
		}

		/// <summary>
		/// Amount of last samples to get average from. Set 0 to get average from all samples since startup or level load. One Sample recorded per #UpdateInterval.
		/// </summary>
		public int AverageFromSamples
		{
			get { return averageFromSamples; }
			set
			{
				if (averageFromSamples == value || !Application.isPlaying) return;
				averageFromSamples = value;
				if (!enabled) return;

				if (averageFromSamples > 0)
				{
					if (accumulatedAverageSamples == null)
					{
						accumulatedAverageSamples = new float[averageFromSamples];
					}
					else if (accumulatedAverageSamples.Length != averageFromSamples)
					{
						Array.Resize(ref accumulatedAverageSamples, averageFromSamples);
					}
				}
				else
				{
					accumulatedAverageSamples = null;
				}
				ResetAverage();
				Refresh();
			}
		}

		/// <summary>
		/// Shows minimum and maximum FPS readings since game / scene start, depending on #resetMinMaxOnNewScene toggle.
		/// </summary>
		public bool ShowMinMax
		{
			get { return showMinMax; }
			set
			{
				if (showMinMax == value || !Application.isPlaying) return;
				showMinMax = value;
				if (!enabled) return;
				if (!showMinMax) ResetMinMax();

				Refresh();
			}
		}

		/// <summary>
		/// Color of the FPS counter while FPS is between criticalLevelValue and warningLevelValue.
		/// </summary>
		public Color ColorWarning
		{
			get { return colorWarning; }
			set
			{
				if (colorWarning == value || !Application.isPlaying) return;
				colorWarning = value;
				if (!enabled) return;

				CacheWarningColor();

				Refresh();
			}
		}

		/// <summary>
		/// Color of the FPS counter while FPS is below criticalLevelValue.
		/// </summary>
		public Color ColorCritical
		{
			get { return colorCritical; }
			set
			{
				if (colorCritical == value || !Application.isPlaying) return;
				colorCritical = value;
				if (!enabled) return;

				CacheCriticalColor();

				Refresh();
			}
		}

		#endregion

		/// <summary>
		/// Resets Average FPS counter accumulative data.
		/// </summary>
		public void ResetAverage()
		{
			lastAverageValue = 0;
			currentAverageSamples = 0;
			currentAverageRaw = 0;

			if (averageFromSamples > 0 && accumulatedAverageSamples != null)
			{
				Array.Clear(accumulatedAverageSamples, 0, accumulatedAverageSamples.Length);
			}
		}

		/// <summary>
		/// Resets minimum and maximum FPS readings.
		/// </summary>
		public void ResetMinMax()
		{
			lastMinimumValue = -1;
			lastMaximumValue = -1;
			UpdateValue(true);
			dirty = true;
		}

		internal override void Activate()
		{
			if (!enabled || inited) return;
			base.Activate();
			inited = true;

			lastValue = 0;

			if (main.OperationMode == AFPSCounterOperationMode.Normal)
			{
				if (colorCached == null)
				{
					CacheCurrentColor();
				}

				if (colorWarningCached == null)
				{
					CacheWarningColor();
				}

				if (colorCriticalCached == null)
				{
					CacheCriticalColor();
				}

				text.Append(colorCriticalCached).Append("0").Append(FPS_TEXT_END);
				dirty = true;
			}

			main.StartCoroutine(COROUTINE_NAME);
		}

		internal override void Deactivate()
		{
			if (!inited) return;
			base.Deactivate();

			main.StopCoroutine(COROUTINE_NAME);
			ResetMinMax();
			ResetAverage();
			lastValue = 0;

			inited = false;
		}

		internal override void UpdateValue(bool force)
		{
			if (!enabled) return;

			if (lastValue != newValue || force)
			{
				lastValue = newValue;
				dirty = true;
			}

			int currentAverageRounded = 0;
			if (showAverage)
			{
				if (averageFromSamples == 0)
				{
					currentAverageSamples++;
					currentAverageRaw += (lastValue - currentAverageRaw) / currentAverageSamples;
				}
				else
				{
					if (accumulatedAverageSamples == null)
					{
						accumulatedAverageSamples = new float[averageFromSamples];
						ResetAverage();
					}

					accumulatedAverageSamples[currentAverageSamples % averageFromSamples] = lastValue;
					currentAverageSamples++;

					currentAverageRaw = GetAverageFromAccumulatedSamples();
				}

				currentAverageRounded = Mathf.RoundToInt(currentAverageRaw);

				if (lastAverageValue != currentAverageRounded || force)
				{
					lastAverageValue = currentAverageRounded;
					dirty = true;
				}
			}

			if (showMinMax && dirty)
			{
				if (lastMinimumValue == -1)
					 lastMinimumValue = lastValue;
				else if (lastValue < lastMinimumValue)
				{
					lastMinimumValue = lastValue;
					dirty = true;
				}

				if (lastMaximumValue == -1)
					lastMaximumValue = lastValue;
				else if (lastValue > lastMaximumValue)
				{
					lastMaximumValue = lastValue;
					dirty = true;
				}
			}

			if (dirty && main.OperationMode == AFPSCounterOperationMode.Normal)
			{
				string color;

				if (lastValue >= warningLevelValue)
					color = colorCached;
				else if (lastValue <= criticalLevelValue)
					color = colorCriticalCached;
				else
					color = colorWarningCached;

				text.Length = 0;
				text.Append(color).Append(lastValue).Append(FPS_TEXT_END);

				if (showAverage)
				{
					if (currentAverageRounded >= warningLevelValue)
						color = colorCachedAvg;
					else if (currentAverageRounded <= criticalLevelValue)
						color = colorCriticalCachedAvg;
					else
						color = colorWarningCachedAvg;


					text.Append(color).Append(currentAverageRounded).Append(AVG_TEXT_END);
				}

				if (showMinMax)
				{
					if (lastMinimumValue >= warningLevelValue)
						color = colorCachedMin;
					else if (lastMinimumValue <= criticalLevelValue)
						color = colorCriticalCachedMin;
					else
						color = colorWarningCachedMin;

					text.Append(color).Append(lastMinimumValue).Append(MIN_TEXT_END);

					if (lastMaximumValue >= warningLevelValue)
						color = colorCachedMax;
					else if (lastMaximumValue <= criticalLevelValue)
						color = colorCriticalCachedMax;
					else
						color = colorWarningCachedMax;

					text.Append(color).Append(lastMaximumValue).Append(MAX_TEXT_END);
				}
			}
		}

		protected override void CacheCurrentColor()
		{
			string colorString = AFPSCounter.Color32ToHex(color);
			colorCached = String.Format(FPS_TEXT_START, colorString);
			colorCachedMin = String.Format(MIN_TEXT_START, colorString);
			colorCachedMax = String.Format(MAX_TEXT_START, colorString);
			colorCachedAvg = String.Format(AVG_TEXT_START, colorString);
		}

		protected void CacheWarningColor()
		{
			string colorString = AFPSCounter.Color32ToHex(colorWarning);
			colorWarningCached = String.Format(FPS_TEXT_START, colorString);
			colorWarningCachedMin = String.Format(MIN_TEXT_START, colorString);
			colorWarningCachedMax = String.Format(MAX_TEXT_START, colorString);
			colorWarningCachedAvg = String.Format(AVG_TEXT_START, colorString);
		}

		protected void CacheCriticalColor()
		{
			string colorString = AFPSCounter.Color32ToHex(colorCritical);
			colorCriticalCached = String.Format(FPS_TEXT_START, colorString);
			colorCriticalCachedMin = String.Format(MIN_TEXT_START, colorString);
			colorCriticalCachedMax = String.Format(MAX_TEXT_START, colorString);
			colorCriticalCachedAvg = String.Format(AVG_TEXT_START, colorString);
		}

		private void RestartCoroutine()
		{
			main.StopCoroutine(COROUTINE_NAME);
			main.StartCoroutine(COROUTINE_NAME);
		}

		private float GetAverageFromAccumulatedSamples()
		{
			float averageFps;
			float totalFps = 0;

			for (int i = 0; i < averageFromSamples; i++)
			{
				totalFps += accumulatedAverageSamples[i];
			}

			if (currentAverageSamples < averageFromSamples)
			{
				averageFps = totalFps / currentAverageSamples;
			}
			else
			{
				averageFps = totalFps / averageFromSamples;
			}

			return averageFps;
		}
	}
}