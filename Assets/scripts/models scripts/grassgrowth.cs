using UnityEngine;

public class grassgrowth : MonoBehaviour
{
    //variables
    public float initialHeight = 0.01f; 
    public float targetHeight = 0.03f; 
    public float growthDuration = 10.0f; //in seconds

    private float startTime;

    void Start()
    {
     
        transform.localScale = new Vector3(transform.localScale.x, initialHeight, transform.localScale.z); //initial Y scale of the grass

        
        startTime = Time.time;
    }
/*
Function that'll simulate the grass growing by changing its y scale over time.
*/
    void Update()
    {
        float progress = Mathf.Clamp01((Time.time - startTime) / growthDuration);  //calculate the progress towards the target height

      
        float newYScale = Mathf.Lerp(initialHeight, targetHeight, progress);//use lerp to interpolate between the initial and target height based on the progress.
        transform.localScale = new Vector3(transform.localScale.x, newYScale, transform.localScale.z);//update the grass y scale

      
    }
}
