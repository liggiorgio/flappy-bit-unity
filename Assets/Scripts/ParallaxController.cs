using UnityEngine;
using UnityEngine.Events;

public class ParallaxController : MonoBehaviour{
    public float velocity;
    public float boundLimit;
    public float boundOffset;
    public UnityEvent moving;
    public UnityEvent pushedBack;
    public UnityEvent reset;

    bool _isMoving;
    Vector3 _startPosition;
    
    void Awake() {
        _startPosition = transform.position;
    }

    void FixedUpdate() {
        if (_isMoving) {
            transform.Translate(velocity * Vector3.right * Time.fixedDeltaTime);
            // Repeat when passing past
            if (transform.position.x < boundLimit) {
                transform.Translate(boundOffset * Vector3.right);
                pushedBack.Invoke();
            }
        }
    }

    public void SetMotion(bool move) {
        _isMoving = move;
        if (move)
            moving.Invoke();
    }

    public void Reset() {
        _isMoving = false;
        transform.position = _startPosition;
        reset.Invoke();
    }
}
