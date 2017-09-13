using UnityEngine;
using System;

/// Time iteration class.
///
/// Component of the sky dome parent game object.

public class TOD_Time : MonoBehaviour
{
	/// Day length inspector variable.
	/// Length of one day in minutes.
	public float DayLengthInMinutes = 30;

	/// Time progression inspector variable.
	/// Automatically updates Cycle.Hour if enabled.
	public bool ProgressTime = true;

	/// Date progression inspector variable.
	/// Automatically updates Cycle.Day if enabled.
	public bool ProgressDate = true;

	/// Moon phase progression inspector variable.
	/// Automatically updates Moon.Phase if enabled.
	public bool ProgressMoonPhase = true;

	/// Adjust the time progress according to the time curve.
	public bool UseTimeCurve = false;

	/// Time of day progression curve.
	/// Can be used to make days longer and nights shorter.
	public AnimationCurve TimeCurve = AnimationCurve.Linear(0, 0, 24, 24);

	private TOD_Sky sky;

	private float skyTime;

	private AnimationCurve timeCurve;
	private AnimationCurve timeCurveInverse;

	private void CalculateLinearTangents(Keyframe[] keys)
	{
		for (int i = 0; i < keys.Length; i++)
		{
			var key = keys[i];

			if (i > 0)
			{
				var prev = keys[i-1];
				key.inTangent = (key.value - prev.value) / (key.time - prev.time);
			}

			if (i < keys.Length-1)
			{
				var next = keys[i+1];
				key.outTangent = (next.value - key.value) / (next.time - key.time);
			}

			keys[i] = key;
		}
	}

	private void ApproximateCurve(AnimationCurve source, out Keyframe[] resKeys, out Keyframe[] resKeysInverse)
	{
		const float minstep = 0.01f;

		resKeys        = new Keyframe[25];
		resKeysInverse = new Keyframe[25];

		float time = -minstep;
		for (int i = 0; i < 25; i++)
		{
			time = Mathf.Max(time + minstep, source.Evaluate(i));

			resKeys[i]        = new Keyframe(i, time);
			resKeysInverse[i] = new Keyframe(time, i);
		}
	}

	/// Apply changes made to TimeCurve
	public void ApplyTimeCurve()
	{
		TimeCurve.preWrapMode  = WrapMode.Clamp;
		TimeCurve.postWrapMode = WrapMode.Clamp;

		Keyframe[] timeCurveKeys, timeCurveKeysInverse;
		ApproximateCurve(TimeCurve, out timeCurveKeys, out timeCurveKeysInverse);
		CalculateLinearTangents(timeCurveKeys);
		CalculateLinearTangents(timeCurveKeysInverse);

		timeCurve = new AnimationCurve(timeCurveKeys);
		timeCurve.preWrapMode  = WrapMode.Loop;
		timeCurve.postWrapMode = WrapMode.Loop;

		timeCurveInverse = new AnimationCurve(timeCurveKeysInverse);
		timeCurveInverse.preWrapMode  = WrapMode.Loop;
		timeCurveInverse.postWrapMode = WrapMode.Loop;
	}

	/// Adds delta to the sky dome time.
	/// Ensures that all values remain in a valid range.
	public void AddTime(float delta, bool adjust = false)
	{
		if (UseTimeCurve && adjust)
		{
			float time = delta + timeCurveInverse.Evaluate(skyTime);
			delta = timeCurve.Evaluate(time) - skyTime;

			if (ProgressDate)
			{
				if (time >= 24)
				{
					delta += ((int)time / 24) * 24;
				}
				else if (time < 0)
				{
					delta += ((int)time / 24 - 1) * 24;
				}
			}
		}

		if (skyTime != sky.Cycle.Hour)
		{
			delta += sky.Cycle.Hour - skyTime;
			sky.Cycle.Hour = skyTime;
		}

		if (ProgressDate)
		{
			sky.Cycle.Hour += delta;
			if (sky.Cycle.Hour >= 24)
			{
				int days = (int)sky.Cycle.Hour / 24;
				sky.Cycle.Hour -= days * 24;
				sky.Cycle.DateTime = sky.Cycle.DateTime.AddDays(days);
			}
		}
		else
		{
			sky.Cycle.Hour += delta;
			CheckTimeRange();
		}

		skyTime = sky.Cycle.Hour;
	}

	/// Adds delta to the moon phase.
	/// Ensures that all values remain in a valid range.
	public void AddMoon(float delta)
	{
		sky.Moon.Phase += delta;
		CheckMoonRange();
	}

	/// Forces the time values to be within a valid range.
	public void CheckTimeRange()
	{
		sky.Cycle.Year  = Mathf.Clamp(sky.Cycle.Year, 1, 9999);
		sky.Cycle.Month = Mathf.Clamp(sky.Cycle.Month, 1, 12);
		sky.Cycle.Day   = Mathf.Clamp(sky.Cycle.Day, 1, DateTime.DaysInMonth(sky.Cycle.Year, sky.Cycle.Month));
		sky.Cycle.Hour  = Mathf.Repeat(sky.Cycle.Hour, 24);
	}

	/// Forces the moon phase to be within a valid range.
	public void CheckMoonRange()
	{
		if (sky.Moon.Phase > 1)
		{
			sky.Moon.Phase -= (int)sky.Moon.Phase + 1;
		}
		else if (sky.Moon.Phase < -1)
		{
			sky.Moon.Phase -= (int)sky.Moon.Phase - 1;
		}
	}

	protected void Awake()
	{
		sky = GetComponent<TOD_Sky>();

		ApplyTimeCurve();
	}

	protected void OnEnable()
	{
		skyTime = sky.Cycle.Hour;
	}

	protected void Update()
	{
		float oneDay  = DayLengthInMinutes * 60;
		float oneHour = oneDay / 24;

		if (ProgressTime)
		{
			AddTime(Time.deltaTime / oneHour, true);
		}
		else
		{
			CheckTimeRange();
		}

		if (ProgressMoonPhase)
		{
			AddMoon(Time.deltaTime / (30*oneDay) * 2);
		}
		else
		{
			CheckMoonRange();
		}
	}
}
