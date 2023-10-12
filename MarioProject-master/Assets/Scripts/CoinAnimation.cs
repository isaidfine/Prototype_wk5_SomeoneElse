using System.Collections;
using UnityEngine;

public class CoinAnimation : MonoBehaviour
{
    [SerializeField] private float riseHeight = 1f; // 上升的高度
    [SerializeField] private float fallDistance = 0.5f; // 下降的距离
    [SerializeField] private float speed = 2f; // 动画速度

    private Vector3 originalPosition;
    private Vector3 peakPosition;
    private Vector3 endPosition;

    private void Start()
    {
        originalPosition = transform.position;
        peakPosition = originalPosition + Vector3.up * riseHeight;
        endPosition = peakPosition - Vector3.up * fallDistance;

        StartCoroutine(AnimateCoin());
    }

    private IEnumerator AnimateCoin()
    {
        float elapsedTime = 0f;
        float totalDuration = riseHeight / speed;

        // Rise phase
        while (elapsedTime < totalDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(originalPosition, peakPosition, elapsedTime / totalDuration);
            yield return null;
        }

        elapsedTime = 0f;
        totalDuration = fallDistance / speed;

        // Fall phase
        while (elapsedTime < totalDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(peakPosition, endPosition, elapsedTime / totalDuration);
            yield return null;
        }

        Destroy(gameObject); // 销毁金币
    }
}