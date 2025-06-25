using System.Collections;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private Vector3 _rotate;
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        StartCoroutine(RotationLoop());
    }

    private IEnumerator RotationLoop()
    {
        while (true)
        {
            Quaternion deltaRotation = Quaternion.Euler(_rotate * Time.fixedDeltaTime);
            _rb.MoveRotation(_rb.rotation * deltaRotation);
            yield return new WaitForFixedUpdate();
        }
    }
}