using System.Collections;
using UnityEngine;

public class SunController : MonoBehaviour
{
    Transform tr;
    Light light;
    void Start()
    {
        tr = GetComponent<Transform>();
        tr.rotation = new Quaternion(0.215452597f, 0.643494844f, -0.207305238f, 0.704641163f);
        light = GetComponent<Light>();
        StartCoroutine(Sunset());
    }

    public IEnumerator Sunset()
    {
        float duration = 60 * 5;
        float elapsedTime = 0f;
        Quaternion startRotation = tr.rotation;
        Quaternion endRotation = new Quaternion(-0.439363092f, 0.517170191f, 0.49591276f, 0.541817069f);
        while (elapsedTime < duration)
        {
            tr.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / duration);
            light.intensity = Mathf.Lerp(1f, 0f, elapsedTime / (duration /2 ));
            light.intensity = Mathf.Clamp(light.intensity, 0f, 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        tr.rotation = endRotation;
        light.intensity = 0f;
    }
}
