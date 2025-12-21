using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target; // Játékos transform
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);
    public bool followOnStart = true;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();

        if (target == null && GameManager2D.Instance != null && GameManager2D.Instance.player != null)
        {
            target = GameManager2D.Instance.player.transform;
            Debug.Log("[CameraFollow2D] Játékos automatikusan becélozva");
        }

        if (target == null)
        {
            Debug.LogWarning("[CameraFollow2D] Nincs célpont.");
        }

        // Ha nincs offset beállítva, használjuk a jelenlegi pozíciót
        if (offset.z == 0 && cam != null)
        {
            offset = new Vector3(0, 0, transform.position.z);
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        if (followOnStart || GameManager2D.Instance == null ||
            GameManager2D.Instance.currentState == GameManager2D.GameState.Playing)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

    [ContextMenu("Set Target to Player")]
    private void SetTargetToPlayer()
    {
        if (GameManager2D.Instance != null && GameManager2D.Instance.player != null)
        {
            target = GameManager2D.Instance.player.transform;
            Debug.Log("[CameraFollow2D] Célpont a játékos");
        }
        else
        {
            Debug.LogWarning("[CameraFollow2D] Játékos nem található!");
        }
    }
}

