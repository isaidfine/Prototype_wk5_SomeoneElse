using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D rig2d;
    [SerializeField]
    Animator anim;
    
    public Transform checkPoint;

    public float curSpeed = 3f;
    public float maxSpeed = 5f;
    public float jumpSpeed = 10f;
    bool isFacingRight = true;
    bool isGrounded = true;
    
    public float maxJumpTime = 0.5f;  // 最大跳跃持续时间
    private bool isJumping = false;   // 是否正在跳跃
    private float jumpTimer = 0f;     // 跳跃计时器

    float checkDistance = 0.05f;

    public bool isDead = false;

    LayerMask groundLayer;
    LayerMask enemyLayer;

    Animator playerAnim;
    AnimatorStateInfo stateInfo;

    void Start ()
    {
        Init();

    }

    void Update()
    {
        stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (isDead && stateInfo.IsName("Die"))
        {
            return;
        }

        var h = Input.GetAxis("Horizontal");

        if (!isDead)
        { Move(h); }

        CheckIsGrounded();

        if (h > 0 && !isFacingRight)
        {
            Reverse();
        }
        else if (h < 0 && isFacingRight)
        {
            Reverse();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            StartJump();
        }

        if (Input.GetKeyUp(KeyCode.Space) || jumpTimer > maxJumpTime)
        {
            StopJump();
        }

        if (isJumping)
        {
            jumpTimer += Time.deltaTime;
        }
        void StartJump()
        {
            isJumping = true;
            jumpTimer = 0f;
            rig2d.velocity = new Vector2(rig2d.velocity.x, jumpSpeed);
        }

        void StopJump()
        {
            if (isJumping)
            {
                rig2d.velocity = new Vector2(rig2d.velocity.x, rig2d.velocity.y * 0.5f);  // 减少垂直速度，使跳跃更自然
                isJumping = false;
            }
        }
    }

    void Init()
    {
        rig2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        checkPoint = transform.Find("GroundCheckPoint");
        playerAnim = GetComponent<Animator>();

        groundLayer = 1 << LayerMask.NameToLayer("Ground");
        enemyLayer = 1 << LayerMask.NameToLayer("Enemy");
    }

    /// <summary>
    /// 反转角色
    /// </summary>
    void Reverse()
    {
        if (isGrounded)
        {
            isFacingRight = !isFacingRight;
            var scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    void Move(float dic)
    {
        rig2d.velocity = new Vector2(dic * curSpeed, rig2d.velocity.y);
        anim.SetFloat("Speed", Mathf.Abs(dic * curSpeed));
        anim.SetFloat("MoveSpeed", curSpeed);
    }

    void Jump()
    {
        //rig2d.AddForce(new Vector2(0, jumpHeight));
        rig2d.velocity = new Vector2(rig2d.velocity.x, jumpSpeed);
    }

    void CheckIsGrounded()
    {
        Vector2 check = checkPoint.position;
        RaycastHit2D hit = Physics2D.Raycast(check, Vector2.down, checkDistance, groundLayer.value);

        if (hit.collider != null)
        {
            anim.SetBool("IsGrounded", true);
            isGrounded = true;
        }
        else
        {
            anim.SetBool("IsGrounded", false);
            isGrounded = false;
        }
    }

    //void CheckHit()
    //{
    //    var check = checkPoint.position;
    //    var hit = Physics2D.OverlapCircle(check, 0.07f, enemyLayer.value);

    //    if (hit != null)
    //    {
    //        if (hit.CompareTag("Normal")) //若踩中普通怪物，则给予玩家一个反弹力，并触发怪物的死亡效果
    //        {
    //            Debug.Log("Hit Normal!");
    //            rig2d.velocity = new Vector2(rig2d.velocity.x, 5f);
    //            hit.GetComponentInParent<EnemyCharacter>().isHit = true;
    //        }
    //        else if (hit.CompareTag("Special")) //若踩中特殊怪物（乌龟），则在敌人相关代码中做对应变化
    //        {
    //            hitCount += 1;
    //            if (hitCount == 1)
    //            {
    //                rig2d.velocity = new Vector2(rig2d.velocity.x, 5f);
    //                hit.GetComponentInParent<EnemyCharacter>().GetHit(1);
    //            }
    //        }
    //    }
    //}

    public void InitHitState()
    {
        
    }

    public void Die()
    {
        Debug.Log("Player Die!");
        isDead = true;
        playerAnim.SetTrigger("Die");
        rig2d.velocity = new Vector2(0, 0);
        StartCoroutine(DeathMovement());
    }

    private IEnumerator DeathMovement()
    {
        float elapsedTime = 0f;
        float duration = 0.5f; // 时间段
        Vector3 startingPos = transform.position;
        Vector3 targetPos = startingPos + new Vector3(0, 1, 0);

        // 0 - 0.5秒 向上移动
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPos, targetPos, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos; // 确保达到目标位置

        // 重置时间和目标位置
        elapsedTime = 0f;
        startingPos = transform.position;
        targetPos = startingPos + new Vector3(0, -3, 0);

        // 0.5 - 1秒 向下移动
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPos, targetPos, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos; // 确保达到目标位置
    }

    public void Bounce()
    {
        rig2d.velocity = new Vector2(rig2d.velocity.x, 5f);
    }
}