using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class birdspawner : MonoBehaviour
{
   public GameObject birdPrefab;
   public int numofBirdsToSpawn;
    // Start is called before the first frame update
    void Start()
    {
      numofBirdsToSpawn = Random.Range(4,10);//this decides a random amount of birds to spawn so its not the same each time. adding to the procedural generation aspect of the planet
        StartCoroutine(SpawnBirdWithDelay());
        
    }


     
  /* 
  The reason i spawn the birds with a slight delay, is because they follow a predetermined path. So if i spawned them all at once they would all be at the exact
  same position in that path but by adding a delay they all start at a different position in their movement so it seems more realistic.
  */
  public IEnumerator SpawnBirdWithDelay()
  {
    for (int i = 0; i < numofBirdsToSpawn; i++)
    {
        //this decides where they will spawn on the map.
        float randomX = Random.Range(10F, 145f); 
        float randomY = Random.Range(30f, 42f);
        float randomZ = Random.Range(10f, 145F);

          Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);
          yield return new WaitForSeconds(0.5f);//this is the slight delay given before the birds are spawned
            Instantiate(birdPrefab, randomPosition, Quaternion.identity);
    }
    

    }
}
