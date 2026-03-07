using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource footstepSource;
    [Header("Sounds on specific surfaces:")]

    public AudioClip[] defaultSounds;
    public AudioClip[] roadSounds;
    public AudioClip[] woodSounds;
    public AudioClip[] dirtSounds;
    public AudioClip[] stoneSounds;

    [Header("Settings:")]
    public float stepInterval = 0.5f;
    public float sprintInterval = 0.3f;

    private CharacterController cc;
    private Player playerScript;
    private float stepTimer;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        playerScript = GetComponent<Player>();
        if(footstepSource == null)
            footstepSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        Vector2 horizontalVelocity = new(cc.velocity.x, cc.velocity.z);
        if (horizontalVelocity.magnitude > 0.1f && cc.isGrounded)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                PlayStep();

                if (playerScript != null && playerScript.speedMultiplier > 1.1f)
                    stepTimer = sprintInterval;
                else
                    stepTimer = stepInterval;
            }
        }
        else
            stepTimer = 0;
    }
    void PlayStep()
    {
        AudioClip[] currentClips = defaultSounds;

        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f))
        {
            switch (hit.collider.tag)
            {
                case "MAP_Roads":
                    if (roadSounds.Length > 0) currentClips = roadSounds;
                    break;
                case "MAP_Wood":
                    if(woodSounds.Length > 0) currentClips = woodSounds;
                    break;
                case "MAP_Dirt":
                    if(dirtSounds.Length > 0) currentClips = dirtSounds;
                    break;
                case "MAP_Stone":
                    if(stoneSounds.Length > 0) currentClips = stoneSounds;
                    break;
            }
        }
        if (currentClips == null || currentClips.Length == 0) return;
        
        int i = Random.Range(0, currentClips.Length);

        footstepSource.pitch = Random.Range(0.9f, 1.1f);
        footstepSource.volume = Random.Range(0.2f, 0.5f);

        footstepSource.PlayOneShot(currentClips[i]);
    }
}
