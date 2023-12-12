using UnityEngine;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    //variables 

    //Prefab lists
    [SerializeField] List<GameObject> treePrefabs = new List<GameObject>();
    [SerializeField] int maxNumberOfTrees = 100;

    [SerializeField] List<GameObject> rockPrefabs = new List<GameObject>();
    [SerializeField] int maxNumberOfRocks = 10;

    [SerializeField] List<GameObject> vegetationPrefabs = new List<GameObject>();
    [SerializeField] int maxNumberOfVegetation = 50;

    [SerializeField] List<GameObject> grassPrefabs = new List<GameObject>();
    [SerializeField] int maxNumberOfGrass = 50;

    //layers
    [SerializeField] LayerMask terrainLayer;  
    [SerializeField] LayerMask waterLayer; 

    public float waterReferenceHeight; //height of water plane, will be assigned in Start
    
    Dictionary<string, List<GameObject>> spawnedObjects = new Dictionary<string, List<GameObject>>();//dictionary to keep track of spawned objects


    [SerializeField] float spawnDistance = 50f;  //distance from the camera at which objects will be spawned
    [SerializeField] float spawnBuffer = 5f;  //buffer to avoid spawning objects too close to the camera

    void Start()
    {
        MeshGenerator meshgenerator = GetComponent<MeshGenerator>();//get the terrain mesh generator
    
        if(GetComponent<MeshGenerator>().waterSpawned)//check if the water has been spawned before trying to spawn anything in
        {
            waterReferenceHeight = GetComponent<MeshGenerator>().waterPlaneHeight;//get the height of the water plane
            StartCoroutine(SpawnObjectsCoroutine()); //once the height of the water has been found objects can be spawned
        }
    }
/*
Co routine to spawn objects(trees, rocks, vegetation) on the terrain.
*/
    IEnumerator<object> SpawnObjectsCoroutine()
    {
        while (true)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);//get the cameras frustum planes

            SpawnObjectsOnMesh(treePrefabs, maxNumberOfTrees, "Tree", planes);//spawn based on the frustum plane
            SpawnObjectsOnMesh(rockPrefabs, maxNumberOfRocks, "Rock", planes);
            SpawnObjectsOnMesh(grassPrefabs, maxNumberOfGrass, "Grass", planes);
            SpawnObjectsOnMesh(vegetationPrefabs, maxNumberOfVegetation, "Vegetation", planes);

           
            yield return null;
        }
    }
/*
Function that spawns the objects on the terrain based on the frustum planes of the camera.
*/

void SpawnObjectsOnMesh(List<GameObject> prefabList, int count, string type, Plane[] planes)//takes the object, the amount to spawn, what object it is and the frustum plane as parameter
{
   
    if (!spawnedObjects.ContainsKey(type)) //ensure the dictionary has an entry for the current thing being spawned (Tree, Rock, Grass or Vegetation)
    {
        spawnedObjects[type] = new List<GameObject>();
    }

    Mesh mesh = GetComponent<MeshFilter>().mesh;

    if (mesh == null)
    {
        Debug.LogError("Mesh not found.");
        return;
    }

    Vector3[] vertices = mesh.vertices;//get the mesh vertices 

    int spawnedCount = spawnedObjects[type].Count;//number of objects of that type that have been spawned

    for (int i = spawnedCount; i < count; i++)//whilst the amount of objects spawned is less than the maxNumberOf 
    {
        if (spawnedCount >= count) 
        {
            break;
        }

        int randomPrefabIndex = Random.Range(0, prefabList.Count);//Get a random game object from the list of gameobjects (eg out of the 5 tree objects it might choose tree #2)

        Vector3 randomPoint;
        RaycastHit hit = new RaycastHit(); //initialize raycast 

        //The amount of times itll try to spawn an item
        int attempts = 0;
        int maxAttempts = 10; 

        do
        {
            //if the object is grass it'll spawn them slightly higher on the terrain as they're small meshes it matters more if theyre slightly underground
            if (type == "Grass")
            {
                
                int randomVertexIndex = Random.Range(0, vertices.Length);//get a random vertex index
                randomPoint = vertices[randomVertexIndex];//get the position of the random vertex

                //adjust the random point based on terrain normal
                Vector3 normal = hit.normal.normalized;
                randomPoint += normal * 0.1f; 
            }
            else
            {
                //for other types use random points 
                int randomVertexIndex = Random.Range(0, vertices.Length);
                randomPoint = vertices[randomVertexIndex];
            }

            attempts++;//increase the amount of attempts
        } 
        
        while (attempts < maxAttempts && !IsValidSpawnPoint(randomPoint, out hit));
        {
        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Unable to find a valid spawn point after " + maxAttempts + " attempts");
            break;  //exit the loop if a valid spawn cannot be found
        }
        }
     
        randomPoint.y = hit.point.y;  //use the y value of the hit point

        //instantiate(spawn) the object and add it to the spawnedObjects list
        GameObject spawnedObject = Instantiate(prefabList[randomPrefabIndex], randomPoint, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
        spawnedObjects[type].Add(spawnedObject);
    }

    //update the visibility of spawned objects(this is to help performance, dont want to render something if it's not going to be seen on screen.)
    UpdateObjectVisibility(planes, count, type);
}


   void UpdateObjectVisibility(Plane[] planes, int count, string type)
    {
        List<GameObject> objectsOfType = spawnedObjects[type];

        for (int i = 0; i < objectsOfType.Count; i++)
        {
            GameObject obj = objectsOfType[i];

            if (obj != null)
            {
              
                bool isInFrustum = IsInCameraFrustum(obj, obj.transform.position, planes);  //check if the object is within the camera's frustum and close enough to the camera

                //check if the object is beyond the buffer distance
                float distanceToCamera = Vector3.Distance(obj.transform.position, Camera.main.transform.position);
                if (isInFrustum && distanceToCamera <= spawnDistance + spawnBuffer && objectsOfType.Count <= count)//if object is in cameras view and is in range
                {
                   
                    SetObjectVisibility(obj, true);//enable and render the object and show it on screen
                }
                else
                {
                   
                    SetObjectVisibility(obj, false);//disable and stop rendering the object 
                }
            }
        }
    }
/*
This bool function will see if an object is in the camera view and return true if it is or false if it isn't.

*/
    bool IsInCameraFrustum(GameObject obj, Vector3 point, Plane[] planes)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null)
        {
            return false;
        }

        Bounds bounds = renderer.bounds;//get bounding box of object
        Vector3[] boundsCorners = new Vector3[8];//create an array of each corner of the bounding box
        //define the box
        boundsCorners[0] = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
        boundsCorners[1] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
        boundsCorners[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        boundsCorners[3] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
        boundsCorners[4] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
        boundsCorners[5] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
        boundsCorners[6] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
        boundsCorners[7] = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);

    //check if the bounding box corners are inside all planes of the camera frustum
        for (int i = 0; i < planes.Length; i++)
        {
            int insideCount = 0;
             //check each corner of the bounding box against the current frustum plane
            for (int j = 0; j < boundsCorners.Length; j++)
            {
                if (planes[i].GetSide(boundsCorners[j]))
                {
                    insideCount++;
                }
            }

            if (insideCount == 0)//if no corners are inside the current frustum plane the object is outside the frustum
            {
                return false;
            }
        }

        return true;//the object is inside the frustum planes
    }
/*
A function to check if the point where an object is trying to spawn is allowed or not. 
Mostly will check the point is on the terrain but also not under the water for example.
*/
bool IsValidSpawnPoint(Vector3 point, out RaycastHit hit)
{
    //raycast to check the point is on the terrain
    if (Physics.Raycast(new Vector3(point.x, 50f, point.z), Vector3.down, out hit, Mathf.Infinity, terrainLayer))
    {
        
          //get the heights of where the mountain(stone area) and the water starts
            float mountainLayerHeight = FindObjectOfType<MeshGenerator>().GetMountainLayerHeight();
            float waterLayerHeight = GetComponent<MeshGenerator>().waterPlaneHeight;

            float hitY = hit.point.y;//height where raycast hit
            if(hitY > waterLayerHeight)//if raycast height is above where the water is
            {
               if (hitY < mountainLayerHeight + 1f)  //if raycast height is also lower than the mountain layer height with a bit of offset for realism(dont want a complete sudden cut off)
               {
                return true;//The spawnpoint is valid 
               }
           
            }
             else
            {
                return false;//the spawnpoint is NOT valid
            }  
           
        
    }
    else
    {
        Debug.LogWarning("Raycast did not hit terrain");
    }

    return false;
}


    void SetObjectVisibility(GameObject obj, bool isVisible)
    {
        obj.SetActive(isVisible);//the isVisible will be either true or false
    }
}
