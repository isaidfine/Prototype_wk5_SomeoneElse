using System.Collections;
using DG.Tweening;
using TMPro;
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
    public GameManager gameManager;
    public TextMeshProUGUI lvUpText;
    
    public AudioClip coinSound;
    private AudioSource audioSource;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.position;
        idleCoroutine = StartCoroutine(IdleAnimation());
        audioSource = GetComponent<AudioSource>();
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
            gameManager.level++;
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
        lvUpText.DOFade(1, 0);
        lvUpText.rectTransform.DOMoveY(lvUpText.rectTransform.position.y + 80f, 1).OnComplete((() =>
        {
            lvUpText.rectTransform.position += Vector3.down * 80f;
        }));
        lvUpText.DOFade(0, 1f);
        audioSource.PlayOneShot(coinSound);
    }
    public void ResetBlock()
    {
        Debug.Log("Block Reset!");
        canBeHit = true;
        spriteRenderer.sprite = idleSprites[0];
        currentSpriteIndex = 0;
        if(idleCoroutine == null)
        {
            idleCoroutine = StartCoroutine(IdleAnimation());
        }
    }
}