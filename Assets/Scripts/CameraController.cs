using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float maxHeight = 100;
    public float minHeight = 1;
    public Vector2 maxDistance = new Vector2(100, 100);
    public float moveSpeed = 3f;
    public float boostSpeed = 8f;
    private Rigidbody rb;
    float v = 0;
    private Quaternion startRot;
    private Camera cam;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
        startRot = cam.transform.rotation;
    }

    void Update()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? boostSpeed : moveSpeed;
        Vector3 input = Vector3.ClampMagnitude(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("UpDown"), Input.GetAxisRaw("Vertical")), 1f);
        v = speed * input.magnitude;
        rb.velocity = speed * transform.TransformDirection(input) * Mathf.Sqrt(Mathf.Abs(transform.position.y));
        rb.position = new Vector3(Mathf.Clamp(rb.position.x, -maxDistance.x, maxDistance.x), Mathf.Clamp(rb.position.y, minHeight, maxHeight), Mathf.Clamp(rb.position.z, -maxDistance.y, maxDistance.y));  
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(0, (maxHeight - minHeight) / 2f + minHeight, 0), new Vector3(maxDistance.x * 2, (maxHeight - minHeight), maxDistance.y * 2));
    }

    private void OnCollisionEnter(Collision collision)
    {
        cam.transform.DOShakeRotation(Mathf.Sqrt(v) / 2f, v / 2, Mathf.FloorToInt(Mathf.Sqrt(v)) + 5).SetUpdate(true).OnComplete(() => { cam.transform.rotation = startRot; });
    }
}
