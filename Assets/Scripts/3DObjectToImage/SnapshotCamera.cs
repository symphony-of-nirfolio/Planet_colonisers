using UnityEngine;

public class SnapshotCamera : MonoBehaviour
{
    public RenderTextureParametersForBuilding renderTextureParametersForBuilding;

    public Camera currentCamera;
    
    private int width;
    private int height;
    private int depth;


    public void TakeSnapshot(string fileName)
    {
        Texture2D snapshot = new Texture2D(width, height, TextureFormat.RGBA32, false);
        currentCamera.Render();
        RenderTexture.active = currentCamera.targetTexture;
        snapshot.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
        
        byte[] bytes = snapshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fileName, bytes);
    }

    private void Awake()
    {
        width = renderTextureParametersForBuilding.width;
        height = renderTextureParametersForBuilding.height;
        depth = renderTextureParametersForBuilding.depth;

        currentCamera.targetTexture = new RenderTexture(width, height, depth);
    }
}
