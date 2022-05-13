using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPellet : MonoBehaviour
{

    int score = 10;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.AddPellet();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.instance.ReducePellet(score);
            GameManager.instance.frightened = true;
            Destroy(gameObject);
        }
    }
}
