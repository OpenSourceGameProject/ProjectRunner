using UnityEngine;

public class BossMaterialAura : MonoBehaviour
{
    [Header("Boss Renderers")]
    public Renderer[] bossRenderers;

    [Header("Change Settings")]
    public float changeInterval = 3f;
    public float emissionPower = 2.5f;

    [Header("Current State")]
    public TrackColorType currentColor = TrackColorType.Green;

    public TrackColorType CurrentColor => currentColor;

    private float timer;

    private void Start()
    {
        if (bossRenderers == null || bossRenderers.Length == 0)
            bossRenderers = GetComponentsInChildren<Renderer>();

        ChangeAura();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= changeInterval)
        {
            timer = 0f;
            ChangeAura();
        }
    }

    private void ChangeAura()
    {
        int randomIndex = Random.Range(0, 3);

        if (randomIndex == 0)
            currentColor = TrackColorType.Red;
        else if (randomIndex == 1)
            currentColor = TrackColorType.Green;
        else
            currentColor = TrackColorType.Blue;

        ApplyColor(GetUnityColor(currentColor));
    }

    private Color GetUnityColor(TrackColorType colorType)
    {
        switch (colorType)
        {
            case TrackColorType.Red:
                return new Color(1f, 0.1f, 0.1f);

            case TrackColorType.Green:
                return new Color(0.1f, 1f, 0.3f);

            case TrackColorType.Blue:
                return new Color(0.1f, 0.4f, 1f);

            default:
                return Color.white;
        }
    }

    private void ApplyColor(Color selectedColor)
    {
        foreach (Renderer r in bossRenderers)
        {
            if (r == null)
                continue;

            foreach (Material mat in r.materials)
            {
                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", selectedColor * emissionPower);
                }

                if (mat.HasProperty("_BaseColor"))
                {
                    Color baseColor = mat.GetColor("_BaseColor");
                    Color mixedColor = Color.Lerp(baseColor, selectedColor, 0.25f);
                    mat.SetColor("_BaseColor", mixedColor);
                }
                else if (mat.HasProperty("_Color"))
                {
                    Color baseColor = mat.GetColor("_Color");
                    Color mixedColor = Color.Lerp(baseColor, selectedColor, 0.25f);
                    mat.SetColor("_Color", mixedColor);
                }
            }
        }
    }
}