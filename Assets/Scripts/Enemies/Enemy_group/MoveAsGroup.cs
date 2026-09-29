using UnityEngine;

public class MoveAsGroup : MonoBehaviour
{
    [SerializeField] private float speed = 0.48f;
    [SerializeField] private float halfWidth = 0.9f;
    private int _dir = 1;
    Vector3 _startPos;
    
    private void Awake()
    {
        _startPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 nextPosition = transform.position + Vector3.right * (_dir * speed * Time.fixedDeltaTime);
        float offsetX = nextPosition.x - _startPos.x;
        if (Mathf.Abs(offsetX) >= halfWidth)
        {
            _dir *= -1;
            float clampedX = _startPos.x + (Mathf.Sign(offsetX) * halfWidth);
            nextPosition = new Vector3(clampedX, nextPosition.y, nextPosition.z);
        }
        transform.position = nextPosition;
    }
}
