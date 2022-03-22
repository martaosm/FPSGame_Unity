using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiderEnemy : MonoBehaviour
{
    public Animator anima;
    public Slider healthBar;
    private bool dead = false;
    private int enemiesKilled = 0;
    //public Text score;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet") && !dead) 
        {
            anima.SetTrigger("damage");
            healthBar.value = healthBar.value - 10;
            Destroy(other.gameObject);
        }
        if (healthBar.value <= 0)
        {
            dead = true;
            anima.SetTrigger("die");
            //gameObject.SetActive(false);
            Invoke("Death",3f);
        }
    }

    private void Death()
    {
        //enemiesKilled++;
        //score.text = enemiesKilled + "/3";
        Destroy(gameObject);
    }
}
