using UnityEngine;

namespace ArkansasIo.GoonzuGameEngine;

/// <summary>
/// Minimal prototype movement controller. It intentionally contains no economy or authoritative
/// gameplay rules; those belong to the server/application layer.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public sealed class GoonzuPlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4.5f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float gravity = -20f;

    private CharacterController controller;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        // Isometric presentation maps screen-like input into world X/Z axes.
        var move = new Vector3(input.x, 0f, input.y);
        if (move.sqrMagnitude > 0.001f)
        {
            controller.Move(move * (moveSpeed * Time.deltaTime));
            var targetRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        controller.Move(Vector3.up * (verticalVelocity * Time.deltaTime));
    }
}
