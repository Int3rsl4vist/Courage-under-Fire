using UnityEngine;
using UnityEngine.Events;

public class StageTrigger : MonoBehaviour
{
    [Header("What should happen when player enters trigger:")]
    public UnityEvent onPlayerEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player detected near '{gameObject.name}', engaging starter events");
            onPlayerEnter?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
