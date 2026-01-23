using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Návrhový vzor Jedináèek
    public static AudioManager Instance;

    [Header("Music:")]
    public AudioSource musicSource;

    [Header("SFX:")]
    public AudioSource sfxSource;

    [Header("Clips:")]
    public AudioClip objectiveCompleteClip;
    public AudioClip objectiveFailedClip;
    public AudioClip missionCompleteClip;
    public AudioClip missionFailClip;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(gameObject);
    }

    public void PlaySFX(AudioClip clip)
    {
        // Pøehrání urèitého klipu + ošetøení existence klipu
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }
    // Pomocné metody, použitelné ostatními skripty, pøedevším skriptem MissionManager
    public void PlayObjectiveComplete() => PlaySFX(objectiveCompleteClip);
    public void PlayObjectiveFail() => PlaySFX(objectiveFailedClip);
    public void PlayMissionComplete() => PlaySFX(missionCompleteClip);
    public void PlayMissionFail() => PlaySFX(missionFailClip);
}
