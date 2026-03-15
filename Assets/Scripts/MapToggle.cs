using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapToggle : MonoBehaviour
{
    [Header("Object to toggle:")]
    public GameObject mapPanel;

    public static bool IsOpen {  get; private set; }

    private void Start()
    {
        if(mapPanel != null)
        {
            mapPanel.SetActive(false);
            IsOpen = false;
        }
    }
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
            ToggleMap();
    }
    private void ToggleMap()
    {
        if(mapPanel == null) return;

        IsOpen = !mapPanel.activeSelf;
        mapPanel.SetActive(IsOpen);

        if (IsOpen)
        {
            Debug.Log($"CODE_LOG: Map open, weapons locked");
        }
        else
            Debug.Log($"CODE_LOG: Map hidden, weapons active");
    }
}