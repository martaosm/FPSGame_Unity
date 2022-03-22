using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float bulletSpeed;
    public GameObject bulletPrefab;
    public Transform bulletPos;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, bulletPos.position, bulletPos.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward*-bulletSpeed);
        Destroy(bullet, 3f);
    }
}
