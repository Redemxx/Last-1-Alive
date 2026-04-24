using System.Collections;
using UnityEngine;

public class SunController : MonoBehaviour
{
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.rotation = new Quaternion(0.215452597f, 0.643494844f, -0.207305238f, 0.704641163f);
        StartCoroutine(Sunset());
    }

    public IEnumerator Sunset()
    {
        float duration = 60;
        float elapsedTime = 0f;
        Quaternion startRotation = rb.rotation;
        Quaternion endRotation = new Quaternion(-0.439363092f, 0.517170191f, 0.49591276f, 0.541817069f);
        while (elapsedTime < duration)
        {
            rb.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rb.rotation = endRotation;
    }
}
