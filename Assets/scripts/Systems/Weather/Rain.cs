using UnityEngine;

public class Rain : MonoBehaviour
{
    //variables
    public Transform camPos;
    public bool isRainning = true; 
    [SerializeField] private GameObject rainParticle;
    [SerializeField] GameObject rainRippleParticle; 


    void Update()
    {
        if (camPos != null)
        {
          
            transform.position = new Vector3(camPos.position.x, transform.position.y, camPos.position.z);  //update the position of the rain system based on the camera position
        }
//allow user to control if raining
        if(Input.GetKeyDown(KeyCode.E))
        {
            isRainning = !isRainning;
        }

        if (isRainning)
        {
            
            EnableRain();
        }
        else
        {
            
            DisableRain();
        }
    }
/*
These functions just enable or disable the rainparticles.
*/
    void EnableRain()
    {
            rainParticle.SetActive(true);
            rainRippleParticle.SetActive(true);
       
    }

    void DisableRain()
    {
        rainParticle.SetActive(false);
            rainRippleParticle.SetActive(false);
    }
}
