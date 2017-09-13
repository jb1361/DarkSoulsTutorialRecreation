using UnityEngine;

/// Moon position types.
public enum TOD_MoonPositionType
{
	OppositeToSun,
	Realistic
}

/// Stars position types.
public enum TOD_StarsPositionType
{
	Static,
	Rotating
}

/// Horizon type.
public enum TOD_HorizonType
{
	Static,
	ZeroLevel
}

/// Fog adjustment types.
public enum TOD_FogType
{
	None,
	Color
}

/// Ambient light types.
public enum TOD_AmbientType
{
	None,
	Flat,
	Trilight,
	Spherical
}

/// Reflection cubemap types.
public enum TOD_ReflectionType
{
	None,
	Cubemap
}

/// Unity color space detection.
public enum TOD_ColorSpaceDetection
{
	Auto,
	Linear,
	Gamma
}

/// Cloud rendering qualities.
public enum TOD_CloudQualityType
{
	Fastest,
	Density,
	Bumped
}

/// Mesh vertex count levels.
public enum TOD_MeshQualityType
{
	Low,
	Medium,
	High
}

/// Cloud coverage types.
public enum TOD_CloudType
{
	Custom,
	None,
	Few,
	Scattered,
	Broken,
	Overcast
}

/// Weather types.
public enum TOD_WeatherType
{
	Custom,
	Clear,
	Storm,
	Dust,
	Fog
}
