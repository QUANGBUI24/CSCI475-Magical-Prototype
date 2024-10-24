using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{ 
    [SerializeField] Rigidbody2D _playerRigidBody;  // Reference to the Rigidbody2D component for applying physics-based movement
    [SerializeField] Animator _playerAnimator;      // Reference to the Animator component to handle player animations
    [SerializeField] Texture2D _crosshairTexture;
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] Transform _gun;
    [SerializeField] GameObject _player;

    WeaponStat _weaponStat;
    PlayerStats _playerStat;
    Vector2 _moveInput;          // Stores the input for player movement (horizontal and vertical axis)
    float _nextFireTime = 0f; // Tracks when the gun can shoot next
    float _lastHorizontal = 0f;  // Stores the last non-zero horizontal input value (for direction when idle)
    float _lastVertical = -1f;   // Stores the last non-zero vertical input value (default facing down)
    float _bulletSpeed;
    float _fireRate;
    float _gunDamage;
    float _playerMoveSpeed;

    void Start()
    {
        _weaponStat = _gun.GetComponent<WeaponStat>();
        _playerStat = _player.GetComponent<PlayerStats>();
        _bulletSpeed = _weaponStat.GetAmmoSpeed();
        _fireRate = _weaponStat.GetWeaponFireRate();
        _gunDamage = _weaponStat.GetWeaponDamage();
        _playerMoveSpeed = _playerStat.GetPlayerMoveSpeed();
        // Change the cursor to the crosshair and hide the default system cursor
        Cursor.SetCursor(_crosshairTexture, Vector2.zero, CursorMode.Auto);
    }

    void Update()
    {
        UpdatePlayerAnimationStat();
        UpdateStats();
    }

    /**
    * Keep updating player's speed, direction for movement and idle
    */
    void UpdatePlayerAnimationStat()
    {
        if (_playerAnimator != null)
        {
            if (_moveInput.sqrMagnitude > 0)
            {
                _lastHorizontal = _moveInput.x;
                _lastVertical = _moveInput.y;

                _playerAnimator.SetFloat("Horizontal", _lastHorizontal);
                _playerAnimator.SetFloat("Vertical", _lastVertical);
            }
            else
            {
                _playerAnimator.SetFloat("Horizontal", _lastHorizontal);
                _playerAnimator.SetFloat("Vertical", _lastVertical);
            }

            _playerAnimator.SetFloat("Speed", _moveInput.sqrMagnitude);
        }

        if (!_playerAnimator.IsInTransition(0))
        {
            AnimatorStateInfo currentState = _playerAnimator.GetCurrentAnimatorStateInfo(0);

            if (currentState.IsName("StabAnimationBlendTree") && currentState.normalizedTime >= 1.0f)
            {
                _playerAnimator.SetBool("isStabbing", false);
            }
            ResetShootingAnimation(currentState);
        }
    }

    void UpdateStats()
    {
        _bulletSpeed = _weaponStat.GetAmmoSpeed();
        _fireRate = _weaponStat.GetWeaponFireRate();
        _gunDamage = _weaponStat.GetWeaponDamage();
        _playerMoveSpeed = _playerStat.GetPlayerMoveSpeed();
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

    private void OnFire(InputValue inputValue)
    {
        if (Time.time >= _nextFireTime)
        {
            if (inputValue.isPressed)
            {
                // Handle shooting logic...
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;

                Vector2 direction = (mousePosition - transform.position).normalized;

                RotatePlayerSprite(direction);

                GameObject bullet = Instantiate(_bulletPrefab, _gun.position, Quaternion.identity);

                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    bulletRb.velocity = direction * _bulletSpeed;
                }
            }
            _nextFireTime = Time.time + _fireRate;
        }
    }

    private void RotatePlayerSprite(Vector2 direction)
    {
        float threshold = 0.001f; // Set a threshold to give vertical direction priority when needed

        // Prioritize shooting up or down when vertical movement is significant
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x) + threshold)
        {
            if (direction.y > 0)
            {
                _playerAnimator.SetBool("isShootingUp", true);
            }
            else if (direction.y < 0)
            {
                _playerAnimator.SetBool("isShootingDown", true);
            }
        }
        else // Otherwise, prioritize horizontal movement
        {
            if (direction.x > 0)
            {
                _playerAnimator.SetBool("isShootingRight", true);
            }
            else if (direction.x < 0)
            {
                _playerAnimator.SetBool("isShootingLeft", true);
            }
        }
    }

    private void ResetShootingAnimation(AnimatorStateInfo currentState)
    {
        if (currentState.IsName("ShootRight") && currentState.normalizedTime >= 1.0f)
        {
            _playerAnimator.SetBool("isShootingRight", false);
        }
        if (currentState.IsName("ShootLeft") && currentState.normalizedTime >= 1.0f)
        {
            _playerAnimator.SetBool("isShootingLeft", false);
        }
        if (currentState.IsName("ShootUp") && currentState.normalizedTime >= 1.0f)
        {
            _playerAnimator.SetBool("isShootingUp", false);
        }
        if (currentState.IsName("ShootDown") && currentState.normalizedTime >= 1.0f)
        {
            _playerAnimator.SetBool("isShootingDown", false);
        }
    }
}
