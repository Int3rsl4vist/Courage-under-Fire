using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

public class ShootingTarget : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    public float animationSpeed = 5f;
    public bool isUp = false;

    [Header("Audio")]
    public AudioSource targetAudio;
    public AudioClip[] hitSounds;

    private Quaternion _downRotation;
    private Quaternion _upRotation;
    private TargetSequenceManager _myManager;
    private bool _isMoving = false;

    private void Awake()
    {
        _upRotation = transform.localRotation;
        _downRotation = _upRotation * Quaternion.Euler(90, 0, 0);

        if (!isUp) 
            transform.localRotation = _downRotation;
    }
    public void Setup(TargetSequenceManager manager)
    {
        _myManager = manager;
    }
    public void PopUp()
    {
        if (isUp) return;
        StartCoroutine(AnimateMotion(_upRotation));
        isUp = true;
    }
    public void TakeDamage(float amount)
    {
        if (!isUp || _isMoving) return;

        Debug.Log("CODE_LOG: Target hit");

        if (targetAudio != null && hitSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, hitSounds.Length);
            targetAudio.pitch = Random.Range(0.9f, 1.1f);
            targetAudio.PlayOneShot(hitSounds[randomIndex]); 
        }

        StartCoroutine(AnimateMotion(_downRotation));
        isUp = false;

        if (_myManager != null)
            _myManager.TargetHit(this);
    }
    IEnumerator AnimateMotion(Quaternion targetRot)
    {
        _isMoving = true;
        float t = 0;
        Quaternion startRot = transform.localRotation;

        while (t < 1)
        {
            t += Time.deltaTime * animationSpeed;
            transform.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        transform.localRotation = targetRot;
        _isMoving = false;
    }
}
