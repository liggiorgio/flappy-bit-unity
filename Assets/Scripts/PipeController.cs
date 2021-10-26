using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeController : MonoBehaviour {
    public Transform bottomBody;
    public Transform bottomEdgeSpec;
    public Transform bottomBodySpec;
    public float height {
        get { return _height; }
        set {
            _height = Mathf.Clamp(value, 1f, 8f);
            transform.position = new Vector2(transform.position.x, _height);
            // Refactor pipe pieces
            bottomEdgeSpec.position = new Vector3(transform.position.x, -1f - _height / 2f, transform.position.z);
            bottomBody.localScale = new Vector3(1f, .25f + 1.25f * (_height -1) , 1f);
            bottomBodySpec.localScale = new Vector3(1f, .25f + 1.25f * (_height -1) , 1f);
        }
    }
    
    [SerializeField]
    float _height;

    void OnValidate() {
        height = _height;
    }

    public void Randomise() {
        height = Random.Range(1, 9);
    }
}
