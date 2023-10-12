using System.Collections;
using UnityEngine;

public class MarioBlock : MonoBehaviour
{
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite hitSprite;
    [SerializeField] private float bounceHeight = 0.2f;
    [SerializeField] private float bounceDuration = 0.2f;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float idleAnimationSpeed = 0.5f;

    private bool canBeHit = true;
    private Vector3 originalPosition;
    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;
    private Coroutine idleCoroutine;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.position;
        idleCoroutine = StartCoroutine(IdleAnimation());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.contacts[0].normal.y > 0.7f && canBeHit)
        {
            canBeHit = false;
            if (idleCoroutine != null)
            {
                StopCoroutine(idleCoroutine);
                idleCoroutine = null;
            }
            spriteRenderer.sprite = hitSprite;
            SpawnCoin();
            StartCoroutine(BounceBlock());
        }
    }

    private IEnumerator IdleAnimation()
    {
        while (true)
        {
            spriteRenderer.sprite = idleSprites[currentSpriteIndex];
            currentSpriteIndex = (currentSpriteIndex + 1) % idleSprites.Length;
            yield return new WaitForSeconds(idleAnimationSpeed);
        }
    }

    private IEnumerator BounceBlock()
    {
        float elapsedTime = 0f;
        Vector3 endPosition = originalPosition + Vector3.up * bounceHeight;

        // Bounce up
        while (elapsedTime < bounceDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(originalPosition, endPosition, elapsedTime / (bounceDuration / 2));
            yield return null;
        }

        elapsedTime = 0f;

        // Bounce down
        while (elapsedTime < bounceDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(endPosition, originalPosition, elapsedTime / (bounceDuration / 2));
            yield return null;
        }

        transform.position = originalPosition;
    }

    private void SpawnCoin()
    {
        GameObject coin = Instantiate(coinPrefab, transform.position + Vector3.up, Quaternion.identity);
    }
}