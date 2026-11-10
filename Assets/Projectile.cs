using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float respawnDelay = 3f; 

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;
    private XRGrabInteractable grab;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();

        grab.selectEntered.AddListener(OnGrabbed);
        grab.selectExited.AddListener(OnReleased);
    }

    void OnDestroy()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnGrabbed);
            grab.selectExited.RemoveListener(OnReleased);
        }
    }

    void Update()
    {
        
        if (transform.position.y < -2f)
            Respawn();
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        CancelInvoke(nameof(Respawn)); 
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        
        CancelInvoke(nameof(Respawn));
        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        if (grab.isSelected) return; 

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.SetPositionAndRotation(startPosition, startRotation);
    }
}