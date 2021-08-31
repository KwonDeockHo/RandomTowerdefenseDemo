﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : Tower
{
    private float ShootTime;
    private float Timer;

    //public GameObject bulletPrefab;
    [SerializeField] private List<Transform> firePos;
    public string FirePosTagName = "FirePos";

    public Transform bulletPrefab;
    public Transform TargetPoint;
    // Start is called before the first frame update
    private void Awake()
    {
        Timer = 0.5f;
        ShootTime = 0.5f;

        InvokeRepeating("UPDATETARGET", 0f, 0.005f);

        turnSpeed = 8;
        partToRotate = transform;

        FindFirePos();
    }

    public override void UPDATETARGET()
    {
        if (target == null)
        {
            // 무작위 공격
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

            List<GameObject> disEnemies = new List<GameObject>();

            //Enemy 게임오브젝트를 모음
            foreach (GameObject enemy in enemies)
            {
                //해당 오브젝트의 위치와 적 오브젝트의 거리
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

                if (distanceToEnemy < range)
                {
                    disEnemies.Add(enemy.gameObject);
                }
            }

            //적이 있고 range거리보다 적 거리가 작으면
            if (disEnemies.Count > 0)
            {
                int random = Random.Range(0, disEnemies.Count);

                target = disEnemies[random].transform;

                targetEnemy = disEnemies[random].GetComponent<Enemy>();
            }
            else
            {
                target = null;
            }
        }
    }

    public override void LOCKONTARGET()
    {
        if (target == null)
            target.position = Vector3.zero;

        Vector3 dir = target.position - transform.position;
        var angle = Quaternion.FromToRotation(Vector3.up, dir).eulerAngles.z;
        Quaternion lookRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        // Lerp  0 ~ 100의 가중치 값이 회전하면서 가중치 값이 변하기 때문에
        // 초기에는 빠르게 회전 하나 이후에는 천천히 회전 함.

        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(new Vector3(0, 0, rotation.z));
    }

    void FindFirePos()
    {
        Transform[] tr = GetComponentsInChildren<Transform>();
        //Debug.Log("FirePos call : " + GetComponentsInChildren<Transform>().);
        for (int i = 0; i < tr.Length; i++)
        {
            Debug.Log("FirePos call : " + tr[i].gameObject.name);
            if (tr[i].gameObject.tag.Equals(FirePosTagName))
            {
                //Debug.Log("FirePos : " + transform.GetChild(i).transform);
                firePos.Add(tr[i].gameObject.transform);
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Timer += Time.deltaTime;

        if (Timer > ShootTime)
        {
            if (firePos != null)
            {
                ATTACK();
            }
            Timer = 0;
        }

        if (target == null)
        {
            //타겟 없고 레이저 쓸때
            return;
        }

        LOCKONTARGET();
    }

    public override void ATTACK()
    {
        Shoots();
    }

    void Shoots()
    {
        Vector3 v3distance = transform.forward + new Vector3(0, 10f, 0);
        Quaternion qrotate = Quaternion.Euler(transform.rotation.eulerAngles);

        for (int i = 0; i < firePos.Count; i++)
        {
            Transform bulletTransform = Instantiate(bulletPrefab, firePos[i].position, Quaternion.identity);
            Vector3 shootdir = ((qrotate * v3distance) - firePos[i].position).normalized;
            bulletTransform.GetComponent<SlowBullet>().Setup(shootdir);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {

    }
}
