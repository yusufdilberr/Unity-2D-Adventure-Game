using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            CompleteLevel();
            Invoke("Complete Level", 2f);
            

        }
    }

    private void CompleteLevel()
    {
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1 );
    }
}
