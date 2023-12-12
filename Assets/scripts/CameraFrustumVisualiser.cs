using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFrustumVisualiser : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Camera camera = GetComponent<Camera>();
        if (camera == null)
            return;

        Gizmos.color = Color.green;
        Matrix4x4 matrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(camera.transform.position, camera.transform.rotation, Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, camera.fieldOfView, camera.farClipPlane, camera.nearClipPlane, camera.aspect);
        Gizmos.matrix = matrix;
    }
}