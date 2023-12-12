using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    //variables
    public float dayDuration = 60f; //duration of a full day in seconds

  
    public float SunRotation;

    void Start()
    {
        SunRotation = transform.rotation.eulerAngles.x;
    }
    void Update()
    {
        UpdateSunPosition();
    }
/*
Function that will simulate the light(sun) rotation throughout the day.
*/
    void UpdateSunPosition()
    {
       
        float angle = Time.time / dayDuration * 360f; //calculate the rotation angle based on the time of day

       
        transform.rotation = Quaternion.Euler(new Vector3(angle, 0, 0)); //apply the rotation to the Directional Light
    }
}