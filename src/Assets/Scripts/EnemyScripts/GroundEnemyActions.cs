using System.Collections;
using UnityEngine;

public class GroundEnemyActions : MonoBehaviour
{
    [Header("Enemy Movement")]
    [SerializeField] float _enemyMoveSpeed = 5f; // Enemy's movement speed
    [SerializeField] Rigidbody2D _enemyRigidBody; // Rigidbody2D for movement
    [SerializeField] Animator _enemyAnimator;    // Animator component for animations
    [SerializeField] float _raycastDistance = 1f; // Distance for detecting obstacles with Raycast
    [SerializeField] LayerMask obstacleLayer; // Layer mask to detect obstacles

    [Header("Idle and Movement Timers")]
    [Range(1f, 10f)][SerializeField] float idleTimeMin = 1f; // Minimum idle time
    [Range(1f, 10f)][SerializeField] float idleTimeMax = 3f; // Maximum idle time
    [Range(1f, 10f)][SerializeField] float moveTimeMin = 2f; // Minimum move time
    [Range(1f, 10f)][SerializeField] float moveTimeMax = 5f; // Maximum move time

    [Header("Player Detection")]
    [SerializeField] Transform _playerTransform;   // Reference to the player
    [SerializeField] float detectionRadius = 3f;   // Radius to detect the player

    private Vector2 _movementDirection;    // Stores the current movement direction
    private bool _isMoving = false;        // Tracks if the enemy is moving
    private float _changeStateTimer = 0f;  // Timer to switch between idle/moving
    private float _currentStateDuration = 0f; // Duration of the current state
    private bool _isChasingPlayer = false; // Tracks if the enemy is chasing the player

    void Start()
    {
        ChangeState(); // Set initial state (either idle or moving)
    }

    void Update()
    {
        UpdateStateTimer();
        DetectPlayer();
        UpdateAnimation(); // Update animations based on velocity
    }

    void FixedUpdate()
    {
        if (_isChasingPlayer)
        {
            MoveTowardsPlayer(); // Chase the player
        }
        else
        {
            AutoMove(); // Continue wandering
        }
    }

    // Updates the timer for changing between idle and moving states
    private void UpdateStateTimer()
    {
        _changeStateTimer += Time.deltaTime;

        if (!_isChasingPlayer && _changeStateTimer >= _currentStateDuration)
        {
            ChangeState();  // Change the enemy's state (idle or moving)
        }
    }

    // Detects if the player is within the detection radius
    private void DetectPlayer()
    {
        if (Vector2.Distance(transform.position, _playerTransform.position) <= detectionRadius)
        {
            _isChasingPlayer = true;  // Start chasing the player
        }
        else
        {
            _isChasingPlayer = false; // Resume wandering behavior
        }
    }

    // Changes the state between idle and moving
    private void ChangeState()
    {
        _isMoving = !_isMoving;

        if (_isMoving)
        {
            ChooseRandomDirection();
            _currentStateDuration = Random.Range(moveTimeMin, moveTimeMax); // Set how long to move
        }
        else
        {
            _currentStateDuration = Random.Range(idleTimeMin, idleTimeMax); // Set how long to stay idle
        }

        _changeStateTimer = 0f;  // Reset the state change timer
    }

    // Chooses a random movement direction
    private void ChooseRandomDirection()
    {
        int randomDirection = Random.Range(0, 8);

        switch (randomDirection)
        {
            case 0: _movementDirection = Vector2.up; break;
            case 1: _movementDirection = Vector2.down; break;
            case 2: _movementDirection = Vector2.left; break;
            case 3: _movementDirection = Vector2.right; break;
            case 4: _movementDirection = new Vector2(1, 1).normalized; break;   // Diagonal up-right
            case 5: _movementDirection = new Vector2(-1, 1).normalized; break;  // Diagonal up-left
            case 6: _movementDirection = new Vector2(1, -1).normalized; break;  // Diagonal down-right
            case 7: _movementDirection = new Vector2(-1, -1).normalized; break; // Diagonal down-left
        }
    }

    // Handles automatic movement and obstacle detection
    private void AutoMove()
    {
        if (_isMoving)
        {
            if (IsObstacleInPath())
            {
                ChooseRandomDirection(); // Pick a new direction if obstacle detected
            }
            else
            {
                _enemyRigidBody.velocity = _movementDirection * _enemyMoveSpeed;
            }
        }
        else
        {
            _enemyRigidBody.velocity = Vector2.zero; // Idle, no movement
        }
    }

    // Moves the enemy toward the player when in chase mode
    private void MoveTowardsPlayer()
    {
        Vector2 directionToPlayer = (_playerTransform.position - transform.position).normalized;

        _enemyRigidBody.velocity = directionToPlayer * _enemyMoveSpeed; // Move toward the player

        // Update the enemy's animation based on the movement direction
        _enemyAnimator.SetFloat("E_Horizontal", directionToPlayer.x);
        _enemyAnimator.SetFloat("E_Vertical", directionToPlayer.y);
        _enemyAnimator.SetFloat("E_Speed", directionToPlayer.magnitude);
    }

    // Updates the enemy's animation based on its velocity
    private void UpdateAnimation()
    {
        Vector2 velocity = _enemyRigidBody.velocity;

        if (velocity.sqrMagnitude > 0.1f)  // Check if the enemy is moving
        {
            _enemyAnimator.SetFloat("E_Horizontal", velocity.x);
            _enemyAnimator.SetFloat("E_Vertical", velocity.y);
            _enemyAnimator.SetFloat("E_Speed", velocity.magnitude);
        }
        else
        {
            _enemyAnimator.SetFloat("E_Speed", 0);  // Idle animation
        }
    }

    // Checks for obstacles in the enemy's path using a Raycast
    private bool IsObstacleInPath()
    {
        RaycastHit2D hit = Physics2D.Raycast(_enemyRigidBody.position, _movementDirection, _raycastDistance, obstacleLayer);

        Debug.DrawRay(_enemyRigidBody.position, _movementDirection * _raycastDistance, Color.red);

        return hit.collider != null;
    }
}
