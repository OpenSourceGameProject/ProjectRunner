using UnityEngine;

public class BossAuraRandomizer : MonoBehaviour
{
    public Renderer auraRenderer;

    [Header("Change Settings")]
    public float changeInterval = 3f;

    [Header("Aura Colors")]
    public Color redAura = new Color(1f, 0.1f, 0.1f, 0.35f);
    public Color blueAura = new Color(0.1f, 0.4f, 1f, 0.35f);
    public Color greenAura = new Color(0.1f, 1f, 0.3f, 0.35f);

    private Material auraMaterial;
    private float timer;

    private void Start()
    {
        if (auraRenderer == null)
            auraRenderer = GetComponent<Renderer>();

        auraMaterial = auraRenderer.material;
        ChangeAuraRandomly();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= changeInterval)
        {
            timer = 0f;
            ChangeAuraRandomly();
        }
    }

    private void ChangeAuraRandomly()
    {
        int randomIndex = Random.Range(0, 3);

        if (randomIndex == 0)
            ApplyAuraColor(redAura);
        else if (randomIndex == 1)
            ApplyAuraColor(blueAura);
        else
            ApplyAuraColor(greenAura);
    }

    private void ApplyAuraColor(Color color)
    {
        auraMaterial.color = color;

        if (auraMaterial.HasProperty("_BaseColor"))
            auraMaterial.SetColor("_BaseColor", color);

        if (auraMaterial.HasProperty("_EmissionColor"))
            auraMaterial.SetColor("_EmissionColor", color * 3f);
    }
}