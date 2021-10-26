using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BirdController : MonoBehaviour {
    public float maxVelocity = 12.5f;
    public float flapVelocity = 7.5f;
    public float rotateFactor = .5f;
    public Transform specular;
    public AudioClip flapSound;
    public AudioClip scoreSound;
    public AudioClip hitSound;
    
    Animator _animator;
    Animator _animatorSpec;
    AudioSource _audioSource;
    GameManager _gameManager;
    PlayerInput _playerInput;
    Rigidbody2D _rigidbody2D;
    SpriteRenderer _spriteRenderer;
    SpriteRenderer _spriteRendererSpec;

    void Awake() {
        // Get references
        _animator = GetComponent<Animator>();
        _animatorSpec = specular.GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _gameManager = FindObjectOfType<GameManager>();
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRendererSpec = specular.GetComponent<SpriteRenderer>();
    }

    void Start() {
    }

    void FixedUpdate() {
        // Face apparent movement direction
        if (_playerInput.enabled == true) {
            float targetRotation = Mathf.Atan2(_rigidbody2D.velocity.y, 10f) * Mathf.Rad2Deg;
            _rigidbody2D.rotation = Mathf.LerpAngle(_rigidbody2D.rotation, targetRotation, rotateFactor);
            specular.rotation = Quaternion.AngleAxis(
                    -Mathf.LerpAngle(_rigidbody2D.rotation, 0f, .75f),
                    Vector3.forward
            );
        }
    }

    void Update() {
        // Limit max velocity
        _rigidbody2D.velocity = Vector2.ClampMagnitude(_rigidbody2D.velocity, maxVelocity);

        // Move specular
        if (_spriteRenderer.enabled) {
            specular.position = new Vector3(
                    transform.position.x,
                    -1f - transform.position.y / 2f,
                    transform.position.z
            );
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (_gameManager.gameState == GameState.Playing) {
            if (other.CompareTag("Checkpoint")) {
                // Score
                _gameManager.currentScore += 1;
                _audioSource.PlayOneShot(scoreSound);
            } else if (other.CompareTag("Obstacle")) {
                DoGameOver();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (_gameManager.gameState == GameState.Playing) {
            if (other.collider.CompareTag("Obstacle")) {
                DoGameOver();
            }
        }
    }

    // Player input callback
    public void OnTouch(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            Flap();
        }
    }

    // Back to menu callback
    public void OnMenu() {
        _spriteRenderer.enabled = false;
        _spriteRendererSpec.enabled = false;
    }

    // New game callback
    public void OnReady() {
        _spriteRenderer.enabled = true;
        _spriteRendererSpec.enabled = true;
        Reset();
    }

    // Game over callback
    public void OnGameOver() {
        _playerInput.enabled = false;
        _animator.speed = 0f;
        _animatorSpec.speed = 0f;
    }

    ///////////////////////////////////////////////////////

    void DoGameOver() {
        _gameManager.gameState = GameState.GameOver;
        _rigidbody2D.velocity = 5f * Vector2.up;
        _audioSource.PlayOneShot(hitSound);
    }

    void Flap() {
        if (!_rigidbody2D.simulated) {
            _gameManager.gameState = GameState.Playing;
            _rigidbody2D.simulated = true;
            _animator.speed = 1f;
            _animatorSpec.speed = 1f;
        }
        _rigidbody2D.velocity = Mathf.Max(flapVelocity, _rigidbody2D.velocity.y + flapVelocity) * Vector2.up;
        _audioSource.PlayOneShot(flapSound, 0.5f);
    }

    void Reset() {
        transform.position = new Vector2(-2f, 6f);
        transform.rotation = Quaternion.identity;
        specular.position = new Vector3(
            transform.position.x,
            -1f - transform.position.y / 2f,
            transform.position.z
        );
        specular.rotation = Quaternion.identity;
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.angularVelocity = 0f;
        _rigidbody2D.simulated = false;
        _playerInput.enabled = true;
        _animator.speed = 0f;
        _animatorSpec.speed = 0f;
    }
}
