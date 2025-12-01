using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Player2D player2D;

    private void Start()
    {
        player2D = GetComponent<Player2D>();
        if (player2D == null)
        {
            Debug.LogError("PlayerController2D requires Player2D component!");
        }
    }

    private void Update()
    {
        if (player2D == null) return;

        if (GameManager2D.Instance != null &&
            GameManager2D.Instance.currentState != GameManager2D.GameState.Playing)
        {
            return;
        }

        HandleSmoothMovement();
    }

    // Gombok bekötése a mozgáshoz
    private void HandleSmoothMovement()
    {
        Vector2 input = Vector2.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) input.y += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) input.y -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) input.x -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) input.x += 1f;

        if (input != Vector2.zero)
        {
            input.Normalize();
            Vector3 movement = new Vector3(input.x, input.y, 0) * moveSpeed * Time.deltaTime;
            transform.position += movement;

            if (player2D != null)
            {
                player2D.position = Vector2Int.RoundToInt(transform.position);
            }
        }
    }
}
