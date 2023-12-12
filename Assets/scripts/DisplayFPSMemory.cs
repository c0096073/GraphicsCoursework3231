using UnityEngine;
using TMPro;

public class FPSMemoryDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI memoryText;

    void Update()
    {
        float fps = 1f / Time.deltaTime;
        fpsText.text = "FPS: " + fps.ToString();

        //https://docs.unity3d.com/ScriptReference/Profiling.Profiler.html
        float memoryUsage = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);
        memoryText.text = "Memory: " + memoryUsage.ToString() + " MB";

    }
}
