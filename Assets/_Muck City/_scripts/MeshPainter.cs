// Sets the vertex color to be red at the y=0 and green at y=1.
// (Note that most built-in Shaders don't display vertex colors. Use one that does, such as a Particle Shader, to see vertex colors)

using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
public class MeshPainter : MonoBehaviour
{
    // [Button]
    // public void PaintMesh(Transform obj)
    // {
    //     Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;
    //     Vector3[] vertices = mesh.vertices;

    //     // create new colors array where the colors will be created.
    //     Color[] colors = new Color[vertices.Length];

    //     for (int i = 0; i < vertices.Length; i++)
    //         colors[i] = Color.Lerp(Color.red, Color.green, vertices[i].y);

    //     // assign the array of colors to the Mesh.
    //     mesh.colors = colors;
    // }


    [Button]
    public void PaintMesh(Transform obj)
    {
        Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] vertices = mesh.vertices;

        // create new colors array where the colors will be created.
        Color[] colors = new Color[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
            colors[i] = Color.Lerp(Color.red, Color.green, vertices[i].y);

        // create a new mesh instance
        Mesh newMesh = Instantiate(mesh);
        newMesh.colors = colors;

        // save the new mesh as an asset
        AssetDatabase.CreateAsset(newMesh, "Assets/PaintedMesh.mesh");
        AssetDatabase.SaveAssets();

        // assign the new mesh to the MeshFilter
        obj.GetComponent<MeshFilter>().mesh = newMesh;
    }
}
