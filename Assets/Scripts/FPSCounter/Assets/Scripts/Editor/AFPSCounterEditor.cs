using CodeStage.AdvancedFPSCounter.Labels;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter
{
	[CustomEditor(typeof(AFPSCounter))]
	public class AFPSCounterEditor: Editor
	{
		private AFPSCounter self;

		private SerializedProperty operationMode;
		private SerializedProperty fpsGroupToggle;
		private SerializedProperty fpsCounter;
		private SerializedProperty fpsCounterEnabled;
		private SerializedProperty fpsCounterAnchor;
		private SerializedProperty fpsCounterUpdateInterval;
		private SerializedProperty fpsCounterShowMinMax;
		private SerializedProperty fpsCounterResetMinMaxOnNewScene;
		private SerializedProperty fpsCounterShowAverage;
		private SerializedProperty fpsCounterAverageFromSamples;
		private SerializedProperty fpsCounterResetAverageOnNewScene;
		private SerializedProperty fpsCounterWarningLevelValue;
		private SerializedProperty fpsCounterCriticalLevelValue;
		private SerializedProperty fpsCounterColor;
		private SerializedProperty fpsCounterColorWarning;
		private SerializedProperty fpsCounterColorCritical;

		private SerializedProperty memoryGroupToggle;
		private SerializedProperty memoryCounter;
		private SerializedProperty memoryCounterEnabled;
		private SerializedProperty memoryCounterAnchor;
		private SerializedProperty memoryCounterUpdateInterval;
		private SerializedProperty memoryCounterPreciseValues;
		private SerializedProperty memoryCounterColor;
		private SerializedProperty memoryCounterTotalReserved;
		private SerializedProperty memoryCounterAllocated;
		private SerializedProperty memoryCounterMonoUsage;

		private SerializedProperty deviceGroupToggle;
		private SerializedProperty deviceCounter;
		private SerializedProperty deviceCounterEnabled;
		private SerializedProperty deviceCounterAnchor;
		private SerializedProperty deviceCounterColor;
		private SerializedProperty deviceCounterCpuModel;
		private SerializedProperty deviceCounterGpuModel;
		private SerializedProperty deviceCounterRamSize;
		private SerializedProperty deviceCounterScreenData;

		private SerializedProperty lookAndFeelToggle;
		private SerializedProperty labelsFont;
		private SerializedProperty fontSize;
		private SerializedProperty lineSpacing;
		private SerializedProperty anchorsOffset;

		private SerializedProperty hotKey;
		private SerializedProperty keepAlive;
		private SerializedProperty forceFrameRate;
		private SerializedProperty forcedFrameRate;

		public void OnEnable()
		{
			self = (target as AFPSCounter);

			operationMode = serializedObject.FindProperty("operationMode");

			fpsGroupToggle = serializedObject.FindProperty("fpsGroupToggle");

			fpsCounter = serializedObject.FindProperty("fpsCounter");
			fpsCounterEnabled = fpsCounter.FindPropertyRelative("enabled");
			fpsCounterUpdateInterval = fpsCounter.FindPropertyRelative("updateInterval");
			fpsCounterAnchor = fpsCounter.FindPropertyRelative("anchor");
			fpsCounterShowMinMax = fpsCounter.FindPropertyRelative("showMinMax");
			fpsCounterResetMinMaxOnNewScene = fpsCounter.FindPropertyRelative("resetMinMaxOnNewScene");
			fpsCounterShowAverage = fpsCounter.FindPropertyRelative("showAverage");
			fpsCounterAverageFromSamples = fpsCounter.FindPropertyRelative("averageFromSamples");
			fpsCounterResetAverageOnNewScene = fpsCounter.FindPropertyRelative("resetAverageOnNewScene");
			fpsCounterWarningLevelValue = fpsCounter.FindPropertyRelative("warningLevelValue");
			fpsCounterCriticalLevelValue = fpsCounter.FindPropertyRelative("criticalLevelValue");
			fpsCounterColor = fpsCounter.FindPropertyRelative("color");
			fpsCounterColorWarning = fpsCounter.FindPropertyRelative("colorWarning");
			fpsCounterColorCritical = fpsCounter.FindPropertyRelative("colorCritical");

			memoryGroupToggle = serializedObject.FindProperty("memoryGroupToggle");

			memoryCounter = serializedObject.FindProperty("memoryCounter");
			memoryCounterEnabled = memoryCounter.FindPropertyRelative("enabled");
			memoryCounterUpdateInterval = memoryCounter.FindPropertyRelative("updateInterval");
			memoryCounterAnchor = memoryCounter.FindPropertyRelative("anchor");
			memoryCounterPreciseValues = memoryCounter.FindPropertyRelative("preciseValues");
			memoryCounterColor = memoryCounter.FindPropertyRelative("color");
			memoryCounterTotalReserved = memoryCounter.FindPropertyRelative("totalReserved");
			memoryCounterAllocated = memoryCounter.FindPropertyRelative("allocated");
			memoryCounterMonoUsage = memoryCounter.FindPropertyRelative("monoUsage");

			deviceGroupToggle = serializedObject.FindProperty("deviceGroupToggle");

			deviceCounter = serializedObject.FindProperty("deviceInfoCounter");
			deviceCounterEnabled = deviceCounter.FindPropertyRelative("enabled");
			deviceCounterAnchor = deviceCounter.FindPropertyRelative("anchor");
			deviceCounterColor = deviceCounter.FindPropertyRelative("color");
			deviceCounterCpuModel = deviceCounter.FindPropertyRelative("cpuModel");
			deviceCounterGpuModel = deviceCounter.FindPropertyRelative("gpuModel");
			deviceCounterRamSize = deviceCounter.FindPropertyRelative("ramSize");
			deviceCounterScreenData = deviceCounter.FindPropertyRelative("screenData");

			lookAndFeelToggle = serializedObject.FindProperty("lookAndFeelToggle");
			labelsFont = serializedObject.FindProperty("labelsFont");
			fontSize = serializedObject.FindProperty("fontSize");
			lineSpacing = serializedObject.FindProperty("lineSpacing");
			anchorsOffset = serializedObject.FindProperty("anchorsOffset");

			hotKey = serializedObject.FindProperty("hotKey");
			keepAlive = serializedObject.FindProperty("keepAlive");
			forceFrameRate = serializedObject.FindProperty("forceFrameRate");
			forcedFrameRate = serializedObject.FindProperty("forcedFrameRate");
		}

		public override void OnInspectorGUI()
		{
			if (self == null) return;

			serializedObject.Update();

			int indent = EditorGUI.indentLevel;

			if (PropertyFieldChanged(operationMode, new GUIContent("Operation Mode", "Disabled: removes labels and stops all internal processes except Hot Key listener\n\nBackground: removes labels keeping counters alive; use for hidden performance monitoring\n\nNormal: shows labels and runs all internal processes as usual")))
			{
				self.OperationMode = (AFPSCounterOperationMode)operationMode.enumValueIndex;
			}

			EditorGUILayout.PropertyField(hotKey, new GUIContent("Hot Key", "Used to enable / disable plugin. Set to None to disable"));
			EditorGUILayout.PropertyField(keepAlive, new GUIContent("Keep Alive", "Prevent current Game Object from destroying on level (scene) load"));

			if (PropertyFieldChanged(forceFrameRate, new GUIContent("Force FPS", "Allows to see how your game performs on specified frame rate.\nIMPORTANT: this option disables VSync while enabled!")))
			{
				self.ForceFrameRate = forceFrameRate.boolValue;
			}

			
			EditorGUI.indentLevel = 1;
			if (PropertyFieldChanged(forcedFrameRate, new GUIContent("Desired frame rate", "Does not guarantee selected frame rate. Set -1 to render as fast as possible in current conditions")))
			{
				self.ForcedFrameRate = forcedFrameRate.intValue;
			}
			EditorGUI.indentLevel = indent;

			if (Foldout(lookAndFeelToggle, "Look and feel"))
			{
				EditorGUI.indentLevel = 1;

				if (PropertyFieldChanged(labelsFont, new GUIContent("Labels font", "Leave blank to use default font")))
				{
					self.LabelsFont = (Font)labelsFont.objectReferenceValue;
				}

				// workaround for layout issues in Unity 4.2
#if UNITY_4_2
				if (PropertyFieldChanged(fontSize, new GUIContent(" Font size", "Set to 0 to use font size specified in the font importer")))
				{
					self.FontSize = fontSize.intValue;
				}

				if (PropertyFieldChanged(lineSpacing, new GUIContent(" Labels line spacing")))
				{
					self.LineSpacing = lineSpacing.floatValue;
				}
#else
				if (PropertyFieldChanged(fontSize, new GUIContent("Font size", "Set to 0 to use font size specified in the font importer")))
				{
					self.FontSize = fontSize.intValue;
				}

				if (PropertyFieldChanged(lineSpacing, new GUIContent("Labels line spacing")))
				{
					self.LineSpacing = lineSpacing.floatValue;
				}
#endif

				if (PropertyFieldChanged(anchorsOffset, new GUIContent("Labels pixel offset", "Offset in pixels, will be applied to all 4 corners automatically")))
				{
					self.AnchorsOffset = anchorsOffset.vector2Value;
				}

				EditorGUI.indentLevel = indent;

				EditorGUILayout.Space();
			}

			if (ToggleFoldout(fpsGroupToggle, "FPS Counter", fpsCounterEnabled))
			{
				self.fpsCounter.Enabled = fpsCounterEnabled.boolValue;
			}

			if (fpsGroupToggle.boolValue)
			{
				EditorGUI.indentLevel = 2;

				if (PropertyFieldChanged(fpsCounterUpdateInterval))
				{
					self.fpsCounter.UpdateInterval = fpsCounterUpdateInterval.floatValue;
				}

				if (PropertyFieldChanged(fpsCounterAnchor))
				{
					self.fpsCounter.Anchor = (LabelAnchor)fpsCounterAnchor.enumValueIndex;
				}

				if (PropertyFieldChanged(fpsCounterShowMinMax, new GUIContent("MinMax FPS", "Shows minimum and maximum FPS readouts since game or scene start, depending on 'Reset On Load' toggle")))
				{
					self.fpsCounter.ShowMinMax = fpsCounterShowMinMax.boolValue;
				}

				if (fpsCounterShowMinMax.boolValue)
				{
					EditorGUI.indentLevel = 3;

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(fpsCounterResetMinMaxOnNewScene, new GUIContent("Reset On Load", "Minimum and maximum FPS readouts will be reset on new scene load if enabled"));
					if (GUILayout.Button("Reset now"))
					{
						self.fpsCounter.ResetMinMax();
					}
					EditorGUILayout.EndHorizontal();

					EditorGUI.indentLevel = 2;
				}


				if (PropertyFieldChanged(fpsCounterShowAverage, new GUIContent("Average FPS", "Shows Average FPS calculated from specified Samples amount or since game or scene start, depending on Samples value and 'Reset On Load' toggle")))
				{
					self.fpsCounter.ShowAverage = fpsCounterShowAverage.boolValue;
				}

				if (fpsCounterShowAverage.boolValue)
				{
					EditorGUI.indentLevel = 3;

					if (PropertyFieldChanged(fpsCounterAverageFromSamples, new GUIContent("Samples", "Amount of last samples to get average from. Set 0 to get average from all samples since startup or level load. One Sample recodred per Interval")))
					{
						self.fpsCounter.AverageFromSamples = fpsCounterAverageFromSamples.intValue;
					}
					
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(fpsCounterResetAverageOnNewScene, new GUIContent("Reset On Load", "Average FPS counter accumulative data will be reset on new scene load if enabled"));
					if (GUILayout.Button("Reset now"))
					{
						self.fpsCounter.ResetAverage();
					}
					EditorGUILayout.EndHorizontal();
					
					EditorGUI.indentLevel = 2;
				}

				float minVal = fpsCounterCriticalLevelValue.intValue;
				float maxVal = fpsCounterWarningLevelValue.intValue;

				EditorGUILayout.MinMaxSlider(new GUIContent("Coloration range", "This range will be used to apply colors below on specific FPS:\nCritical: 0 - min\nWarning: min+1 - max-1\nNormal: max+"), ref minVal, ref maxVal, 1, 60);

				fpsCounterCriticalLevelValue.intValue = (int)minVal;
				fpsCounterWarningLevelValue.intValue = (int)maxVal;

				GUILayout.BeginHorizontal();

				if (PropertyFieldChanged(fpsCounterColor, new GUIContent("Normal Color")))
				{
					self.fpsCounter.Color = fpsCounterColor.colorValue;
				}
				EditorGUILayout.LabelField(maxVal + "+ FPS");
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();

				if (PropertyFieldChanged(fpsCounterColorWarning, new GUIContent("Warning Color")))
				{
					self.fpsCounter.ColorWarning = fpsCounterColorWarning.colorValue;
				}
				EditorGUILayout.LabelField((minVal + 1) + " - " + (maxVal - 1) + " FPS");
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				if (PropertyFieldChanged(fpsCounterColorCritical, new GUIContent("Critical Color")))
				{
					self.fpsCounter.ColorCritical = fpsCounterColorCritical.colorValue;
				}
				EditorGUILayout.LabelField("0 - " + minVal + " FPS");
				GUILayout.EndHorizontal();


				EditorGUI.indentLevel = indent;
			}

			if (ToggleFoldout(memoryGroupToggle, "Memory Counter", memoryCounterEnabled))
			{
				self.memoryCounter.Enabled = memoryCounterEnabled.boolValue;
			}

			if (memoryGroupToggle.boolValue)
			{
				EditorGUI.indentLevel = 2;

				if (PropertyFieldChanged(memoryCounterUpdateInterval))
				{
					self.memoryCounter.UpdateInterval = memoryCounterUpdateInterval.floatValue;
				}

				if (PropertyFieldChanged(memoryCounterAnchor))
				{
					self.memoryCounter.Anchor = (LabelAnchor)memoryCounterAnchor.enumValueIndex;
				}

				if (PropertyFieldChanged(memoryCounterPreciseValues, new GUIContent("Precise", "Maked memory usage output more precise thus using more system resources (not recommended)")))
				{
					self.memoryCounter.PreciseValues = memoryCounterPreciseValues.boolValue;
				}

				if (PropertyFieldChanged(memoryCounterColor, new GUIContent("Color")))
				{
					self.memoryCounter.Color = memoryCounterColor.colorValue;
				}

				if (PropertyFieldChanged(memoryCounterTotalReserved, new GUIContent("Total Counter", "Total reserved memory size")))
				{
					self.memoryCounter.TotalReserved = memoryCounterTotalReserved.boolValue;
				}

				if (PropertyFieldChanged(memoryCounterAllocated, new GUIContent("Allocated Counter", "Amount of allocated memory")))
				{
					self.memoryCounter.Allocated = memoryCounterAllocated.boolValue;
				}

				if (PropertyFieldChanged(memoryCounterMonoUsage, new GUIContent("Mono Counter", "Amount of memory used by mamaged Mono objects")))
				{
					self.memoryCounter.MonoUsage = memoryCounterMonoUsage.boolValue;
				}

				EditorGUI.indentLevel = indent;
			}


			if (ToggleFoldout(deviceGroupToggle, "Device Information", deviceCounterEnabled))
			{
				self.deviceInfoCounter.Enabled = deviceCounterEnabled.boolValue;
			}

			if (deviceGroupToggle.boolValue)
			{
				EditorGUI.indentLevel = 2;

				if (PropertyFieldChanged(deviceCounterAnchor))
				{
					self.deviceInfoCounter.Anchor = (LabelAnchor)deviceCounterAnchor.intValue;
				}

				if (PropertyFieldChanged(deviceCounterColor, new GUIContent("Color")))
				{
					self.deviceInfoCounter.Color = deviceCounterColor.colorValue;
				}

				if (PropertyFieldChanged(deviceCounterCpuModel, new GUIContent("CPU")))
				{
					self.deviceInfoCounter.CpuModel = deviceCounterCpuModel.boolValue;
				}

				if (PropertyFieldChanged(deviceCounterGpuModel, new GUIContent("GPU")))
				{
					self.deviceInfoCounter.GpuModel = deviceCounterGpuModel.boolValue;
				}

				if (PropertyFieldChanged(deviceCounterRamSize, new GUIContent("RAM")))
				{
					self.deviceInfoCounter.RamSize = deviceCounterRamSize.boolValue;
				}

				if (PropertyFieldChanged(deviceCounterScreenData, new GUIContent("Screen")))
				{
					self.deviceInfoCounter.ScreenData = deviceCounterScreenData.boolValue;
				}

				EditorGUI.indentLevel = indent;
			}



			EditorGUILayout.Space();

			serializedObject.ApplyModifiedProperties();
		}

		private bool PropertyFieldChanged(SerializedProperty property)
		{
			return PropertyFieldChanged(property, null);
		}

		private bool PropertyFieldChanged(SerializedProperty property, GUIContent content)
		{
			bool result = false;

			EditorGUI.BeginChangeCheck();
			if (content == null)
			{
				EditorGUILayout.PropertyField(property);
			}
			else
			{
				EditorGUILayout.PropertyField(property, content);
			}
			if (EditorGUI.EndChangeCheck())
			{
				result = true;
			}

			return result;
		}

		private bool ToggleFoldout(SerializedProperty foldout, string caption, SerializedProperty toggle)
		{
			bool toggleStateChanged = false;

			Rect foldoutRect = EditorGUILayout.BeginHorizontal();
			Rect toggleRect = new Rect(foldoutRect);

			toggleRect.width = 15;

			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(toggleRect, toggle, new GUIContent(""));
			if (EditorGUI.EndChangeCheck())
			{
				toggleStateChanged = true;
			}

#if UNITY_4_1 || UNITY_4_2
			foldoutRect.xMin = toggleRect.xMax;
#else
			foldoutRect.xMin = toggleRect.xMax + 15;
#endif
			foldout.boolValue = EditorGUI.Foldout(foldoutRect, foldout.boolValue, caption, true);
			EditorGUILayout.LabelField("");
			EditorGUILayout.EndHorizontal();

			return toggleStateChanged;
		}

		private bool Foldout(SerializedProperty foldout, string caption)
		{
			Rect foldoutRect = EditorGUILayout.BeginHorizontal();
			foldout.boolValue = EditorGUI.Foldout(foldoutRect, foldout.boolValue, caption, true);
			EditorGUILayout.LabelField("");
			EditorGUILayout.EndHorizontal();

			return foldout.boolValue;
		}
	}
}