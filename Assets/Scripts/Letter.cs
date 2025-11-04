using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Letter : MonoBehaviour
{
    public char letter;
    public bool isTargetLetter = false;

    private Renderer[] renderers;
    private Color originalColor;

    private void Awake()
    {
        // Сохраняем все рендереры, чтобы можно было подсветить всю модель
        renderers = GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
            originalColor = renderers[0].material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameController gc = FindObjectOfType<GameController>();
        if (gc != null)
        {
            gc.OnLetterCollected(this); // передаём сам объект буквы
        }

        Destroy(gameObject);
    }

    public void LightOn()
    {
        foreach (var r in renderers)
        {
            r.material.color = Color.yellow;
        }

        Debug.Log($"✨ Highlight ON для буквы '{letter}'");
    }

    public void LightOff()
    {
        foreach (var r in renderers)
        {
            r.material.color = originalColor;
        }

        Debug.Log($"💡 Highlight OFF для буквы '{letter}'");
    }
}
