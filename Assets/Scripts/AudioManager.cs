using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Ambient & Music")]
    [SerializeField] private AudioSource ambientSource;    // ambient
    [SerializeField] private AudioSource musicSource;      // музыка и пауза
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip pausedMusicClip;
    [SerializeField] private AudioClip ambientClip;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource sfxSource;       // общий для всех эффектов
    [SerializeField] private AudioClip swooshClip;
    [SerializeField] private AudioClip[] bubleClips;
    [SerializeField] private AudioClip[] clickButtonClips;
    [SerializeField] private AudioClip countinueGameClip;
    [SerializeField] private AudioClip exitClip;
    [SerializeField] private AudioClip damageTakenClip;
    [SerializeField] private AudioClip[] frogClips;
    [SerializeField] private AudioClip impactClip;
    [SerializeField] private AudioClip landingClip;
    [SerializeField] private AudioClip letterCorrectClip;
    [SerializeField] private AudioClip letterWrongClip;
    [SerializeField] private AudioClip revelationClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip loseClip;

    [Header("Movement Sounds")]
    [SerializeField] private AudioClip smoothFromCenterToLeftClip;
    [SerializeField] private AudioClip smoothFromCenterToRightClip;
    [SerializeField] private AudioClip smoothFromLeftToCenterClip;
    [SerializeField] private AudioClip smoothFromRightToCenterClip;

    private void Start()
    {
        if (ambientClip != null)
        {
            ambientSource.clip = ambientClip;
            ambientSource.loop = true;
            ambientSource.Play();
        }

        if (musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // -----------------------------
    // Music control
    // -----------------------------
    public void PlayMusic() => SwitchMusic(musicClip);
    public void PlayPausedMusic() => SwitchMusic(pausedMusicClip);

    private void SwitchMusic(AudioClip clip)
    {
        if (clip == null || musicSource.clip == clip) return;
        StartCoroutine(FadeToNewMusic(clip, 0.5f));
    }

    private System.Collections.IEnumerator FadeToNewMusic(AudioClip newClip, float duration)
    {
        float startVolume = musicSource.volume;
        // затухание старой
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }
        musicSource.volume = 0f;
        musicSource.clip = newClip;
        musicSource.Play();
        // увеличение новой
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, startVolume, t / duration);
            yield return null;
        }
        musicSource.volume = startVolume;
    }

    // -----------------------------
    // Sound Effects (new sound interrupts previous)
    // -----------------------------
    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.Stop();
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    private void PlayRandomSFX(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;
        AudioClip chosen = clips[Random.Range(0, clips.Length)];
        PlaySFX(chosen);
    }

    // -----------------------------
    // Public methods for gameplay
    // -----------------------------
    public void PlaySwoosh() => PlaySFX(swooshClip);
    public void PlayBuble()
    {
        Debug.Log("sound");
        PlayRandomSFX(bubleClips);
    }
    public void PlayClickButton() => PlayRandomSFX(clickButtonClips);
    public void PlayContinueGameButton() => PlaySFX(countinueGameClip);
    public void PlayExitButton() => PlaySFX(exitClip);
    public void PlayDamageTaken() => PlaySFX(damageTakenClip);
    public void PlayFrogSound() => PlayRandomSFX(frogClips);
    public void PlayImpact() => PlaySFX(impactClip);
    public void PlayLanding() => PlaySFX(landingClip);
    public void PlayJump() => PlaySFX(jumpClip);
    public void PlayLose() => PlaySFX(loseClip);
    public void PlayLetterCorrect() => PlaySFX(letterCorrectClip);
    public void PlayLetterWrong() => PlaySFX(letterWrongClip);
    public void PlayRevelation() => PlaySFX(revelationClip);

    public void PlaySmoothFromCenterToLeft() => PlaySFX(smoothFromCenterToLeftClip);
    public void PlaySmoothFromCenterToRight() => PlaySFX(smoothFromCenterToRightClip);
    public void PlaySmoothFromLeftToCenter() => PlaySFX(smoothFromLeftToCenterClip);
    public void PlaySmoothFromRightToCenter() => PlaySFX(smoothFromRightToCenterClip);
}
