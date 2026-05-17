using UnityEngine;

public class BossMaterialAura : MonoBehaviour
{
    [Header("Boss Renderers")]
    public Renderer[] bossRenderers;

    [Header("Change Settings")]
    public float changeInterval = 3f;
    public float emissionPower = 2.5f;

    private float timer;

    private Color[] auraColors =
    {
        new Color(1f, 0.1f, 0.1f),
        new Color(0.1f, 0.4f, 1f),
        new Color(0.1f, 1f, 0.3f)
    };

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
        Color selectedColor = auraColors[Random.Range(0, auraColors.Length)];

        foreach (Renderer r in bossRenderers)
        {
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
                    Color mixedColor = Color.Lerp(baseColor, selectedColor, 0.1f);
                    mat.SetColor("_BaseColor", mixedColor);
                }
                else if (mat.HasProperty("_Color"))
                {
                    Color baseColor = mat.GetColor("_Color");
                    Color mixedColor = Color.Lerp(baseColor, selectedColor, 0.1f);
                    mat.SetColor("_Color", mixedColor);
                }
            }
        }
    }
}