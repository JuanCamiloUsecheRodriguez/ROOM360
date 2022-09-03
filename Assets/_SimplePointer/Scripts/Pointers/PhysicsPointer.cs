using UnityEngine;

public class PhysicsPointer : MonoBehaviour
{
    public float defaultLength = 20f;

    private LineRenderer lineRenderer = null;

    public void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Update()
    {
        UpdateLength();
    }

    private void UpdateLength()
    {
        lineRenderer.SetPosition(0, transform.position + (transform.forward / 20 ));
        lineRenderer.SetPosition(1, CalculateEnd());
    }

    private Vector3 CalculateEnd()
    {
        RaycastHit hit = CreateForwardRaycast();
        Vector3 endPosition = DefaultEnd(defaultLength);

        if (hit.collider)
        {
            endPosition = hit.point;
        }

        return endPosition;
    }

    private RaycastHit CreateForwardRaycast()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        Physics.Raycast(ray, out hit, defaultLength);
        return hit;
    }

    private Vector3 DefaultEnd (float length)
    {
        return transform.position + (transform.forward * length);
    }


}
