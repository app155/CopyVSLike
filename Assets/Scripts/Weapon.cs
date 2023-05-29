using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float dmg;
    public int cnt;
    public float speed;

    float timer;
    Player player;

    void Awake()
    {
        player = GameManager.instance.player;
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            case 1:
                timer += Time.deltaTime;

                if (timer > speed)
                {
                    timer = 0f;
                    Fire();
                }

                break;
            default:
                break;
        }

        if (Input.GetButtonDown("Jump"))
            LevelUp(100, 1);
    }

    public void Init(ItemData data)
    {
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        id = data.itemId;
        dmg = data.baseDmg * Character.AtkDmg;
        cnt = data.baseCnt + Character.Count;

        for (int i = 0; i < GameManager.instance.pool.prefabs.Length; i++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[i])
            {
                prefabId = i;
                break;
            }
        }

        switch (id)
        {
            case 0:
                speed = 150 * Character.AtkSpeed;
                Place();
                break;
            case 1:
                speed = 0.3f * Character.AtkRate;
                break;
            default:
                break;
        }

        Hand hand = player.hands[(int)data.itemType];
        hand.spriter.sprite = data.Hand;
        hand.gameObject.SetActive(true);

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    void Place()
    {
        for (int i = 0; i < cnt; i++)
        {
            Transform bullet;

            if (i < transform.childCount)
                bullet = transform.GetChild(i);

            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * i / cnt;

            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);

            bullet.GetComponent<Bullet>().Init(dmg, -1, Vector3.zero);
        }
    }

    public void LevelUp(float dmg, int cnt)
    {
        this.dmg = dmg * Character.AtkDmg;
        this.cnt += cnt;

        if (id == 0)
            Place();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    void Fire()
    {
        if (player.scanner.nearestTarget == null)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.transform.position = transform.position;

        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(dmg, cnt, dir);
        AudioManager.instance.PlaySFX(AudioManager.SFX.Range);
    }
}
