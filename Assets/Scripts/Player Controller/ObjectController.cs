using System.Collections;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [Header("Settings")]
    public Animator animator;
    public string openTrigger = "Open";
    public string closeTrigger = "Close";

    public string openStateName = "DoorOpen";
    public string closeStateName = "DoorClose";
    public bool mirrorAnimation = false;

    [Header("Auto-Close")]
    public bool enableAutoClose = false;
    public float autoCloseDelay = 5f;

    [Header("Optimalization")]
    public OcclusionPortal portal;

    private Coroutine _timer;
    private void Awake()
    {
        if(animator == null)
            animator = GetComponent<Animator>();
        if(portal  == null)
            portal = GetComponent<OcclusionPortal>();
        /*if(animator != null)
            animator.SetBool("Mirror", mirrorAnimation);*/
    }
    public void Interact()
    {
        if (animator == null)
            return;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(openStateName) || animator.GetCurrentAnimatorStateInfo(0).IsName("CrateOpen"))
        {
            Close();
        }
        else
            Open();
    }
    public void Open()
    {
        if (animator == null)
            return;
        animator.ResetTrigger(closeTrigger);
        animator.SetTrigger(openTrigger);
        if (portal != null)
            portal.open = true;
        if (enableAutoClose)
        {
            if (_timer != null)
                StopCoroutine(_timer);
            _timer = StartCoroutine(AutoCloseCoroutine());
        }
    }
    public void Close()
    {
        if (animator == null)
            return;
        if(_timer != null)
        {
            StopCoroutine(_timer);
            _timer = null;
        }
        animator.ResetTrigger(openTrigger);
        animator.SetTrigger(closeTrigger);
        if(portal != null)
            portal.open = false;
    }
    private IEnumerator AutoCloseCoroutine()
    {
        yield return new WaitForSeconds(autoCloseDelay);

        Close();
    }
}
