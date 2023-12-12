/* 
NOISE FUCNTION ADAPTED FROM SEBASTIAN LAUGE:

https://www.youtube.com/watch?v=WP-Bm65Q-1Y
https://www.youtube.com/watch?v=MRNFcywkUSA


 */
//noise generation
using UnityEngine;

public static class Noise
{
    // Generates a 2D noise map based on parameters
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
    {
        // Initialize a 2D array to store the generated noise map
        float[,] noiseMap = new float[mapWidth, mapHeight];

        // Create a pseudo-random number generator using the provided seed
        System.Random prng = new System.Random(seed);

        // Create an array to store offsets for each octave
        Vector2[] octaveOffsets = new Vector2[octaves];

        // Generate random offsets for each octave
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        // Ensure that the scale is not zero to prevent division by zero
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        // Variables to track the minimum and maximum noise height for normalization
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        // Calculate half-width and half-height for optimization
        float halfwidth = mapWidth / 2f;
        float halfheight = mapHeight / 2f;

        // Nested loops to iterate through each point on the noise map
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // Initialize variables for amplitude, frequency, and cumulative noise height
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                // Loop through each octave and accumulate perlin noise values
                for (int i = 0; i < octaves; i++)
                {
                    // Calculate the sample coordinates for the current octave
                    float sampleX = (x - halfwidth) / scale * frequency + octaveOffsets[i].x * frequency;
                    float sampleY = (y - halfheight) / scale * frequency + octaveOffsets[i].y * frequency;

                    // Generate perlin noise value and adjust to the range [-1, 1]
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    // Accumulate noise value with amplitude
                    noiseHeight += perlinValue * amplitude;

                    // Modify amplitude and frequency for the next octave
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                // Apply falloff function
                float falloff = Falloff(x / (float)mapWidth, y / (float)mapHeight);
                noiseHeight *= falloff;

                // Update min and max noise height for normalization
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                // Assign the noise value to the current map position
                noiseMap[x, y] = noiseHeight;
            }
        }

        // Normalize the noise map values to the range [0, 1]
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        // Return the generated noise map
        return noiseMap;
    }

    // Falloff function to apply smoothing 
    private static float Falloff(float x, float y)
    {
        float a = 3f;
        float b = 2.2f;

        return Mathf.Pow(a - x, b) / (Mathf.Pow(a - x, b) + Mathf.Pow(a - y, b));
    }
}


