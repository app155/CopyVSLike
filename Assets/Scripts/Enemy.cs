using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float hp;
    public float maxHp;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    bool isDead;

    Animator anim;
    Rigidbody2D rigid;
    Collider2D coll;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new();
    }

    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isDead = false;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        hp = maxHp;
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (isDead || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (isDead)
            return;

        spriter.flipX = target.position.x < rigid.position.x;
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHp = data.hp;
        hp = data.hp;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || isDead)
            return;

        hp -= collision.GetComponent<Bullet>().dmg;
        StartCoroutine(KnockBack());

        if (hp > 0)
        {
            anim.SetTrigger("Hit");
            AudioManager.instance.PlaySFX(AudioManager.SFX.Hit);
        }

        else
        {
            isDead = true;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            anim.SetBool("Dead", true);
            GameManager.instance.kill++;
            GameManager.instance.GetEXP();

            if (GameManager.instance.isLive)
                AudioManager.instance.PlaySFX(AudioManager.SFX.Dead);
        }
    }

    IEnumerator KnockBack()
    {
        yield return wait;

        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;
        Vector3 dirVec = (myPos - playerPos).normalized;

        rigid.AddForce(dirVec * 3, ForceMode2D.Impulse);
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }
}
