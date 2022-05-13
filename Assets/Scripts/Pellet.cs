using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    public int score = 3;

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
            Destroy(gameObject);
        }    
    }

}
