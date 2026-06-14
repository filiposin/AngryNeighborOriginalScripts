using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
	public Light DirectionLight;

	public Texture2D SkyColors;

	public Texture2D LightColor;

	public Texture2D AmbientColor;

	public float TimePerDay = 2f;

	[Range(0f, 1f)]
	public float Timer;

	private float timeTemp;

	private float fade;

	public float MaxFade = 20f;

	public bool FadeOnTime = true;

	public bool Freeze;

	public bool LightRotation;

	private void Start()
	{
		timeTemp = Time.time;
		fade = Timer;
	}

	private void Update()
	{
		if (!Freeze && (Network.isServer || (!Network.isClient && !Network.isServer)))
		{
			if (FadeOnTime)
			{
				float num = TimePerDay / MaxFade;
				if (Time.time >= timeTemp + num)
				{
					timeTemp = Time.time;
					fade += 1f / MaxFade;
				}
				if (fade > 1f)
				{
					fade = 0f;
					Timer = 0f;
				}
				Timer += (fade - Timer) / 10f;
			}
			else if (Timer > 1f)
			{
				Timer = 0f;
			}
			else
			{
				Timer += 1f * Time.deltaTime * (1f / TimePerDay);
			}
		}
		if (LightRotation)
		{
			DirectionLight.transform.rotation = Quaternion.Euler(new Vector3(360f * Timer, 0f, 0f));
		}
		Color pixelBilinear = SkyColors.GetPixelBilinear(Timer, 0f);
		RenderSettings.skybox.SetColor("_Tint", pixelBilinear);
		RenderSettings.fogColor = pixelBilinear;
		RenderSettings.ambientLight = AmbientColor.GetPixelBilinear(Timer, 0f);
		DirectionLight.color = LightColor.GetPixelBilinear(Timer, 0f);
	}
}
