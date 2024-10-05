using UnityEngine;

public class Speeen : MonoBehaviour
{
    private void Update()
        => transform.localEulerAngles += Time.deltaTime * 45f * Vector3.back;
}
