using Oculus.Interaction;
using UnityEngine;

public class Socket : MonoBehaviour
{
    [Tooltip("Tag for objects that can fit into this socket.")]
    public string targetTag = "Interactable";

    [Tooltip("Parent transform to assign the snapped object to.")]
    public Transform parentTransform;
    
    [SerializeField] private MeshRenderer meshRenderer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            // Temporarily disable the grab script or interaction (this will depend on how the grab system is set up)
            GrabInteractable grabInteractable = other.GetComponent<GrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.enabled = false; // Disable grabbing
            }

            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }

            // Snap the object to this socket's transform
            other.transform.position = transform.position;
            other.transform.rotation = transform.rotation;

            // Store the object's original global scale
            Vector3 originalScale = other.transform.lossyScale;

            // Parent the object (if a parentTransform is specified)
            if (parentTransform != null)
            {
                other.transform.SetParent(parentTransform);
            }

            // Correct the local scale to maintain the original world scale
            Vector3 parentScale = parentTransform != null ? parentTransform.lossyScale : Vector3.one;
            other.transform.localScale = new Vector3(
                originalScale.x / parentScale.x,
                originalScale.y / parentScale.y,
                originalScale.z / parentScale.z
            );

            // Disable physics on the snapped object
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // rb.useGravity = false;
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            Rigidbody parentRb = parentTransform.GetComponent<Rigidbody>();
            if (parentRb != null)
            {
                parentRb.linearVelocity = Vector3.zero;
                parentRb.angularVelocity = Vector3.zero;
            }

            // Log the snap action
            // Debug.Log($"{other.name} snapped into socket {name}, with size preserved.");

            // // Re-enable the grab system after the object has been snapped
            // if (grabInteractable != null)
            // {
            //     grabInteractable.enabled = true;
            // }
        }
    }
}