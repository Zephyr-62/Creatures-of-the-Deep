using UnityEngine;

public class MoveDemoObject : MonoBehaviour
{
    private bool _dirRight = true;
    public float speed = 2.0f;
    public float range = 3.0f;

    void Update () {
        if (_dirRight)
            transform.Translate (Vector2.right * (speed * Time.deltaTime));
        else
            transform.Translate (-Vector2.right * (speed * Time.deltaTime));
		
        if(transform.position.x >= range) {
            _dirRight = false;
        }
		
        if(transform.position.x <= -range) {
            _dirRight = true;
        }
    }
}
