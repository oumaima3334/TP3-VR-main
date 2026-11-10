using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class ShootManager : MonoBehaviour
{
    [SerializeField] private Transform controller;             
    [SerializeField] private float angleTolerance = 10f;       
    [SerializeField] private GameObject hitParticlesPrefab;
    [SerializeField] private AudioClip hitSound;

    [Header("Bonus")]
    [SerializeField] private float vibrationAmplitude = 0.5f;
    [SerializeField] private float vibrationDuration = 0.1f;
    [SerializeField] private GameObject laserPointerPrefab;    
    [SerializeField] private float maxRayDistance = 20f;
    [SerializeField] private float flashDuration = 0.5f;

    private GameObject[] targets;
    private InputDevice _rightController;
    private bool previousTriggerPressed = false;
    private GameObject laserPointer;

    void Start()
    {
        targets = GameObject.FindGameObjectsWithTag("Target");
        _rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Bonus 
        if (laserPointerPrefab != null)
        {
            laserPointer = Instantiate(laserPointerPrefab);
            laserPointer.SetActive(false);
        }
    }

    void Update()
    {
        UpdateLaserPointer();

        if (!_rightController.isValid)
        {
            _rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            if (!_rightController.isValid) return;
        }

        if (_rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed))
        {
            if (triggerPressed && !previousTriggerPressed)
                Shoot();

            previousTriggerPressed = triggerPressed;
        }
    }

    private void Shoot()
    {
        StartCoroutine(Vibrate());

        GameObject bestTarget = null;
        float bestAngle = angleTolerance;

        foreach (GameObject target in targets)
        {
            if (target == null) continue;

            Vector3 toTarget = target.transform.position - controller.position;
            float angle = Vector3.Angle(controller.forward, toTarget);

            if (angle < bestAngle)
            {
                bestAngle = angle;
                bestTarget = target;
            }
        }

        if (bestTarget == null) return;

        GameObject particles = Instantiate(hitParticlesPrefab, bestTarget.transform.position, Quaternion.identity);
        ParticleSystem ps = particles.GetComponent<ParticleSystem>();
        ps.Play();
        Destroy(particles, ps.main.duration + ps.main.startLifetime.constantMax);

        AudioSource audioSource = bestTarget.GetComponent<AudioSource>();
        if (audioSource != null && hitSound != null)
            audioSource.PlayOneShot(hitSound);

        StartCoroutine(FlashRed(bestTarget));
    }

    private IEnumerator Vibrate()
    {
        _rightController.SendHapticImpulse(0, vibrationAmplitude, vibrationDuration);
        yield return new WaitForSeconds(vibrationDuration);
    }

    private void UpdateLaserPointer()
    {
        if (laserPointer == null) return;

        if (Physics.Raycast(controller.position, controller.forward, out RaycastHit hit, maxRayDistance))
        {
            laserPointer.transform.position = hit.point;
            laserPointer.SetActive(true);
        }
        else
        {
            laserPointer.SetActive(false);
        }
    }

    private IEnumerator FlashRed(GameObject target)
    {
        Renderer r = target.GetComponent<Renderer>();
        Color startColor = r.material.color;
        float t = 0f;

        while (t < flashDuration)
        {
            float k = Mathf.PingPong(t * 4f, 1f);
            r.material.color = Color.Lerp(startColor, Color.red, k);
            t += Time.deltaTime;
            yield return null;
        }
        r.material.color = startColor;
    }
}