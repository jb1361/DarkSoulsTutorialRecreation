#if UNITY_3_0||UNITY_3_1||UNITY_3_2||UNITY_3_3||UNITY_3_4||UNITY_3_5||UNITY_3_6||UNITY_3_7||UNITY_3_8||UNITY_3_9
#define UNITY_3
#endif

#if UNITY_4_0||UNITY_4_1||UNITY_4_2||UNITY_4_3||UNITY_4_4||UNITY_4_5||UNITY_4_6||UNITY_4_7||UNITY_4_8||UNITY_4_9
#define UNITY_4
#endif
#if UNITY_5_0||UNITY_5_1||UNITY_5_2||UNITY_5_3||UNITY_5_4||UNITY_5_5||UNITY_5_6||UNITY_5_7||UNITY_5_8||UNITY_5_9
#define UNITY_5
#endif

using UnityEngine;
using System.Collections.Generic;

#if !UNITY_3
using UnityEngine.Rendering;
#endif

public partial class TOD_Sky : MonoBehaviour
{
	private static List<TOD_Sky> instances = new List<TOD_Sky>();

	//
	// Static properties
	//

	/// All currently active sky dome instances.
	public static List<TOD_Sky> Instances
	{
		get
		{
			return instances;
		}
	}

	/// The most recently created sky dome instance.
	public static TOD_Sky Instance
	{
		get
		{
			return instances.Count == 0 ? null : instances[instances.Count-1];
		}
	}

	//
	// Inspector variables
	//

	/// Inspector variable to adjust the color space.
	public TOD_ColorSpaceDetection UnityColorSpace = TOD_ColorSpaceDetection.Auto;

	/// Inspector variable to adjust the cloud quality.
	public TOD_CloudQualityType CloudQuality = TOD_CloudQualityType.Bumped;

	/// Inspector variable to adjust the mesh quality.
	public TOD_MeshQualityType MeshQuality = TOD_MeshQualityType.High;

	/// Inspector variable containing parameters of the day and night cycle.
	public TOD_CycleParameters Cycle;

	/// Inspector variable containing parameters of the world.
	public TOD_WorldParameters World;

	/// Inspector variable containing parameters of the atmosphere.
	public TOD_AtmosphereParameters Atmosphere;

	/// Inspector variable containing parameters of the day.
	public TOD_DayParameters Day;

	/// Inspector variable containing parameters of the night.
	public TOD_NightParameters Night;

	/// Inspector variable containing parameters of the sun.
	public TOD_SunParameters Sun;

	/// Inspector variable containing parameters of the moon.
	public TOD_MoonParameters Moon;

	/// Inspector variable containing parameters of the light source.
	public TOD_LightParameters Light;

	/// Inspector variable containing parameters of the stars.
	public TOD_StarParameters Stars;

	/// Inspector variable containing parameters of the cloud layers.
	public TOD_CloudParameters Clouds;

	/// Inspector variable containing parameters of the fog.
	public TOD_FogParameters Fog;

	/// Inspector variable containing parameters of the ambient light.
	public TOD_AmbientParameters Ambient;

	/// Inspector variable containing parameters of the reflection cubemap.
	public TOD_ReflectionParameters Reflection;

	//
	// Class properties
	//

	/// Whether or not the sky dome was successfully initialized.
	internal bool Initialized
	{
		get; private set;
	}

	/// Containins references to all components.
	internal TOD_Components Components
	{
		get; private set;
	}

	/// Boolean to check if it is day.
	internal bool IsDay
	{
		get; private set;
	}

	/// Boolean to check if it is night.
	internal bool IsNight
	{
		get; private set;
	}

	/// Radius of the sky dome.
	internal float Radius
	{
		get { return Components.DomeTransform.lossyScale.y; }
	}

	/// Height level of the sky dome.
	internal float Level
	{
		get { return Components.DomeTransform.position.y; }
	}

	/// Gamma value that is being used in the shaders.
	internal float Gamma
	{
		get { return (UnityColorSpace == TOD_ColorSpaceDetection.Auto && QualitySettings.activeColorSpace == ColorSpace.Linear || UnityColorSpace == TOD_ColorSpaceDetection.Linear) ? 1.0f : 2.2f; }
	}

	/// Inverse of the gamma value (1 / Gamma) that is being used in the shaders.
	internal float OneOverGamma
	{
		get { return (UnityColorSpace == TOD_ColorSpaceDetection.Auto && QualitySettings.activeColorSpace == ColorSpace.Linear || UnityColorSpace == TOD_ColorSpaceDetection.Linear) ? 1.0f/1.0f : 1.0f/2.2f; }
	}

	/// Falls off the darker the sunlight gets.
	/// Can for example be used to lerp between day and night values in shaders.
	/// \n = +1 at day
	/// \n = 0 at night
	internal float LerpValue
	{
		get; private set;
	}

	/// Sun zenith angle in degrees.
	/// \n = 0   if the sun is exactly at zenith.
	/// \n = 180 if the sun is exactly below the ground.
	internal float SunZenith
	{
		get; private set;
	}

	/// Moon zenith angle in degrees.
	/// \n = 0   if the moon is exactly at zenith.
	/// \n = 180 if the moon is exactly below the ground.
	internal float MoonZenith
	{
		get; private set;
	}

	/// Horizon angle in degrees.
	/// \n = 90  if the horizon is exactly in the middle of the sky dome.
	/// \n = 180 if the horizon is exactly at the bottom of the sky dome.
	internal float HorizonAngle
	{
		get { return (World.Horizon == TOD_HorizonType.Static) ? 90f : 90f + Mathf.Asin(HorizonOffset) * Mathf.Rad2Deg; }
	}

	/// Relative horizon offset.
	/// \n = 0 if the horizon is exactly in the middle of the sky dome.
	/// \n = 1 if the horizon is exactly at the bottom of the sky dome.
	internal float HorizonOffset
	{
		get { return (World.Horizon == TOD_HorizonType.Static) ? 0f : Mathf.Clamp01((Level - World.ZeroLevel) / Radius); }
	}

	/// Absolute horizon height level in world space.
	internal float HorizonLevel
	{
		get { return World.ZeroLevel - Level; }
	}

	/// Currently active light source zenith angle in degrees.
	/// \n = 0  if the currently active light source (sun or moon) is exactly at zenith.
	/// \n = 90 if the currently active light source (sun or moon) is exactly at the horizon.
	internal float LightZenith
	{
		get { return Mathf.Min(SunZenith, MoonZenith); }
	}

	/// Current light intensity.
	internal float LightIntensity
	{
		get { return Components.LightSource.intensity; }
	}

	/// Moon direction vector in world space.
	internal Vector3 MoonDirection
	{
		get; private set;
	}

	/// Sun direction vector in world space.
	internal Vector3 SunDirection
	{
		get; private set;
	}

	/// Current directional light vector in world space.
	/// Lerps between TOD_Sky.SunDirection and TOD_Sky.MoonDirection at dusk and dawn.
	internal Vector3 LightDirection
	{
		get; private set;
	}

	/// Moon direction vector in sky dome object space.
	internal Vector3 LocalMoonDirection
	{
		get; private set;
	}

	/// Sun direction vector in sky dome object space.
	internal Vector3 LocalSunDirection
	{
		get; private set;
	}

	/// Current directional light vector in sky dome object space.
	/// Lerps between TOD_Sky.LocalSunDirection and TOD_Sky.LocalMoonDirection at dusk and dawn.
	internal Vector3 LocalLightDirection
	{
		get; private set;
	}

	/// Current light color.
	/// Returns the color of TOD_Sky.Components.LightSource.
	internal Color LightColor
	{
		get { return Components.LightSource.color; }
	}

	/// Current ray color.
	internal Color RayColor
	{
		get; private set;
	}

	/// Current sun color.
	internal Color SunColor
	{
		get; private set;
	}

	/// Current moon color.
	internal Color MoonColor
	{
		get; private set;
	}

	/// Current moon halo color.
	internal Color MoonHaloColor
	{
		get; private set;
	}

	/// Current cloud color.
	internal Color CloudColor
	{
		get; private set;
	}

	/// Current additive color.
	internal Color AdditiveColor
	{
		get; private set;
	}

	/// Current ambient color.
	internal Color AmbientColor
	{
		get; private set;
	}

	//
	// Class methods
	//

	/// Convert spherical coordinates to cartesian coordinates.
	/// \param radius Spherical coordinates radius.
	/// \param theta Spherical coordinates theta.
	/// \param phi Spherical coordinates phi.
	/// \return Unity position in world space.
	internal Vector3 OrbitalToUnity(float radius, float theta, float phi)
	{
		Vector3 res;

		float sinTheta = Mathf.Sin(theta);
		float cosTheta = Mathf.Cos(theta);
		float sinPhi   = Mathf.Sin(phi);
		float cosPhi   = Mathf.Cos(phi);

		res.z = radius * sinTheta * cosPhi;
		res.y = radius * cosTheta;
		res.x = radius * sinTheta * sinPhi;

		return res;
	}

	/// Convert spherical coordinates to cartesian coordinates.
	/// \param theta Spherical coordinates theta.
	/// \param phi Spherical coordinates phi.
	/// \return Unity position in local space.
	internal Vector3 OrbitalToLocal(float theta, float phi)
	{
		Vector3 res;

		float sinTheta = Mathf.Sin(theta);
		float cosTheta = Mathf.Cos(theta);
		float sinPhi   = Mathf.Sin(phi);
		float cosPhi   = Mathf.Cos(phi);

		res.z = sinTheta * cosPhi;
		res.y = cosTheta;
		res.x = sinTheta * sinPhi;

		return res;
	}

	/// Sample atmosphere colors from the sky dome.
	/// \param direction View direction in world space.
	/// \param directLight Whether or not to include direct light.
	/// \return Color of the atmosphere in the specified direction.
	internal Color SampleAtmosphere(Vector3 direction, bool directLight = true)
	{
		direction = Components.DomeTransform.InverseTransformDirection(direction);

		const float _Gamma        = 2.2f;
		const float _OneOverGamma = 1.0f / _Gamma;

		float _Horizon   = HorizonOffset;
		float _Contrast  = Atmosphere.Contrast * _OneOverGamma;
		float _Haziness  = Atmosphere.Haziness;
		float _Fogginess = Atmosphere.Fogginess;

		Color   TOD_SunColor      = SunColor;
		Color   TOD_MoonColor     = MoonColor;
		Color   TOD_MoonHaloColor = MoonHaloColor;
		Color   TOD_CloudColor    = CloudColor;
		Color   TOD_AdditiveColor = AdditiveColor;
		Vector3 TOD_SunDirection  = LocalSunDirection;
		Vector3 TOD_MoonDirection = LocalMoonDirection;

		Vector3 _OpticalDepth      = this.opticalDepth;
		Vector3 _OneOverBeta       = this.oneOverBeta;
		Vector3 _BetaRayleigh      = this.betaRayleigh;
		Vector3 _BetaRayleighTheta = this.betaRayleighTheta;
		Vector3 _BetaMie           = this.betaMie;
		Vector3 _BetaMieTheta      = this.betaMieTheta;
		Vector3 _BetaMiePhase      = this.betaMiePhase;
		Vector3 _BetaNight         = this.betaNight;

		Color color = Color.black;

		// Angle between sun and normal
		float cosTheta = directLight ? Mathf.Max(0, Vector3.Dot(-direction, TOD_SunDirection)) : 0;

		// Parameter value
		// See [7] page 70 equation (5.7)
		float h = Mathf.Clamp(direction.y + _Horizon, 0.001f, 1);
		float f = Mathf.Pow(h, _Haziness);

		// Optical depth integral approximation
		// See [7] page 71 equation (5.8)
		// See [7] page 71 equation (5.10)
		// See [7] page 76 equation (6.1)
		float sh = (1 - f) * 190000;
		float sr = sh + f * (_OpticalDepth.x - sh);
		float sm = sh + f * (_OpticalDepth.y - sh);

		// Angular dependency
		// See [3] page 2 equation (2) and (4)
		float angular = (1 + cosTheta*cosTheta);

		// Rayleigh and mie scattering factors
		// See [3] page 2 equation (1) and (2)
		// See [3] page 2 equation (3) and (4)
		Vector3 beta = _BetaRayleigh * sr
		             + _BetaMie * sm;
		Vector3 betaTheta = _BetaRayleighTheta
		                  + _BetaMieTheta / Mathf.Pow(_BetaMiePhase.x - _BetaMiePhase.y * cosTheta, 1.5f);

		// Scattering solution
		// See [5] page 11
		float E_sun_r  = TOD_SunColor.r;
		float E_sun_g  = TOD_SunColor.g;
		float E_sun_b  = TOD_SunColor.b;
		float E_moon_r = TOD_MoonColor.r;
		float E_moon_g = TOD_MoonColor.g;
		float E_moon_b = TOD_MoonColor.b;
		float T_val_r  = Mathf.Exp(-beta.x);
		float T_val_g  = Mathf.Exp(-beta.y);
		float T_val_b  = Mathf.Exp(-beta.z);
		float L_sun_r  = angular * betaTheta.x * _OneOverBeta.x;
		float L_sun_g  = angular * betaTheta.y * _OneOverBeta.y;
		float L_sun_b  = angular * betaTheta.z * _OneOverBeta.z;
		float L_moon_r = _BetaNight.x;
		float L_moon_g = _BetaNight.y;
		float L_moon_b = _BetaNight.z;

		// Add scattering color
		color.r = (1-T_val_r) * (E_sun_r*L_sun_r + E_moon_r*L_moon_r);
		color.g = (1-T_val_g) * (E_sun_g*L_sun_g + E_moon_g*L_moon_g);
		color.b = (1-T_val_b) * (E_sun_b*L_sun_b + E_moon_b*L_moon_b);

		// Add simple moon halo
		if (directLight) color += TOD_MoonHaloColor * Mathf.Pow(Mathf.Max(0, Vector3.Dot(TOD_MoonDirection, -direction)), 10);

		// Add additive color
		color += TOD_AdditiveColor;

		// Add fog color
		color.r = Mathf.Lerp(color.r, TOD_CloudColor.r, _Fogginess);
		color.g = Mathf.Lerp(color.g, TOD_CloudColor.g, _Fogginess);
		color.b = Mathf.Lerp(color.b, TOD_CloudColor.b, _Fogginess);

		// Adjust output contrast
		color = TOD_Util.PowRGB(color, _Contrast);

		return color;
	}

	#if !UNITY_3 && !UNITY_4 && !UNITY_5
	/// Render the sky dome to 3rd order spherical harmonics.
	/// \param size Number of samples to do in every direction.
	/// \param exposure Camera exposure, determines brightness.
	/// \param directLight Whether or not to include direct light.
	internal SphericalHarmonics3 RenderToSH3(int size, float exposure = 1, bool directLight = true)
	{
		var sh = new SphericalHarmonics3();

		float length = size / 2;
		float center = length - 0.5f;
		float weight = (exposure * 4) / (size * size * 6);

		for (int y = 0; y < size; y++)
		for (int z = 0; z < size; z++)
		{
			var dir = new Vector3(+length, center - y, center - z).normalized;
			var col = Color.Lerp(SampleAtmosphere(dir, directLight), AmbientColor, -dir.y);
			sh.AddDirectionalLight(-dir, TOD_Util.Linear(col), weight);
		}

		for (int y = 0; y < size; y++)
		for (int z = 0; z < size; z++)
		{
			var dir = new Vector3(-length, center - y, z - center).normalized;
			var col = Color.Lerp(SampleAtmosphere(dir, directLight), AmbientColor, -dir.y);
			sh.AddDirectionalLight(-dir, TOD_Util.Linear(col), weight);
		}

		for (int z = 0; z < size; z++)
		for (int x = 0; x < size; x++)
		{
			var dir = new Vector3(x - center, +length, z - center).normalized;
			var col = Color.Lerp(SampleAtmosphere(dir, directLight), AmbientColor, -dir.y);
			sh.AddDirectionalLight(-dir, TOD_Util.Linear(col), weight);
		}

		for (int z = 0; z < size; z++)
		for (int x = 0; x < size; x++)
		{
			var dir = new Vector3(x - center, -length, center - z).normalized;
			var col = Color.Lerp(SampleAtmosphere(dir, directLight), AmbientColor, -dir.y);
			sh.AddDirectionalLight(-dir, TOD_Util.Linear(col), weight);
		}

		for (int y = 0; y < size; y++)
		for (int x = 0; x < size; x++)
		{
			var dir = new Vector3(x - center, center - y, +length).normalized;
			var col = Color.Lerp(SampleAtmosphere(dir, directLight), AmbientColor, -dir.y);
			sh.AddDirectionalLight(-dir, TOD_Util.Linear(col), weight);
		}

		for (int y = 0; y < size; y++)
		for (int x = 0; x < size; x++)
		{
			var dir = new Vector3(center - x, center - y, -length).normalized;
			var col = Color.Lerp(SampleAtmosphere(dir, directLight), AmbientColor, -dir.y);
			sh.AddDirectionalLight(-dir, TOD_Util.Linear(col), weight);
		}

		return sh;
	}
	#endif

	/// Render the sky dome to a cubemap.
	/// \param size Resolution of the cubemap.
	/// \param faceMask Mask to determine which faces of the cubemap to update.
	/// \param exposure Camera exposure, determines brightness.
	/// \param directLight Whether or not to include direct light.
	/// \return Freshly allocated cubemap with the sky rendered to it.
	internal Cubemap RenderToCubemap(int size, int faceMask = 63, float exposure = 1, bool directLight = true)
	{
		var cubemap = new Cubemap(size, TextureFormat.RGB24, true);
		RenderToCubemap(cubemap, faceMask, exposure, directLight);
		return cubemap;
	}

	/// Render the sky dome to a cubemap.
	/// \param cubemap Cubemap target to render the sky to.
	/// \param faceMask Mask to determine which faces of the cubemap to update.
	/// \param exposure Camera exposure, determines brightness.
	/// \param directLight Whether or not to include direct light.
	internal void RenderToCubemap(Cubemap cubemap, int faceMask = 63, float exposure = 1, bool directLight = true)
	{
		if (faceMask == 0) return;

		int size = cubemap.width;
		float length = size / 2;
		float center = length - 0.5f;

		var colors = new Color[size*size];

		if ((faceMask & (1 << (int)CubemapFace.PositiveX)) != 0)
		{
			for (int y = 0; y < size; y++)
			for (int z = 0; z < size; z++)
			{
				var dir = new Vector3(+length, center - y, center - z).normalized;
				var col = Color.Lerp(SampleAtmosphere(dir, directLight), AmbientColor, -dir.y);
				colors[y * size + z] = TOD_Util.Linear(TOD_Util.ExpRGB(col, exposure));
			}
			cubemap.SetPixels(colors, CubemapFace.PositiveX);
		}

		if ((faceMask & (1 << (int)CubemapFace.NegativeX)) != 0)
		{
			for (int y = 0; y < size; y++)
			for (int z = 0; z < size; z++)
			{
				var dir = new Vector3(-length, center - y, z - center).normalized;
				var col = Color.Lerp(SampleAtmosphere(dir, directLight), AmbientColor, -dir.y);
				colors[y * size + z] = TOD_Util.Linear(TOD_Util.ExpRGB(col, exposure));
			}
			cubemap.SetPixels(colors, CubemapFace.NegativeX);
		}

		if ((faceMask & (1 << (int)CubemapFace.PositiveY)) != 0)
		{
			for (int z = 0; z < size; z++)
			for (int x = 0; x < size; x++)
			{
				var dir = new Vector3(x - center, +length, z - center).normalized;
				var col = Color.Lerp(SampleAtmosphere(dir, directLight), AmbientColor, -dir.y);
				colors[z * size + x] = TOD_Util.Linear(TOD_Util.ExpRGB(col, exposure));
			}
			cubemap.SetPixels(colors, CubemapFace.PositiveY);
		}

		if ((faceMask & (1 << (int)CubemapFace.NegativeY)) != 0)
		{
			for (int z = 0; z < size; z++)
			for (int x = 0; x < size; x++)
			{
				var dir = new Vector3(x - center, -length, center - z).normalized;
				var col = Color.Lerp(SampleAtmosphere(dir, directLight), AmbientColor, -dir.y);
				colors[z * size + x] = TOD_Util.Linear(TOD_Util.ExpRGB(col, exposure));
			}
			cubemap.SetPixels(colors, CubemapFace.NegativeY);
		}

		if ((faceMask & (1 << (int)CubemapFace.PositiveZ)) != 0)
		{
			for (int y = 0; y < size; y++)
			for (int x = 0; x < size; x++)
			{
				var dir = new Vector3(x - center, center - y, +length).normalized;
				var col = Color.Lerp(SampleAtmosphere(dir, directLight), AmbientColor, -dir.y);
				colors[y * size + x] = TOD_Util.Linear(TOD_Util.ExpRGB(col, exposure));
			}
			cubemap.SetPixels(colors, CubemapFace.PositiveZ);
		}

		if ((faceMask & (1 << (int)CubemapFace.NegativeZ)) != 0)
		{
			for (int y = 0; y < size; y++)
			for (int x = 0; x < size; x++)
			{
				var dir = new Vector3(center - x, center - y, -length).normalized;
				var col = Color.Lerp(SampleAtmosphere(dir, directLight), AmbientColor, -dir.y);
				colors[y * size + x] = TOD_Util.Linear(TOD_Util.ExpRGB(col, exposure));
			}
			cubemap.SetPixels(colors, CubemapFace.NegativeZ);
		}

		cubemap.Apply();
	}

	/// Calculate the fog color.
	/// \param directLight Whether or not to include direct light.
	internal Color SampleFogColor(bool directLight = true)
	{
		var camera = Camera.main != null ? Camera.main.transform.forward : Vector3.forward;
		var sample = Vector3.Lerp(new Vector3(camera.x, 0, camera.z), Vector3.up, Fog.HeightBias);
		var color  = SampleAtmosphere(sample.normalized, directLight);
		return new Color(Mathf.Clamp01(color.r), Mathf.Clamp01(color.g), Mathf.Clamp01(color.b), 1);
	}

	/// Calculate the sky color.
	/// \param exposure Camera exposure, determines brightness.
	internal Color SampleSkyColor(float exposure = 1)
	{
		var sample = SunDirection; sample.y = Mathf.Abs(sample.y);
		var color  = SampleAtmosphere(sample.normalized, false);
		return TOD_Util.ExpRGB(new Color(color.r, color.g, color.b, 1), exposure);
	}

	/// Calculate the equator color.
	/// \param exposure Camera exposure, determines brightness.
	internal Color SampleEquatorColor(float exposure = 1)
	{
		var sample = SunDirection; sample.y = 0;
		var color  = SampleAtmosphere(sample.normalized, false);
		return TOD_Util.ExpRGB(new Color(color.r, color.g, color.b, 1), exposure);
	}

	/// Update the RenderSettings fog color according to TOD_FogParameters.
	internal void UpdateFog()
	{
		switch (Fog.Mode)
		{
			case TOD_FogType.None:
				break;
			case TOD_FogType.Color:
				var fogColor = SampleFogColor(Fog.Directional);
				#if UNITY_EDITOR
				if (RenderSettings.fogColor != fogColor)
				#endif
				{
					RenderSettings.fogColor = fogColor;
				}
				break;
		}
	}

	/// Update the RenderSettings ambient light according to TOD_AmbientParameters.
	internal void UpdateAmbient()
	{
		var exposure = Mathf.Lerp(Night.AmbientMultiplier, Day.AmbientMultiplier, LerpValue);
		var color = TOD_Util.ExpRGB(AmbientColor, exposure);

		switch (Ambient.Mode)
		{
			case TOD_AmbientType.None:
				break;
			case TOD_AmbientType.Flat:
				#if UNITY_3 || UNITY_4
				{
					#if UNITY_EDITOR
					if (RenderSettings.ambientLight != color)
					#endif
					{
						RenderSettings.ambientLight = color;
					}
				}
				#else
				{
					#if UNITY_EDITOR
					if (RenderSettings.ambientMode != AmbientMode.Flat)
					#endif
					{
						RenderSettings.ambientMode = AmbientMode.Flat;
					}

					#if UNITY_EDITOR
					if (RenderSettings.ambientLight != color)
					#endif
					{
						RenderSettings.ambientLight = color;
					}
				}
				#endif
				break;
			case TOD_AmbientType.Trilight:
				#if UNITY_3 || UNITY_4
				{
					Debug.LogWarning("Ambient.Mode." + Ambient.Mode.ToString() + " is only supported in Unity 5 or later.");
				}
				#else
				{
					#if UNITY_EDITOR
					if (RenderSettings.ambientMode != AmbientMode.Trilight)
					#endif
					{
						RenderSettings.ambientMode = AmbientMode.Trilight;
					}

					var skyColor = SampleSkyColor(exposure);
					var equatorColor = SampleEquatorColor(exposure);

					#if UNITY_EDITOR
					if (RenderSettings.ambientSkyColor != skyColor)
					#endif
					{
						RenderSettings.ambientSkyColor = skyColor;
					}

					#if UNITY_EDITOR
					if (RenderSettings.ambientEquatorColor != equatorColor)
					#endif
					{
						RenderSettings.ambientEquatorColor = equatorColor;
					}

					#if UNITY_EDITOR
					if (RenderSettings.ambientGroundColor != color)
					#endif
					{
						RenderSettings.ambientGroundColor = color;
					}
				}
				#endif
				break;
			case TOD_AmbientType.Spherical:
			#if UNITY_3 || UNITY_4 || UNITY_5
				{
					Debug.LogWarning("Ambient.Mode." + Ambient.Mode.ToString() + " is only supported in Unity 5 or later.");
				}
				#else
				{
					#if UNITY_EDITOR
					if (RenderSettings.ambientMode != AmbientMode.Skybox)
					#endif
					{
						RenderSettings.ambientMode = AmbientMode.Skybox;
					}

					RenderSettings.ambientProbe = RenderToSH3(Ambient.Resolution, exposure, Ambient.Directional);

					#if UNITY_EDITOR
					if (RenderSettings.ambientLight != color)
					#endif
					{
						RenderSettings.ambientLight = color;
					}
				}
				#endif
				break;
		}
	}

	/// Update the RenderSettings reflection probe according to TOD_ReflectionParameters.
	internal void UpdateReflection()
	{
		switch (Reflection.Mode)
		{
			case TOD_ReflectionType.None:
				break;
			case TOD_ReflectionType.Cubemap:
			#if UNITY_3 || UNITY_4 || UNITY_5
				{
					Debug.LogWarning("Reflection.Mode." + Reflection.Mode.ToString() + " is only supported in Unity 5 or later.");
				}
				#else
				{
					#if UNITY_EDITOR
					if (RenderSettings.defaultReflectionMode != DefaultReflectionMode.Custom)
					#endif
					{
						RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
					}

					var exposure = Mathf.Lerp(Night.ReflectionMultiplier, Day.ReflectionMultiplier, LerpValue);

					if (RenderSettings.customReflection == null || RenderSettings.customReflection.width != Reflection.Resolution)
					{
						RenderSettings.customReflection = RenderToCubemap(Reflection.Resolution, 63, exposure, Reflection.Directional);
					}
					else
					{
						RenderToCubemap(RenderSettings.customReflection, 63, exposure, Reflection.Directional);
					}
				}
				#endif
				break;
		}
	}

	// Apply fake HDR to LDR conversion.
	/// \param color The HDR color.
	/// \return The LDR color.
	internal Color FakeHDR2LDR(Color color)
	{
		float exposure = Mathf.Lerp(1.0f, 0.1f, Mathf.Sqrt(SunZenith / HorizonAngle) - 0.25f);
		return TOD_Util.ExpRGB(color, exposure);
	}

	// Apply fake HDR to LDR conversion.
	/// \param color The HDR color.
	/// \param factor The amount of fake HDR to apply.
	/// \return The LDR color.
	internal Color FakeHDR2LDR(Color color, float factor)
	{
		float exposure = Mathf.Lerp(1.0f, Mathf.Lerp(1.0f, 0.1f, Mathf.Sqrt(SunZenith / HorizonAngle) - 0.25f), factor);
		return TOD_Util.ExpRGB(color, exposure);
	}

	// Apply fake HDR to LDR conversion.
	/// \param vector The HDR vector.
	/// \return The LDR vector.
	internal Vector3 FakeHDR2LDR(Vector3 vector)
	{
		float exposure = Mathf.Lerp(1.0f, 0.1f, Mathf.Sqrt(SunZenith / HorizonAngle) - 0.25f);
		return vector * exposure;
	}

	// Apply fake HDR to LDR conversion.
	/// \param vector The HDR vector.
	/// \param factor The amount of fake HDR to apply.
	/// \return The LDR vector.
	internal Vector3 FakeHDR2LDR(Vector3 vector, float factor)
	{
		float exposure = Mathf.Lerp(1.0f, Mathf.Lerp(1.0f, 0.1f, Mathf.Sqrt(SunZenith / HorizonAngle) - 0.25f), factor);
		return vector * exposure;
	}
}
