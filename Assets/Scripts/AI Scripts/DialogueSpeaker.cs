using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct VoiceLineData
{
    public AudioClip clip;
    [Tooltip("Delay after voiceline (seconds)")]
    public float delayAfter;
}

[RequireComponent(typeof(AudioSource))]
public class DialogueSpeaker : MonoBehaviour
{
    [Header("NPC Voicelines:")]
    public VoiceLineData[] voiceLines;

    [Header("Events:")]
    public UnityEvent onSpeechComplete;

    private AudioSource _audioSource;
    private Coroutine _currentSpeech;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.spatialBlend = 1f;
    }
    public void Speak(AudioClip clip)
    {
        if (clip == null) return;
        StopCurrentSpeech();

        _audioSource.clip = clip;
        _audioSource.Play();
    }
    public void SpeakAllLines()
    {
        if(voiceLines == null || voiceLines.Length == 0) return;
        StopCurrentSpeech();

        _currentSpeech = StartCoroutine(SequenceCoroutine());
    }
    public void SpeakRandomLine()
    {
        if(voiceLines == null || voiceLines.Length == 0) return;
        StopCurrentSpeech();
        int randomIndex = Random.Range(0, voiceLines.Length);
        AudioClip randomClip = voiceLines[randomIndex].clip;

        if(randomClip != null)
        {
            _audioSource.clip = randomClip;
            _audioSource.Play();
        }
    }
    public void ShutUp()
    {
        StopCurrentSpeech();
    }
    private void StopCurrentSpeech()
    {
        if (_currentSpeech != null)
        {
            StopCoroutine(_currentSpeech);
            _currentSpeech = null;
        }
        _audioSource.Stop();
    }
    private IEnumerator SequenceCoroutine()
    {
        foreach (var lineData in voiceLines)
        {
            if(lineData.clip == null) continue;

            Debug.Log($"CODE_LOG: NPC '{gameObject.name}' is speaking line '{lineData.clip.name}'");
            _audioSource.clip = lineData.clip;
            _audioSource.Play();

            yield return new WaitForSeconds(lineData.clip.length + lineData.delayAfter);
        }
        _currentSpeech = null;
        onSpeechComplete?.Invoke();
        Debug.Log("CODE_LOG: OnSpeechComplete invoked");
    }
}