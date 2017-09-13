using UnityEngine;

/// Main sky dome management class.
///
/// Component of the sky dome parent game object.

[ExecuteInEditMode]
[RequireComponent(typeof(TOD_Resources))]
[RequireComponent(typeof(TOD_Components))]
public partial class TOD_Sky : MonoBehaviour
{
	private const float pi  = Mathf.PI;
	private const float tau = Mathf.PI * 2.0f;

	private Vector2 opticalDepth;
	private Vector3 oneOverBeta;
	private Vector3 betaRayleigh;
	private Vector3 betaRayleighTheta;
	private Vector3 betaMie;
	private Vector3 betaMieTheta;
	private Vector2 betaMiePhase;
	private Vector3 betaNight;

	private float timeSinceLightUpdate = float.MaxValue;

	/// Setup rayleigh and mie scattering by precalculating as much of it as possible.
	// See [2] page 2
	// See [3] page 2
	private void SetupScattering()
	{
		// Scale
		const float ray_scale_const = 1.0f;
		const float ray_scale_theta = 20.0f;
		const float mie_scale_const = 0.1f;
		const float mie_scale_theta = 2.0f;

		// Rayleigh
		{
			// Artistic color multiplier
			float mult_r = 0.001f + Atmosphere.RayleighMultiplier * Atmosphere.ScatteringColor.r;
			float mult_g = 0.001f + Atmosphere.RayleighMultiplier * Atmosphere.ScatteringColor.g;
			float mult_b = 0.001f + Atmosphere.RayleighMultiplier * Atmosphere.ScatteringColor.b;

			// Scattering coefficient
			const float beta_r = 5.8e-6f;
			const float beta_g = 13.5e-6f;
			const float beta_b = 33.1e-6f;

			// Phase function
			const float phase = (3)/(16*pi);

			// Shader paramters
			betaRayleigh.x = ray_scale_const * beta_r * mult_r;
			betaRayleigh.y = ray_scale_const * beta_g * mult_g;
			betaRayleigh.z = ray_scale_const * beta_b * mult_b;
			betaRayleighTheta.x = ray_scale_theta * beta_r * mult_r * phase;
			betaRayleighTheta.y = ray_scale_theta * beta_g * mult_g * phase;
			betaRayleighTheta.z = ray_scale_theta * beta_b * mult_b * phase;
			opticalDepth.x = 8000; // * Mathf.Exp(-height * 50000 / 8000);
		}

		// Mie
		{
			// Artistic color multiplier
			float mult_r = 0.001f + Atmosphere.MieMultiplier * Atmosphere.ScatteringColor.r;
			float mult_g = 0.001f + Atmosphere.MieMultiplier * Atmosphere.ScatteringColor.g;
			float mult_b = 0.001f + Atmosphere.MieMultiplier * Atmosphere.ScatteringColor.b;

			// Scattering coefficient
			const float beta = 2e-5f;

			// Phase function
			float g = Atmosphere.Directionality;
			float phase = (3)/(4*pi) * (1-g*g)/(2+g*g);

			// Shader paramters
			betaMie.x = mie_scale_const * beta * mult_r;
			betaMie.y = mie_scale_const * beta * mult_g;
			betaMie.z = mie_scale_const * beta * mult_b;
			betaMieTheta.x = mie_scale_theta * beta * mult_r * phase;
			betaMieTheta.y = mie_scale_theta * beta * mult_g * phase;
			betaMieTheta.z = mie_scale_theta * beta * mult_b * phase;
			betaMiePhase.x = 1+g*g;
			betaMiePhase.y = 2*g;
			opticalDepth.y = 1200; // * Mathf.Exp(-height * 50000 / 1200);
		}

		oneOverBeta = TOD_Util.Inverse(betaMie + betaRayleigh);
		betaNight   = Vector3.Scale(betaRayleighTheta + betaMieTheta / Mathf.Pow(betaMiePhase.x, 1.5f), oneOverBeta);
		oneOverBeta = FakeHDR2LDR(oneOverBeta, Atmosphere.FakeHDR);
	}

	/// Calculate sun and moon position.
	private void SetupSunAndMoon()
	{
		// Local latitude
		float lat_rad = Mathf.Deg2Rad * World.Latitude;
		float lat_sin = Mathf.Sin(lat_rad);
		float lat_cos = Mathf.Cos(lat_rad);

		// Local longitude
		float lon_deg = World.Longitude;

		// Horizon angle
		float horizon_rad = HorizonAngle * Mathf.Deg2Rad;

		// Local time
		float d = 367*Cycle.Year - 7*(Cycle.Year + (Cycle.Month+9)/12)/4 + 275*Cycle.Month/9 + Cycle.Day - 730530;
		float t = Cycle.Hour - World.UTC;
		float ecl = 23.4393f - 3.563E-7f * d;
		float ecl_rad = Mathf.Deg2Rad * ecl;
		float ecl_sin = Mathf.Sin(ecl_rad);
		float ecl_cos = Mathf.Cos(ecl_rad);

		// Sun
		float sun_theta, sun_phi;
		{
			// See http://www.stjarnhimlen.se/comp/ppcomp.html#4

			float w = 282.9404f + 4.70935E-5f * d;
			float e = 0.016709f - 1.151E-9f * d;
			float M = 356.0470f + 0.9856002585f * d;

			float M_rad = Mathf.Deg2Rad * M;
			float M_sin = Mathf.Sin(M_rad);
			float M_cos = Mathf.Cos(M_rad);

			// See http://www.stjarnhimlen.se/comp/ppcomp.html#5

			float E_deg = M + e * Mathf.Rad2Deg * M_sin * (1 + e * M_cos);
			float E_rad = Mathf.Deg2Rad * E_deg;
			float E_sin = Mathf.Sin(E_rad);
			float E_cos = Mathf.Cos(E_rad);

			float xv = E_cos - e;
			float yv = E_sin * Mathf.Sqrt(1 - e*e);

			float v = Mathf.Rad2Deg * Mathf.Atan2(yv, xv);
			float r = Mathf.Sqrt(xv*xv + yv*yv);

			float l = v + w;
			float l_rad = Mathf.Deg2Rad * l;
			float l_sin = Mathf.Sin(l_rad);
			float l_cos = Mathf.Cos(l_rad);

			float xs = r * l_cos;
			float ys = r * l_sin;

			float xe = xs;
			float ye = ys * ecl_cos;
			float ze = ys * ecl_sin;

			float rasc_rad = Mathf.Atan2(ye, xe);
			float rasc_deg = Mathf.Rad2Deg * rasc_rad;
			float decl_rad = Mathf.Atan2(ze, Mathf.Sqrt(xe*xe + ye*ye));
			float decl_sin = Mathf.Sin(decl_rad);
			float decl_cos = Mathf.Cos(decl_rad);

			// See http://www.stjarnhimlen.se/comp/ppcomp.html#5b

			float GMST0_deg = v + w + 180;
			float GMST_deg  = GMST0_deg + t*15;
			float LST_deg   = GMST_deg + lon_deg;

			// See http://www.stjarnhimlen.se/comp/ppcomp.html#12b

			float HA_deg = LST_deg - rasc_deg;
			float HA_rad = Mathf.Deg2Rad * HA_deg;
			float HA_sin = Mathf.Sin(HA_rad);
			float HA_cos = Mathf.Cos(HA_rad);

			float x = HA_cos * decl_cos;
			float y = HA_sin * decl_cos;
			float z = decl_sin;

			float xhor = x * lat_sin - z * lat_cos;
			float yhor = y;
			float zhor = x * lat_cos + z * lat_sin;

			float azimuth  = Mathf.Atan2(yhor, xhor) + Mathf.Deg2Rad * 180;
			float altitude = Mathf.Atan2(zhor, Mathf.Sqrt(xhor*xhor + yhor*yhor));

			sun_theta = horizon_rad - altitude;
			sun_phi   = azimuth;
		}

		// Moon
		float moon_theta, moon_phi;
		if (Moon.Position == TOD_MoonPositionType.Realistic)
		{
			float N = 125.1228f - 0.0529538083f * d;
			float i = 5.1454f;
			float w = 318.0634f + 0.1643573223f * d;
			float a = 60.2666f;
			float e = 0.054900f;
			float M = 115.3654f + 13.0649929509f * d;

			float N_rad = Mathf.Deg2Rad * N;
			float N_sin = Mathf.Sin(N_rad);
			float N_cos = Mathf.Cos(N_rad);

			float i_rad = Mathf.Deg2Rad * i;
			float i_sin = Mathf.Sin(i_rad);
			float i_cos = Mathf.Cos(i_rad);

			float M_rad = Mathf.Deg2Rad * M;
			float M_sin = Mathf.Sin(M_rad);
			float M_cos = Mathf.Cos(M_rad);

			// See http://www.stjarnhimlen.se/comp/ppcomp.html#6

			float E_deg = M + e * Mathf.Rad2Deg * M_sin * (1 + e * M_cos);
			float E_rad = Mathf.Deg2Rad * E_deg;
			float E_sin = Mathf.Sin(E_rad);
			float E_cos = Mathf.Cos(E_rad);

			float xv = a * (E_cos - e);
			float yv = a * (E_sin * Mathf.Sqrt(1 - e*e));

			float v = Mathf.Rad2Deg * Mathf.Atan2(yv, xv);
			float r = Mathf.Sqrt(xv*xv + yv*yv);

			float l = v + w;
			float l_rad = Mathf.Deg2Rad * l;
			float l_sin = Mathf.Sin(l_rad);
			float l_cos = Mathf.Cos(l_rad);

			// See http://www.stjarnhimlen.se/comp/ppcomp.html#7

			float xh = r * (N_cos * l_cos - N_sin * l_sin * i_cos);
			float yh = r * (N_sin * l_cos + N_cos * l_sin * i_cos);
			float zh = r * (l_sin * i_sin);

			// See http://www.stjarnhimlen.se/comp/ppcomp.html#12

			float xe = xh;
			float ye = yh * ecl_cos - zh * ecl_sin;
			float ze = zh * ecl_sin + zh * ecl_cos;

			float rasc_rad = Mathf.Atan2(ye, xe);
			float rasc_deg = Mathf.Rad2Deg * rasc_rad;
			float decl_rad = Mathf.Atan2(ze, Mathf.Sqrt(xe*xe + ye*ye));
			float decl_sin = Mathf.Sin(decl_rad);
			float decl_cos = Mathf.Cos(decl_rad);

			// See http://www.stjarnhimlen.se/comp/ppcomp.html#5b

			float GMST0_deg = v + w + 180;
			float GMST_deg  = GMST0_deg + t*15;
			float LST_deg   = GMST_deg + lon_deg;

			// See http://www.stjarnhimlen.se/comp/ppcomp.html#12b

			float HA_deg = LST_deg - rasc_deg;
			float HA_rad = Mathf.Deg2Rad * HA_deg;
			float HA_sin = Mathf.Sin(HA_rad);
			float HA_cos = Mathf.Cos(HA_rad);

			float x = HA_cos * decl_cos;
			float y = HA_sin * decl_cos;
			float z = decl_sin;

			float xhor = x * lat_sin - z * lat_cos;
			float yhor = y;
			float zhor = x * lat_cos + z * lat_sin;

			float azimuth  = Mathf.Atan2(yhor, xhor) + Mathf.Deg2Rad * 180;
			float altitude = Mathf.Atan2(zhor, Mathf.Sqrt(xhor*xhor + yhor*yhor));

			moon_theta = horizon_rad - altitude;
			moon_phi   = azimuth;
		}
		else
		{
			moon_theta = sun_theta - pi;
			moon_phi   = sun_phi;
		}

		// Update space position
		Quaternion spaceRot = Quaternion.Euler(90 - World.Latitude, 0, 0) * Quaternion.Euler(0, World.Longitude, 0) * Quaternion.Euler(0, (Cycle.Hour / 24f) * 360f, 0);
		if (Stars.Position == TOD_StarsPositionType.Rotating)
		{
			#if UNITY_EDITOR
			if (Components.SpaceTransform.localRotation.eulerAngles != spaceRot.eulerAngles)
			#endif
			{
				Components.SpaceTransform.localRotation = spaceRot;
			}
		}
		else
		{
			#if UNITY_EDITOR
			if (Components.SpaceTransform.localRotation.eulerAngles != Quaternion.identity.eulerAngles)
			#endif
			{
				Components.SpaceTransform.localRotation = Quaternion.identity;
			}
		}

		// Update sun position
		var sunPos = OrbitalToLocal(sun_theta, sun_phi);
		#if UNITY_EDITOR
		if (Components.SunTransform.localPosition != sunPos)
		#endif
		{
			Components.SunTransform.localPosition = sunPos;
			Components.SunTransform.LookAt(Components.DomeTransform.position, Components.SunTransform.up);

			// Add camera-based sun animation rotation
			if (Camera.main)
			{
				Vector3 camRot = Camera.main.transform.rotation.eulerAngles;
				Vector3 sunRot = Components.SunTransform.localEulerAngles;
				sunRot.z = 2 * Time.time + Mathf.Abs(camRot.x) + Mathf.Abs(camRot.y) + Mathf.Abs(camRot.z);
				Components.SunTransform.localEulerAngles = sunRot;
			}
		}

		// Update moon position
		var moonPos = OrbitalToLocal(moon_theta, moon_phi);
		#if UNITY_EDITOR
		if (Components.MoonTransform.localPosition != moonPos)
		#endif
		{
			var moonFwd = spaceRot * -Vector3.right;
			Components.MoonTransform.localPosition = moonPos;
			Components.MoonTransform.LookAt(Components.DomeTransform.position, moonFwd);
		}

		// Setup sun size - additional factor of two because it is a quad
		float sun_r = 4 * Mathf.Tan(Mathf.Deg2Rad / 2 * Sun.MeshSize);
		float sun_d = 2 * sun_r;
		var sunScale = new Vector3(sun_d, sun_d, sun_d);
		#if UNITY_EDITOR
		if (Components.SunTransform.localScale != sunScale)
		#endif
		{
			Components.SunTransform.localScale = sunScale;
		}

		// Setup moon size
		float moon_r = 2 * Mathf.Tan(Mathf.Deg2Rad / 2 * Moon.MeshSize);
		float moon_d = 2 * moon_r;
		var moonScale = new Vector3(moon_d, moon_d, moon_d);
		#if UNITY_EDITOR
		if (Components.MoonTransform.localScale != moonScale)
		#endif
		{
			Components.MoonTransform.localScale = moonScale;
		}

		// Update properties
		SunZenith  = Mathf.Rad2Deg * sun_theta;
		MoonZenith = Mathf.Rad2Deg * moon_theta;

		// Update renderer states
		var projEnabled  = (Components.ShadowShader != null && Clouds.ShadowStrength != 0);
		var clearEnabled = (Components.Rays != null && Components.Rays.enabled);
		var sunEnabled   = (Components.SunTransform.localPosition.y  > -(HorizonOffset + sun_d));
		var moonEnabled  = (Components.MoonTransform.localPosition.y > -(HorizonOffset + moon_d));
		var spaceEnabled = true;
		var atmoEnabled  = true;
		var cloudEnabled = (Clouds.Density > 0);

		#if UNITY_EDITOR
		if (Components.ShadowProjector.enabled != projEnabled)
		#endif
		{
			Components.ShadowProjector.enabled = projEnabled;
		}

		#if UNITY_EDITOR
		if (Components.ClearRenderer.enabled != clearEnabled)
		#endif
		{
			Components.ClearRenderer.enabled = clearEnabled;
		}

		#if UNITY_EDITOR
		if (Components.SunRenderer.enabled != sunEnabled)
		#endif
		{
			Components.SunRenderer.enabled = sunEnabled;
		}

		#if UNITY_EDITOR
		if (Components.MoonRenderer.enabled != moonEnabled)
		#endif
		{
			Components.MoonRenderer.enabled  = moonEnabled;
		}

		#if UNITY_EDITOR
		if (Components.SpaceRenderer.enabled != spaceEnabled)
		#endif
		{
			Components.SpaceRenderer.enabled = spaceEnabled;
		}

		#if UNITY_EDITOR
		if (Components.AtmosphereRenderer.enabled != atmoEnabled)
		#endif
		{
			Components.AtmosphereRenderer.enabled = atmoEnabled;
		}

		#if UNITY_EDITOR
		if (Components.CloudRenderer.enabled != cloudEnabled)
		#endif
		{
			Components.CloudRenderer.enabled = cloudEnabled;
		}

		// Update light source
		SetupLightSource(sun_theta, sun_phi, moon_theta, moon_phi);
	}

	/// Update light source color, intensity and position.
	private void SetupLightSource(float sun_theta, float sun_phi, float moon_theta, float moon_phi)
	{
		float theta_norm = sun_theta / (HorizonAngle * Mathf.Deg2Rad);
		float adjusted_theta = Mathf.Pow(theta_norm * 0.25f, 2f - Light.Falloff) * tau;

		// Relative optical mass (air mass coefficient approximated by a spherical shell)
		// See http://en.wikipedia.org/wiki/Air_mass_(solar_energy)
		float c = Mathf.Cos(adjusted_theta);
		float m = Mathf.Sqrt(708f*708f*c*c + 2*708f + 1) - 708f*c;

		// Wavelengths in micrometers
		// See [3] page 2
		const float lambda_r = 680.0e-3f; // [um]
		const float lambda_g = 550.0e-3f; // [um]
		const float lambda_b = 440.0e-3f; // [um]

		// Transmitted sun color
		float r = Sun.LightColor.r;
		float g = Sun.LightColor.g;
		float b = Sun.LightColor.b;

		// Transmittance due to Rayleigh scattering of air molecules
		// See [1] page 21
		const float rayleigh_beta  = 0.008735f;
		const float rayleigh_alpha = 4.08f;
		r *= Mathf.Exp(-rayleigh_beta * Mathf.Pow(lambda_r, -rayleigh_alpha * m));
		g *= Mathf.Exp(-rayleigh_beta * Mathf.Pow(lambda_g, -rayleigh_alpha * m));
		b *= Mathf.Exp(-rayleigh_beta * Mathf.Pow(lambda_b, -rayleigh_alpha * m));

		// Angstrom's turbididty formula for aerosal (does not improve anything visually)
		// See [1] page 21
		// const float aerosol_turbidity = 1.0f;
		// const float aerosal_beta = 0.04608f * aerosol_turbidity - 0.04586f;
		// const float aerosal_alpha = 1.3f;
		// r *= Mathf.Exp(-aerosal_beta * Mathf.Pow(lambda_r, -aerosal_alpha * m));
		// g *= Mathf.Exp(-aerosal_beta * Mathf.Pow(lambda_g, -aerosal_alpha * m));
		// b *= Mathf.Exp(-aerosal_beta * Mathf.Pow(lambda_b, -aerosal_alpha * m));

		// Transmittance due to ozone absorption (does not improve anything visually)
		// See [1] page 21
		// const float ozone_l  = 0.350f; // [cm]
		// const float ozone_kr = 0.067f; // [1/cm]
		// const float ozone_kg = 0.040f; // [1/cm]
		// const float ozone_kb = 0.009f; // [1/cm]
		// r *= Mathf.Exp(-ozone_kr * Mathf.Pow(lambda_r, -ozone_l * m));
		// g *= Mathf.Exp(-ozone_kg * Mathf.Pow(lambda_g, -ozone_l * m));
		// b *= Mathf.Exp(-ozone_kb * Mathf.Pow(lambda_b, -ozone_l * m));

		// Lerp value
		LerpValue = Mathf.Clamp01(1.2f * new Color(r, g, b).grayscale);

		// Light components
		float light_moon_r = Moon.LightColor.r * Moon.LightColor.a;
		float light_moon_g = Moon.LightColor.g * Moon.LightColor.a;
		float light_moon_b = Moon.LightColor.b * Moon.LightColor.a;
		float light_sun_r  = Sun.LightColor.r * Sun.LightColor.a * Mathf.Lerp(1, r, Light.Coloring);
		float light_sun_g  = Sun.LightColor.g * Sun.LightColor.a * Mathf.Lerp(1, g, Light.Coloring);
		float light_sun_b  = Sun.LightColor.b * Sun.LightColor.a * Mathf.Lerp(1, b, Light.Coloring);

		// Sky components
		float sky_night_r = Moon.LightColor.r * Moon.LightColor.a;
		float sky_night_g = Moon.LightColor.g * Moon.LightColor.a;
		float sky_night_b = Moon.LightColor.b * Moon.LightColor.a;
		float sky_day_r   = Sun.LightColor.r * Sun.LightColor.a * Mathf.Lerp(1, r, Light.SkyColoring);
		float sky_day_g   = Sun.LightColor.g * Sun.LightColor.a * Mathf.Lerp(1, g, Light.SkyColoring);
		float sky_day_b   = Sun.LightColor.b * Sun.LightColor.a * Mathf.Lerp(1, b, Light.SkyColoring);

		// Ray components
		float ray_moon_r = Moon.RayColor.r * Moon.RayColor.a;
		float ray_moon_g = Moon.RayColor.g * Moon.RayColor.a;
		float ray_moon_b = Moon.RayColor.b * Moon.RayColor.a;
		float ray_sun_r  = Sun.RayColor.r * Sun.RayColor.a * Mathf.Lerp(1, r, Light.RayColoring);
		float ray_sun_g  = Sun.RayColor.g * Sun.RayColor.a * Mathf.Lerp(1, g, Light.RayColoring);
		float ray_sun_b  = Sun.RayColor.b * Sun.RayColor.a * Mathf.Lerp(1, b, Light.RayColoring);

		// Ambient components
		float amb_night_r = Night.AmbientColor.r * Night.AmbientColor.a;
		float amb_night_g = Night.AmbientColor.g * Night.AmbientColor.a;
		float amb_night_b = Night.AmbientColor.b * Night.AmbientColor.a;
		float amb_day_r   = Day.AmbientColor.r * Day.AmbientColor.a * Mathf.Lerp(1, r, Light.AmbientColoring);
		float amb_day_g   = Day.AmbientColor.g * Day.AmbientColor.a * Mathf.Lerp(1, g, Light.AmbientColoring);
		float amb_day_b   = Day.AmbientColor.b * Day.AmbientColor.a * Mathf.Lerp(1, b, Light.AmbientColoring);

		// Cloud components
		float cloud_night_r = Night.CloudColor.r * Night.CloudColor.a;
		float cloud_night_g = Night.CloudColor.g * Night.CloudColor.a;
		float cloud_night_b = Night.CloudColor.b * Night.CloudColor.a;
		float cloud_day_r   = Day.CloudColor.r * Day.CloudColor.a * Mathf.Lerp(1, r, Light.CloudColoring);
		float cloud_day_g   = Day.CloudColor.g * Day.CloudColor.a * Mathf.Lerp(1, g, Light.CloudColoring);
		float cloud_day_b   = Day.CloudColor.b * Day.CloudColor.a * Mathf.Lerp(1, b, Light.CloudColoring);

		// Sky colors
		float sky_night_mult = Atmosphere.Brightness * Night.SkyMultiplier * (1-LerpValue);
		float sky_day_mult   = Atmosphere.Brightness * Day.SkyMultiplier   * LerpValue;
		MoonColor = new Color(sky_night_r * sky_night_mult, sky_night_g * sky_night_mult, sky_night_b * sky_night_mult, 1-LerpValue);
		SunColor  = new Color(sky_day_r   * sky_day_mult,   sky_day_g   * sky_day_mult,   sky_day_b   * sky_day_mult,   LerpValue);

		// Cloud colors
		float cloud_night_mult = Clouds.Brightness * Night.CloudMultiplier * (1-LerpValue);
		float cloud_day_mult   = Clouds.Brightness * Day.CloudMultiplier   * LerpValue;
		Color cloud_night      = new Color(cloud_night_r * cloud_night_mult, cloud_night_g * cloud_night_mult, cloud_night_b * cloud_night_mult, 1);
		Color cloud_day        = new Color(cloud_day_r   * cloud_day_mult,   cloud_day_g   * cloud_day_mult,   cloud_day_b   * cloud_day_mult,   1);
		CloudColor = Color.Lerp(cloud_night, cloud_day, LerpValue);

		// Additive colors
		Color additiveColor = Color.Lerp(Night.AdditiveColor, Day.AdditiveColor, LerpValue);
		additiveColor.r *= additiveColor.a;
		additiveColor.g *= additiveColor.a;
		additiveColor.b *= additiveColor.a;
		additiveColor.a = additiveColor.grayscale;
		AdditiveColor = additiveColor;

		// Ambient colors
		Color amb_night = new Color(amb_night_r, amb_night_g, amb_night_b, 1);
		Color amb_day   = new Color(amb_day_r,   amb_day_g,   amb_day_b,   1);
		AmbientColor = Color.Lerp(amb_night, amb_day, LerpValue);

		// Light source and ray colors
		Color light_moon = new Color(light_moon_r, light_moon_g, light_moon_b, 1);
		Color light_sun  = new Color(light_sun_r,  light_sun_g,  light_sun_b,  1);
		Color ray_moon = new Color(ray_moon_r, ray_moon_g, ray_moon_b, 1);
		Color ray_sun  = new Color(ray_sun_r,  ray_sun_g,  ray_sun_b,  1);

		// Light source parameters
		const float lerp_threshold = 0.1f;
		const float falloff_angle  = 5f;

		float intensity, shadowStrength;
		Vector3 position;

		if (LerpValue > lerp_threshold)
		{
			IsDay = true; IsNight = false;

			float lerp = (LerpValue - lerp_threshold) / (1 - lerp_threshold);

			intensity      = Mathf.Lerp(0, Sun.LightIntensity, lerp);
			shadowStrength = Sun.ShadowStrength;
			position       = OrbitalToLocal(Mathf.Min(sun_theta, (1 - Light.MinimumHeight) * pi/2), sun_phi);

			// God ray color
			RayColor = ray_sun * intensity;

			// Moon halo color
			MoonHaloColor = new Color(0, 0, 0, 0);

			#if UNITY_EDITOR
			if (Components.LightSource.color != light_sun)
			#endif
			{
				Components.LightSource.color = light_sun;
			}
		}
		else
		{
			IsDay = false; IsNight = true;

			float hvalue = Mathf.Clamp01((HorizonAngle - moon_theta * Mathf.Rad2Deg) / falloff_angle);
			float lvalue = (lerp_threshold - LerpValue) / lerp_threshold;

			float lerp  = Mathf.Min(hvalue, lvalue);
			float phase = 1 - Mathf.Abs(Moon.Phase);

			intensity      = Mathf.Lerp(0, Moon.LightIntensity * phase, lerp);
			shadowStrength = Moon.ShadowStrength;
			position       = OrbitalToLocal(Mathf.Min(moon_theta, (1 - Light.MinimumHeight) * pi/2), moon_phi);

			// God ray color
			RayColor = ray_moon * intensity;

			// Moon halo color
			float haloMult = phase * lerp * Atmosphere.Brightness;
			float haloColor_r = haloMult * Moon.HaloColor.a * Moon.HaloColor.r;
			float haloColor_g = haloMult * Moon.HaloColor.a * Moon.HaloColor.g;
			float haloColor_b = haloMult * Moon.HaloColor.a * Moon.HaloColor.b;
			MoonHaloColor = new Color(haloColor_r, haloColor_g, haloColor_b, haloMult);

			#if UNITY_EDITOR
			if (Components.LightSource.color != light_moon)
			#endif
			{
				Components.LightSource.color = light_moon;
			}
		}

		#if UNITY_EDITOR
		if (Components.LightSource.intensity != intensity)
		#endif
		{
			Components.LightSource.intensity = intensity;
		}

		#if UNITY_EDITOR
		if (Components.LightSource.shadowStrength != shadowStrength)
		#endif
		{
			Components.LightSource.shadowStrength = shadowStrength;
		}

		if (!Application.isPlaying || timeSinceLightUpdate >= Light.UpdateInterval)
		{
			timeSinceLightUpdate = 0;

			#if UNITY_EDITOR
			if (Components.LightTransform.localPosition != position)
			#endif
			{
				Components.LightTransform.localPosition = position;
				Components.LightTransform.LookAt(Components.DomeTransform.position);
			}
		}
		else
		{
			timeSinceLightUpdate += Time.deltaTime;
		}

		// Direction vectors
		SunDirection = Components.SunTransform.forward;
		LocalSunDirection = Components.DomeTransform.InverseTransformDirection(SunDirection);
		MoonDirection = Components.MoonTransform.forward;
		LocalMoonDirection = Components.DomeTransform.InverseTransformDirection(MoonDirection);
		LightDirection = Vector3.Lerp(MoonDirection, SunDirection, LerpValue * LerpValue);
		LocalLightDirection = Components.DomeTransform.InverseTransformDirection(LightDirection);
	}
}
