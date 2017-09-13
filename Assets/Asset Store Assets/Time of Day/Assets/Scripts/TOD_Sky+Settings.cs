using UnityEngine;

public partial class TOD_Sky : MonoBehaviour
{
	private float timeSinceFogUpdate        = float.MaxValue;
	private float timeSinceAmbientUpdate    = float.MaxValue;
	private float timeSinceReflectionUpdate = float.MaxValue;

	/// Adjust shaders and meshes according to the quality settings.
	private void SetupQualitySettings()
	{
		if (SystemInfo.graphicsDeviceID == 0) return;

		TOD_Resources resources = Components.Resources;

		Material cloudMaterial  = null;
		Material shadowMaterial = null;

		switch (CloudQuality)
		{
			case TOD_CloudQualityType.Fastest:
				cloudMaterial  = resources.CloudMaterialFastest;
				shadowMaterial = resources.ShadowMaterialFastest;
				break;
			case TOD_CloudQualityType.Density:
				cloudMaterial  = resources.CloudMaterialDensity;
				shadowMaterial = resources.ShadowMaterialDensity;
				break;
			case TOD_CloudQualityType.Bumped:
				cloudMaterial  = resources.CloudMaterialBumped;
				shadowMaterial = resources.ShadowMaterialBumped;
				break;
		}

		Mesh spaceMesh      = null;
		Mesh atmosphereMesh = null;
		Mesh clearMesh      = null;
		Mesh cloudMesh      = null;
		Mesh sunMesh        = null;
		Mesh moonMesh       = null;

		switch (MeshQuality)
		{
			case TOD_MeshQualityType.Low:
				spaceMesh      = resources.IcosphereLow;
				atmosphereMesh = resources.IcosphereLow;
				clearMesh      = resources.IcosphereLow;
				cloudMesh      = resources.HalfIcosphereLow;
				sunMesh        = resources.Quad;
				moonMesh       = resources.SphereLow;
				break;
			case TOD_MeshQualityType.Medium:
				spaceMesh      = resources.IcosphereMedium;
				atmosphereMesh = resources.IcosphereMedium;
				clearMesh      = resources.IcosphereLow;
				cloudMesh      = resources.HalfIcosphereMedium;
				sunMesh        = resources.Quad;
				moonMesh       = resources.SphereMedium;
				break;
			case TOD_MeshQualityType.High:
				spaceMesh      = resources.IcosphereHigh;
				atmosphereMesh = resources.IcosphereHigh;
				clearMesh      = resources.IcosphereLow;
				cloudMesh      = resources.HalfIcosphereHigh;
				sunMesh        = resources.Quad;
				moonMesh       = resources.SphereHigh;
				break;
		}

		if (Components.SpaceRenderer && Components.SpaceShader != resources.SpaceMaterial)
		{
			Components.SpaceShader = Components.SpaceRenderer.sharedMaterial = resources.SpaceMaterial;
		}

		if (Components.AtmosphereRenderer && Components.AtmosphereShader != resources.AtmosphereMaterial)
		{
			Components.AtmosphereShader = Components.AtmosphereRenderer.sharedMaterial = resources.AtmosphereMaterial;
		}

		if (Components.ClearRenderer && Components.ClearShader != resources.ClearMaterial)
		{
			Components.ClearShader = Components.ClearRenderer.sharedMaterial = resources.ClearMaterial;
		}

		if (Components.CloudRenderer && Components.CloudShader != cloudMaterial)
		{
			Components.CloudShader = Components.CloudRenderer.sharedMaterial = cloudMaterial;
		}

		if (Components.ShadowProjector && Components.ShadowShader != shadowMaterial)
		{
			Components.ShadowShader = Components.ShadowProjector.material = shadowMaterial;
		}

		if (Components.SunRenderer && Components.SunShader != resources.SunMaterial)
		{
			Components.SunShader = Components.SunRenderer.sharedMaterial = resources.SunMaterial;
		}

		if (Components.MoonRenderer && Components.MoonShader != resources.MoonMaterial)
		{
			Components.MoonShader = Components.MoonRenderer.sharedMaterial = resources.MoonMaterial;
		}

		if (Components.SpaceMeshFilter && Components.SpaceMeshFilter.sharedMesh != spaceMesh)
		{
			Components.SpaceMeshFilter.mesh = spaceMesh;
		}

		if (Components.AtmosphereMeshFilter && Components.AtmosphereMeshFilter.sharedMesh != atmosphereMesh)
		{
			Components.AtmosphereMeshFilter.mesh = atmosphereMesh;
		}

		if (Components.ClearMeshFilter && Components.ClearMeshFilter.sharedMesh != clearMesh)
		{
			Components.ClearMeshFilter.mesh = clearMesh;
		}

		if (Components.CloudMeshFilter && Components.CloudMeshFilter.sharedMesh != cloudMesh)
		{
			Components.CloudMeshFilter.mesh = cloudMesh;
		}

		if (Components.SunMeshFilter && Components.SunMeshFilter.sharedMesh != sunMesh)
		{
			Components.SunMeshFilter.mesh = sunMesh;
		}

		if (Components.MoonMeshFilter && Components.MoonMeshFilter.sharedMesh != moonMesh)
		{
			Components.MoonMeshFilter.mesh = moonMesh;
		}
	}

	/// Update render settings like fog, ambient light and reflections
	private void SetupRenderSettings()
	{
		if (SystemInfo.graphicsDeviceID == 0) return;

		var frame = Time.frameCount % 3;

		// Fog color
		if (frame == 0 || Fog.UpdateInterval == 0)
		{
			if (!Application.isPlaying || timeSinceFogUpdate >= Fog.UpdateInterval)
			{
				timeSinceFogUpdate = 0;
				UpdateFog();
			}
			else
			{
				timeSinceFogUpdate += Time.deltaTime;
			}
		}

		// Ambient light
		if (frame == 1 || Ambient.UpdateInterval == 0)
		{
			if (!Application.isPlaying || timeSinceAmbientUpdate >= Ambient.UpdateInterval)
			{
				timeSinceAmbientUpdate = 0;
				UpdateAmbient();
			}
			else
			{
				timeSinceAmbientUpdate += Time.deltaTime;
			}
		}

		// Reflection cubemap
		if (frame == 2 || Reflection.UpdateInterval == 0)
		{
			if (!Application.isPlaying || timeSinceReflectionUpdate >= Reflection.UpdateInterval)
			{
				timeSinceReflectionUpdate = 0;
				UpdateReflection();
			}
			else
			{
				timeSinceReflectionUpdate += Time.deltaTime;
			}
		}
	}

	/// Setup shader properties of all child materials
	private void SetupShaderProperties()
	{
		if (SystemInfo.graphicsDeviceID == 0) return;

		// Precalculations
		Vector4 cloudUV = Components.Animation.CloudUV + Components.Animation.OffsetUV;

		// Setup global shader parameters
		{
			Shader.SetGlobalFloat("TOD_Gamma",         Gamma);
			Shader.SetGlobalFloat("TOD_OneOverGamma",  OneOverGamma);

			Shader.SetGlobalColor("TOD_LightColor",    LightColor);
			Shader.SetGlobalColor("TOD_CloudColor",    CloudColor);
			Shader.SetGlobalColor("TOD_SunColor",      SunColor);
			Shader.SetGlobalColor("TOD_MoonColor",     MoonColor);
			Shader.SetGlobalColor("TOD_AdditiveColor", AdditiveColor);
			Shader.SetGlobalColor("TOD_MoonHaloColor", MoonHaloColor);

			Shader.SetGlobalVector("TOD_SunDirection",   SunDirection);
			Shader.SetGlobalVector("TOD_MoonDirection",  MoonDirection);
			Shader.SetGlobalVector("TOD_LightDirection", LightDirection);

			Shader.SetGlobalVector("TOD_LocalSunDirection",   LocalSunDirection);
			Shader.SetGlobalVector("TOD_LocalMoonDirection",  LocalMoonDirection);
			Shader.SetGlobalVector("TOD_LocalLightDirection", LocalLightDirection);

			Shader.SetGlobalFloat("TOD_Contrast",  Atmosphere.Contrast * OneOverGamma);
			Shader.SetGlobalFloat("TOD_Haziness",  Atmosphere.Haziness);
			Shader.SetGlobalFloat("TOD_Fogginess", Atmosphere.Fogginess);
			Shader.SetGlobalFloat("TOD_Horizon",   HorizonOffset);

			Shader.SetGlobalVector("TOD_OpticalDepth",      opticalDepth);
			Shader.SetGlobalVector("TOD_OneOverBeta",       oneOverBeta);
			Shader.SetGlobalVector("TOD_BetaRayleigh",      betaRayleigh);
			Shader.SetGlobalVector("TOD_BetaRayleighTheta", betaRayleighTheta);
			Shader.SetGlobalVector("TOD_BetaMie",           betaMie);
			Shader.SetGlobalVector("TOD_BetaMieTheta",      betaMieTheta);
			Shader.SetGlobalVector("TOD_BetaMiePhase",      betaMiePhase);
			Shader.SetGlobalVector("TOD_BetaNight",         betaNight);

			Shader.SetGlobalMatrix("TOD_World2Sky", Components.DomeTransform.worldToLocalMatrix);
			Shader.SetGlobalMatrix("TOD_Sky2World", Components.DomeTransform.localToWorldMatrix);
		}

		// Setup cloud shader
		if (Components.CloudShader)
		{
			float glowBase = (1-Atmosphere.Fogginess) * Clouds.Glow * Clouds.Brightness;
			float sunGlow  = glowBase * Day.CloudMultiplier * LerpValue * 2;
			float moonGlow = glowBase * Night.CloudMultiplier * (1 - Mathf.Abs(Moon.Phase)) * 4;

			Components.CloudShader.SetFloat("_SunGlow",        sunGlow);
			Components.CloudShader.SetFloat("_MoonGlow",       moonGlow);
			Components.CloudShader.SetFloat("_CloudDensity",   Clouds.Density);
			Components.CloudShader.SetFloat("_CloudSharpness", Clouds.Sharpness);
			Components.CloudShader.SetVector("_CloudScale1",   Clouds.Scale1);
			Components.CloudShader.SetVector("_CloudScale2",   Clouds.Scale2);
			Components.CloudShader.SetVector("_CloudUV",       cloudUV);
		}

		// Setup space shader
		if (Components.SpaceShader)
		{
			Components.SpaceShader.SetFloat("_Tiling",     Stars.Tiling);
			Components.SpaceShader.SetFloat("_Brightness", Stars.Brightness * (1-Atmosphere.Fogginess) * (1-LerpValue));
		}

		// Setup sun shader
		if (Components.SunShader)
		{
			Components.SunShader.SetColor("_Color",      Sun.MeshColor * (1-Atmosphere.Fogginess));
			Components.SunShader.SetFloat("_Contrast",   Sun.MeshContrast);
			Components.SunShader.SetFloat("_Brightness", Sun.MeshBrightness);
		}

		// Setup moon shader
		if (Components.MoonShader)
		{
			Components.MoonShader.SetColor("_Color",      Moon.MeshColor);
			Components.MoonShader.SetFloat("_Phase",      Moon.Phase);
			Components.MoonShader.SetFloat("_Contrast",   Moon.MeshContrast);
			Components.MoonShader.SetFloat("_Brightness", Moon.MeshBrightness * (1-Atmosphere.Fogginess) * (1-LerpValue));
		}

		// Setup shadow shader
		if (Components.ShadowShader)
		{
			float shadowAlpha = Clouds.ShadowStrength * Mathf.Clamp01(1f - LightZenith / 90f);

			Components.ShadowShader.SetFloat("_Alpha",          shadowAlpha);
			Components.ShadowShader.SetFloat("_CloudDensity",   Clouds.Density);
			Components.ShadowShader.SetFloat("_CloudSharpness", Clouds.Sharpness);
			Components.ShadowShader.SetVector("_CloudScale1",   Clouds.Scale1);
			Components.ShadowShader.SetVector("_CloudScale2",   Clouds.Scale2);
			Components.ShadowShader.SetVector("_CloudUV",       cloudUV);
		}

		// Setup shadow projector
		if (Components.ShadowProjector)
		{
			var farClipPlane     = Radius * 2;
			var orthographicSize = Radius;

			#if UNITY_EDITOR
			if (Components.ShadowProjector.farClipPlane != farClipPlane)
			#endif
			{
				Components.ShadowProjector.farClipPlane = farClipPlane;
			}

			#if UNITY_EDITOR
			if (Components.ShadowProjector.orthographicSize != orthographicSize)
			#endif
			{
				Components.ShadowProjector.orthographicSize = orthographicSize;
			}
		}
	}
}
