using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour {
    public Transform specular;
    public float height {
        get { return _height; }
        set {
            _height = Mathf.Clamp(value, 3f, 11f);
            transform.position = new Vector2(transform.position.x, _height);
            specular.position = new Vector3(transform.position.x, -1f - _height / 2f, transform.position.z);
        }
    }
    
    [SerializeField]
    float _height;

    void OnValidate() {
        height = _height;
    }

    void Start() {
        GetComponent<ParallaxController>().SetMotion(true);
    }

    public void Randomise() {
        height = Random.Range(3f, 11f);
    }
}
