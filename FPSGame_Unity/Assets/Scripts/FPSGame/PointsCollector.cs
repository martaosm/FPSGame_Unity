using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsCollector : MonoBehaviour
{

    public Text pointText;
    private float points;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
            if (this.gameObject.CompareTag("OnePoint"))
            {
                points++;
            }

            if (this.gameObject.CompareTag("TenPoints"))
            {
                points = points + 10;
            }

            if (this.gameObject.CompareTag("Exit"))
            {
                Application.Quit();
            }
            pointText.text = "Points:" + points;
        }
    }
}
