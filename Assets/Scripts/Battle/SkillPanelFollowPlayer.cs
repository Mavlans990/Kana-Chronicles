using UnityEngine;

public class SkillPanelFollowPlayer : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    public Camera mainCamera;
    public Canvas canvas;

    [Header("Offset posisi tombol dari player")]
    public Vector2 offset = new Vector2(120f, 80f);

    private RectTransform rectTransform;
    private RectTransform canvasRectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        if (canvas != null)
        {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }
    }

    void LateUpdate()
    {
        if (player == null || mainCamera == null || canvasRectTransform == null)
            return;

        Vector2 screenPosition = mainCamera.WorldToScreenPoint(player.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            screenPosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out Vector2 localPoint
        );

        rectTransform.anchoredPosition = localPoint + offset;
    }
}