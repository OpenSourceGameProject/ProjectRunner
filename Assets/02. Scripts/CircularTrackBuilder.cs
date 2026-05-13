using UnityEngine;

public class CircularTrackBuilder : MonoBehaviour
{
    [Header("Track Settings")]
    public int pieceCount = 24;
    public float radius = 12f;
    public Vector3 pieceScale = new Vector3(3f, 0.3f, 2f);
    public float height = 0.2f;

    [Header("Materials")]
    public Material redMat;
    public Material blueMat;
    public Material greenMat;
    public Material yellowMat;

    [ContextMenu("Build Circular Track")]
    public void BuildCircularTrack()
    {
        ClearOldTrack();

        for (int i = 0; i < pieceCount; i++)
        {
            float angle = 360f / pieceCount * i;
            float rad = angle * Mathf.Deg2Rad;

            float x = Mathf.Sin(rad) * radius;
            float z = Mathf.Cos(rad) * radius;

            GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
            piece.name = "TrackPiece_" + i.ToString("00");
            piece.transform.SetParent(transform);

            piece.transform.position = new Vector3(x, height, z);
            piece.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            piece.transform.localScale = pieceScale;

            Renderer renderer = piece.GetComponent<Renderer>();
            renderer.sharedMaterial = GetMaterialByIndex(i);
        }
    }

    private Material GetMaterialByIndex(int index)
    {
        int sectionSize = pieceCount / 4;

        if (index < sectionSize)
            return redMat;
        else if (index < sectionSize * 2)
            return blueMat;
        else if (index < sectionSize * 3)
            return greenMat;
        else
            return yellowMat;
    }

    private void ClearOldTrack()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}