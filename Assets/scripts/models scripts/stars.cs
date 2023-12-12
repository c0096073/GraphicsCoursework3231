using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stars : MonoBehaviour
{

    public GameObject starObj;
   public int numOfStars;
   public SkyBoxForDayNight daynight;//reference to the skybox changer script which has a boolean "isNight" which i will use to see when its night in the scene
    private List<GameObject> listOfStarObj = new List<GameObject>();
   
    
    void Start()
    {
        //this spawns all the stars at the very start of loading the scene similar to all the vegetation objects.
        //this is so i can simply "hide" or "unhide" them when i want and therefore not waste having to recreate them every time. 
        for (int i = 0; i < numOfStars; i++)
        {
        //the random postitions are generated each iteration.
        float randomX = Random.Range(0f, 150f);  
        float randomZ = Random.Range(0f, 150f); 
        float randomY = Random.Range(62f, 81f);

        GameObject newStarObj = Instantiate(starObj, new Vector3(randomX, randomY, randomZ), Quaternion.identity);//the starobject is then created at these random positions
            listOfStarObj.Add(newStarObj);//and added to the list of star objects
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (daynight.isNight)//if its night time then show the stars
        {
            spawnStars();
        }
        else{//if its day time then hide the stars
            hideStars();
        }
    }

    public void spawnStars()
    {
        //this goes through each star in the list of star objects and setting each to active so unity renders them
         foreach (GameObject starObj in listOfStarObj)
        {
            starObj.SetActive(true);
        }
    }

    public void hideStars()
    {
        //this goes through each star in the list of star objects and setting each to unactive so unity doesnt render them
         foreach (GameObject starObj in listOfStarObj)
        {
            starObj.SetActive(false);
        }
    }
}
