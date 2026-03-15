using NUnit.Framework.Constraints;
using UnityEngine;

public class ClassicMap : MonoBehaviour
{
    [Header("Links:")]
    public Transform playerTransform;
    public RectTransform playerMapIcon;
    public RectTransform mapBackground;

    [Header("World Calibration (3D coordinates):")]
    [Tooltip("Coordinates of the BOTTOM LEFT corner of the map (X, Z)")]
    public Vector2 worldBottomLeft = new(-100, -100);
    [Tooltip("Coordinates of the TOP RIGHT corner of the map (X, Z)")]
    public Vector2 worldTopRight = new(100, 100);

    private void Update()
    {
        if(playerTransform == null || playerMapIcon == null || mapBackground == null) return;

        float playerX = playerTransform.position.x;
        float playerZ = playerTransform.position.z;

        float percentX = Mathf.InverseLerp(worldBottomLeft.x, worldTopRight.x, playerX);
        float percentY = Mathf.InverseLerp(worldBottomLeft.y, worldTopRight.y, playerZ);

        float mapWidth = mapBackground.rect.width;
        float mapHeight = mapBackground.rect.height;

        playerMapIcon.anchoredPosition = new Vector2(
            (percentX *  mapWidth) - mapWidth/2,
            (percentY * mapHeight) - mapHeight/2
        );
        playerMapIcon.localEulerAngles = new(0, 0, -playerTransform.eulerAngles.y);
    }
}
