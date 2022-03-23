using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private DefaultInput defaultInput;
    public float bulletSpeed;
    public GameObject bulletPrefab;
    public Transform bulletPos;
    private AudioSource gunSound;
    private void Awake()
    {
        defaultInput = new DefaultInput();
        defaultInput.Player.Shoot.performed += e => Shoot();
        defaultInput.Enable();
    }

    private void Update()
    {
        gunSound = GetComponent<AudioSource>();
    }

    public void Shoot()
    {
        gunSound.Play();
        var bullet = Instantiate(bulletPrefab, bulletPos.position, bulletPos.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward*-bulletSpeed);
        Destroy(bullet, 3f);
    }
}
