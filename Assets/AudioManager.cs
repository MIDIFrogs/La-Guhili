using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource ambient;
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource swoosh;

    void Start()
    {
        if (ambient != null && ambient.enabled)
        {
            ambient.Play();
        }

        if (ambient != null && ambient.enabled)
        {
            music.Play();
        }
    }

    public void PlaySwoosh()
    {
        swoosh.Play();
    }
}
