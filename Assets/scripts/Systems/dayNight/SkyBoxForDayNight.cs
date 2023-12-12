using UnityEngine;
using System.Collections;

public class SkyBoxForDayNight : MonoBehaviour
{
    //variables
    public Material daySkybox;
    public Material nightSkybox;
    public float cycleDuration = 600; 
    public float dayAmbientIntensity = 1.0f;
    public float nightAmbientIntensity = 0.1f;
    public Light sunLight; //reference to the directional light for the sun
    public Light moonLight; //reference to the directional light for the moon

    private bool isDaySkyboxActive = true;
    public bool isNight = false;


    void Start()
    {//start in daytime
        sunLight.enabled = true;
        moonLight.enabled = false;
    }
    void Update()
    {
        float lerpValue = Mathf.PingPong(Time.time / cycleDuration, 1.0f);//will go between 0 and 1 to simulate day and night and the cycleDuration will be the length of these day/night

        if (lerpValue >= 0.5f && isDaySkyboxActive)
        {
            //transition to night skybox
            StartCoroutine(FadeSkybox(daySkybox, nightSkybox, 1.0f));
            StartCoroutine(FadeAmbientLight(dayAmbientIntensity, nightAmbientIntensity, 1.0f));
            isDaySkyboxActive = false;
            isNight = true;

            //disable sun and enable moon
            if (sunLight != null)
                sunLight.enabled = false;
            if (moonLight != null)
                moonLight.enabled = true;
        }
        else if (lerpValue < 0.5f && !isDaySkyboxActive)
        {
            //transition to day skybox
            StartCoroutine(FadeSkybox(nightSkybox, daySkybox, 1.0f));
            StartCoroutine(FadeAmbientLight(nightAmbientIntensity, dayAmbientIntensity, 1.0f));
            isDaySkyboxActive = true;
            isNight = false;

            //enable sun and disable moon
            if (sunLight != null)
                sunLight.enabled = true;
            if (moonLight != null)
                moonLight.enabled = false;
        }
    }

    //These are the same functions as in the WeatherSystem script. They just smoothly transition from one thing(skybox, light, sound, etc) to the other over a set duration.
    IEnumerator FadeSkybox(Material startSkybox, Material targetSkybox, float duration)
    {
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            RenderSettings.skybox.Lerp(startSkybox, targetSkybox, currentTime / duration);//transition smoothly from start to target over a set time
            yield return null;
        }

        RenderSettings.skybox = targetSkybox;
    }

    IEnumerator FadeAmbientLight(float startIntensity, float targetIntensity, float duration = 1.0f)
    {
        float currentTime = 0;
    

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            RenderSettings.ambientIntensity = Mathf.Lerp(startIntensity, targetIntensity, currentTime / duration);
            yield return null;
        }

        RenderSettings.ambientIntensity = targetIntensity;
    }
}
