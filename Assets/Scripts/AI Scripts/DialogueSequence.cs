using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueSequence : MonoBehaviour
{
    [System.Serializable]
    public struct DialogueLine
    {
        public DialogueSpeaker speaker;
        public AudioClip voiceLine;
        [Tooltip("Additional delay after the voice line finishes before the next line starts.")]
        public float delayAfter;
    }

    [Header("Conversation Sequence:")]
    public DialogueLine[] lines;

    [Header("Trigger Settings:")]
    public bool trigerOnCollision = true;
    public bool playOnlyOnce = true;

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!trigerOnCollision || !other.CompareTag("Player")) return;
        PlaySequence();
    }
    public void PlaySequence()
    {
        if(playOnlyOnce && hasPlayed) return;
        if(lines.Length > 0)
        {
            hasPlayed = true;
            StartCoroutine(RunSequence());
        }
    }
    private IEnumerator RunSequence()
    {
        foreach (DialogueLine line in lines)
        {
            if (line.speaker == null || line.voiceLine == null) continue;

            Debug.Log($"NPC '{gameObject.name}' is speaking line '{line.voiceLine}'");
            line.speaker.Speak(line.voiceLine);
            yield return new WaitForSeconds(line.voiceLine.length + line.delayAfter);
        }
    }
}