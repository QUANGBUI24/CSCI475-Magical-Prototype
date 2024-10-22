using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] float _playerMoveSpeed = 5f;   // Player's movement speed, adjustable in the Inspector
    [SerializeField] Rigidbody2D _playerRigidBody;  // Reference to the Rigidbody2D component for applying physics-based movement
    [SerializeField] Animator _playerAnimator;      // Reference to the Animator component to handle player animations
    [SerializeField] Texture2D crosshairTexture;

    Vector2 _moveInput;          // Stores the input for player movement (horizontal and vertical axis)
    float _lastHorizontal = 0f;  // Stores the last non-zero horizontal input value (for direction when idle)
    float _lastVertical = -1f;   // Stores the last non-zero vertical input value (default facing down)

    void Start()
    {
        // Change the cursor to the crosshair and hide the default system cursor
        Cursor.SetCursor(crosshairTexture, Vector2.zero, CursorMode.Auto);
    }

    void Update()
    {
        UpdatePlayerAnimationStat();
        // Check if the player is currently in the stab blend tree state
        if (_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("StabAnimationBlendTree"))
        {
            // Check if the animation has finished playing (normalizedTime >= 1.0 means 100% completion)
            if (_playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                _playerAnimator.SetBool("isStabbing", false); // Reset after the animation finishes
            }
        }
    }

    /**
    * Keep updating player's speed, direction for movement and idle
    */
    void UpdatePlayerAnimationStat()
    {
        if (_playerAnimator != null)
        {
            if (_moveInput.sqrMagnitude > 0) // Check if the player is moving
            {
                // Update last direction based on current input
                _lastHorizontal = _moveInput.x;
                _lastVertical = _moveInput.y;

                // Set animator parameters based on the current movement direction
                _playerAnimator.SetFloat("Horizontal", _lastHorizontal);
                _playerAnimator.SetFloat("Vertical", _lastVertical);
            }
            else
            {
                // When the player is idle, retain the last movement direction for idle animation
                _playerAnimator.SetFloat("Horizontal", _lastHorizontal);
                _playerAnimator.SetFloat("Vertical", _lastVertical);
            }
            // Set the speed parameter in the Animator, which controls the transition between idle and movement states
            _playerAnimator.SetFloat("Speed", _moveInput.sqrMagnitude);
        }

        // Check if the Animator is not transitioning to or from another state
        if (!_playerAnimator.IsInTransition(0))
        {
            // Check if the player is currently in the stab blend tree state
            AnimatorStateInfo currentState = _playerAnimator.GetCurrentAnimatorStateInfo(0);

            // Make sure the current state is the StabBlendTree (replace "StabBlendTree" with your actual state name)
            if (currentState.IsName("StabBlendTree"))
            {
                // Check if the animation has finished playing (normalizedTime >= 1.0 means 100% completion)
                if (currentState.normalizedTime >= 1.0f)
                {
                    _playerAnimator.SetBool("isStabbing", false); // Reset after the animation finishes
                }
            }
        }
    }

    /** 
    * Called on a regular interval by Unity for physics calculations.
    * Handles physics-based movement logic by applying velocity to the Rigidbody2D.
    */
    void FixedUpdate()
    {
        Run();
    }

    /**
    * Applies the player's movement input to the Rigidbody2D's velocity.
    * This results in the player moving in the direction of the input with a speed based on _playerMoveSpeed.
    */
    private void Run()
    {
        // Normalize the input to ensure diagonal movement isn't faster
        Vector2 playerVelocity = _moveInput.normalized * _playerMoveSpeed;

        // Apply the calculated velocity to the Rigidbody2D to move the player
        _playerRigidBody.velocity = playerVelocity;
    }

    /**
    * Called by the Unity Input System when movement input is received.
    * Stores the input value as a Vector2, which represents horizontal and vertical movement.
    * @param inputValue - The movement input provided by the Input System.
    */
    private void OnMove(InputValue inputValue)
    {
        // Capture and store the movement input as a Vector2 (x and y axes)
        _moveInput = inputValue.Get<Vector2>();
    }

    /**
    * Called by the Unity Input System when the stab input (F key) is pressed.
    * Triggers the stab animation once.
    * @param inputValue - The stab input provided by the Input System.
    */
    private void OnStab(InputValue inputValue)
    {
        if (inputValue.isPressed)
            _playerAnimator.SetBool("isStabbing", true);
    }
}
