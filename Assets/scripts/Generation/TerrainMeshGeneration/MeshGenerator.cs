using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    //variables

    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Vector2Int size;

    //terrain (noise) parameters
        //these will be randomized so i have clamped them to a minimum and maximum values.
    [SerializeField] private float minimumAmplitude = 30f;
    [SerializeField] private float maxAmplitude = 50f;
    [SerializeField] private float amplitude;
     [SerializeField] private float minimumScale = 30f;
    [SerializeField] private float maxScale = 55f;

    //these will not be random.

    [SerializeField] float scale = 0.1f;
    [SerializeField] int octaves = 4;
    [SerializeField] float persistence = 0.5f;
    [SerializeField] float lacunarity = 2f;

    // Falloff parameters with sliders
    [SerializeField, Range(1f, 10f)] float falloffA = 3f;
    [SerializeField, Range(1f, 10f)] float falloffB = 2.2f;

    [SerializeField] Material mat;

    //water variables
    public bool waterSpawned = false;
    public GameObject waterPlane;//reference to the actual water plane
    public float wateroffset = 2f;
    public float waterPlaneHeight;

    //mesh variables
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    //list of texture layers
    [SerializeField] List<Layer> terrainLayers = new List<Layer>();

    public bool autoUpdate;//so i can update in the scene editor for debug purposes

    void OnValidate()
    {
        if (meshFilter != null)
        {
            amplitude = Random.Range(minimumAmplitude, maxAmplitude);
                    scale = Random.Range(minimumScale, maxScale);

            CreateVertices();
            UpdateMesh();
            GenerateTexture();
        }
    }

    void Start()
    {
        amplitude = Random.Range(minimumAmplitude, maxAmplitude);//get a random amplitude
        scale = Random.Range(minimumScale, maxScale);//get a random scale of the noise map 
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        //create the terrain
        CreateVertices();
        UpdateMesh();
        GenerateTexture();

         float WaterLayerHeight = GetWaterLayerHeight();//get the hieght at which the sand texture ends and the grass texture starts
          SpawnWaterPlane(WaterLayerHeight - wateroffset);//spawn the water at this height with a slight offset 
          
    }
/*
    update the mesh collider to match the generated terrain
*/
     void UpdateMeshCollider()
    {
       
        MeshCollider meshCollider = GetComponent<MeshCollider>();

      
        if (meshCollider != null)
        {
           
            meshCollider.sharedMesh = mesh;//assign the same mesh used for rendering to the mesh collider
        }
       
    }
/*
Function to create the terrain vertices 
*/
    public void CreateVertices()
    {
        //use the Noise script to generate a noise map
        float[,] noiseMap = Noise.GenerateNoiseMap(size.x + 1, size.y + 1, 123, scale, octaves, persistence, lacunarity, Vector2.zero);//pass the size of the terrain as well as the noise parameters

        vertices = new Vector3[(size.x + 1) * (size.y + 1)];//initialize vertices array 

        float[,] falloffMap = GenerateFalloffMap(size.x + 1, size.y + 1);//generate a falloff map 

        for (int z = 0, i = 0; z <= size.y; z++)
        {
            for (int x = 0; x <= size.x; x++)
            {
                float y = noiseMap[x, z] * amplitude * falloffMap[x, z];//calculate height based on noise, amplitude and falloffMap
                vertices[i] = new Vector3(x, y, z);//vertex position 
                i++;
            }
        }

        triangles = new int[size.x * size.y * 6];//initialize triangles array 

        for (int z = 0, vert = 0, tris = 0; z < size.y; z++)
        {
            for (int x = 0; x < size.x; x++)
            {
                //triangles are defined 
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + size.x + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + size.x + 1;
                triangles[tris + 5] = vert + size.x + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

/*
Generates a falloff Map.
code for falloff map and evaluating it has been derived from Sebastian Lague: https://www.youtube.com/watch?v=COmtTyLCd6I and edited to be used and work in my program
*/
    private float[,] GenerateFalloffMap(int width, int height)
    {
        float[,] map = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //normalize x and y 
                float xNormalized = x / (float)width;
                float yNormalized = y / (float)height;
           

                float value = Mathf.Max(Mathf.Abs(xNormalized * 2 - 1), Mathf.Abs(yNormalized * 2 - 1)); //calculate a falloff value based on the normalized coordinates.
                map[x, y] = EvaluateFalloff(value);
            }
        }

        return map;
    }

    private float EvaluateFalloff(float value)
    {
        return Mathf.Pow(falloffA - value, falloffB) / (Mathf.Pow(falloffA - value, falloffB) + Mathf.Pow(falloffB - falloffB * value, falloffB));
    }
/*
    update the mesh based on the vertices and triangles created.
*/
    public void UpdateMesh()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
            meshFilter.mesh = mesh;
        }

        mesh.Clear();//clear last mesh.
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();//recalculate normals of the mesh
        UpdateMeshCollider();//update the collider so it matches the shape of the terrain
    }
/*
This generates textures based on height values.
*/
    private void GenerateTexture()
    {
        float minTerrainHeight = float.MaxValue;
        float maxTerrainHeight = float.MinValue;

        //find min and max terrain height
        foreach (var vertex in vertices)//iterates through each vertices
        {
            float y = vertex.y;
            //sets the maximum or minimum terrain heights
            if (y > maxTerrainHeight)//if vetex y > maxTerrainHeight then maxTerrainHeight is reassigned to this new y value
            {
                maxTerrainHeight = y;
            }
            if (y < minTerrainHeight)//if vetex y < minTerrainHeight then minTerrainHeight is reassigned to this new y value
            {
                minTerrainHeight = y;
            }
        }

        //set material properties
        mat.SetFloat("minTerrainHeight", minTerrainHeight);
        mat.SetFloat("maxTerrainHeight", maxTerrainHeight);

        int layersCount = terrainLayers.Count;//gets the number of terrain layers

        
        mat.SetInt("numTextures", layersCount);

        //set layer heights
        float[] heights = new float[layersCount];
        for (int i = 0; i < layersCount; i++)
        {
            heights[i] = terrainLayers[i].startHeight;//store starting height of each terrain layer
        }
        mat.SetFloatArray("terrainHeights", heights);

        //set layer textures and blend factors
        Texture2DArray textures = new Texture2DArray(512, 512, layersCount, TextureFormat.RGBA32, true);
        float[] blendFactors = new float[layersCount];

        for (int i = 0; i < layersCount; i++)
        {
            textures.SetPixels(terrainLayers[i].texture.GetPixels(), i);//set pixels of the texture2darray for the current layer

            blendFactors[i] = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, terrainLayers[i].startHeight);//calculate blend factor based on height difference
        }

        textures.Apply();
        mat.SetTexture("terrainTextures", textures);
     
    }
/*
small function to get the height of the mountain layer and the first layer (the layer where the water will spawn)
*/
    public float GetMountainLayerHeight()
    {
        if (terrainLayers.Count > 1){
            return terrainLayers[2].startHeight * amplitude; //return the starting height of the second layer
        }
        return 0f;
    }

     public float GetWaterLayerHeight()
    {
     
        if (terrainLayers.Count > 0)
        {
              
           
            return terrainLayers[1].startHeight * amplitude;
        }
      
       
        return 0f;
    }
/*
Function that will "spawn" the water (will move it to the correct position)
*/
    void SpawnWaterPlane(float waterPlaneHeight)//takes the waterplane height as a variable
    {
        Vector3 waterPlanePos = waterPlane.transform.position;
        waterPlanePos.y = waterPlaneHeight;//set the waterPlaneHeight that was calculated as the y value 

        waterPlane.transform.position = waterPlanePos;//move the waterplane to this y value

         this.waterPlaneHeight = waterPlaneHeight;
         waterSpawned = true;//water has been spawned in
    }

    [System.Serializable]
    class Layer
    {
        public Texture2D texture;
        [Range(0, 1)] public float startHeight;
    }
}


