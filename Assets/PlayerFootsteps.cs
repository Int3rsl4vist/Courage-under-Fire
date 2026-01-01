using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource footstepSource;
    public AudioClip[] stepSounds;

    [Header("Settings:")]
    public float stepInterval = 0.3f;
    public float runMultiplier = 0.7f;

    private CharacterController cc;
    private float stepTimer;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        if(footstepSource == null)
            footstepSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (cc.velocity.magnitude > 0.1f && cc.isGrounded)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                PlayStep();
                stepTimer = stepInterval;
            }
        }
        else
            stepTimer = 0;
    }
    void PlayStep()
    {
        if (stepSounds.Length == 0)
            return;

        int i = Random.Range(0, stepSounds.Length);

        footstepSource.pitch = Random.Range(0.9f, 1.1f);
        footstepSource.volume = Random.Range(0.2f, 0.5f);

        footstepSource.PlayOneShot(stepSounds[i]);
    }
}
