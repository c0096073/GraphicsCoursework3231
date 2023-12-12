using UnityEngine;
using System.Collections;

public class WeatherSystem : MonoBehaviour
{
    //variables
    public Material rainySkybox;
    public Material skybox;
    public Rain rainsystem;
    public AudioSource rainSound;
    public AudioSource daySound;

    private bool isRainSoundPlaying = false;
    private bool isDaySoundPlaying = false;

    public SkyBoxForDayNight skyboxfordaynight;

    void Start()
    {
        GameObject rainObj = GameObject.Find("Rain");

        if (rainObj != null)
        {
            rainsystem = rainObj.GetComponent<Rain>();//get rain compononet
        }
    }

    void Update()
    {
        if (rainsystem != null)
        {
            if (rainsystem.isRainning)//if raining
            {
                //check if the rain sound is not already playing
                if (!isRainSoundPlaying)
                {
                    //fade out day sound and fade in rain sound
                    StartCoroutine(FadeAudio(daySound, 1.0f, 0.0f));
                    StartCoroutine(FadeAudio(rainSound, 1.0f, 0.168f));
                    if (!skyboxfordaynight.isNight)
                    {
                        StartCoroutine(FadeSkybox(rainySkybox, 1.0f));
                    }
                    StartCoroutine(FadeAmbientLight(1.0f, 0.5f)); //darker when raining
                    isRainSoundPlaying = true;
                    isDaySoundPlaying = false;
                }
            }
            else
            {
                //check if the day sound is not already playing
                if (!isDaySoundPlaying)
                {
                    //fade out rain sound and fade in day sound
                    StartCoroutine(FadeAudio(rainSound, 1.0f, 0.0f));
                    StartCoroutine(FadeAudio(daySound, 1.0f, 0.168f));
                     if (!skyboxfordaynight.isNight)
                    {
                          StartCoroutine(FadeSkybox(skybox, 1.0f));
                    }
                    StartCoroutine(FadeAmbientLight(0.5f, 1.0f)); //lighter when not raining
                    isDaySoundPlaying = true;
                    isRainSoundPlaying = false;
                }
            }
        }
    }

    // Code Derived from: https://johnleonardfrench.com/how-to-fade-audio-in-unity-i-tested-every-method-this-ones-the-best/
    public static IEnumerator FadeAudio(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float startVolume = audioSource.volume;

        //start playing the audio if it's not already playing
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);//will smoothly fade between current volume->target volume(will basically mute or unmute the rain audio)
            yield return null;
        }

        //if the target volume is 0 stop playing the audio
        if (targetVolume == 0.0f)
        {
            audioSource.Stop();
        }
    }

//follow same idea as fading the audio but using the lighting and the skybox instead. 
    public static IEnumerator FadeSkybox(Material targetSkybox, float duration)
    {
        Material startSkybox = RenderSettings.skybox;//current skybox
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            RenderSettings.skybox.Lerp(startSkybox, targetSkybox, currentTime / duration);//similar to audio but this time itll switch between skyboxes(depending if going from rainy->clear or vice versa)
            yield return null;
        }

        RenderSettings.skybox = targetSkybox;//sets skybox as the new target skybox
    }

    public static IEnumerator FadeAmbientLight(float startIntensity, float targetIntensity, float duration = 1.0f)
    {
        float currentTime = 0;
       

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            RenderSettings.ambientIntensity = Mathf.Lerp(startIntensity, targetIntensity, currentTime / duration);//similar idea to audio but now its just the intensity of the ambient light(the cloudy sky will have less sunlight)
            yield return null;
        }

        RenderSettings.ambientIntensity = targetIntensity;
    }
}
