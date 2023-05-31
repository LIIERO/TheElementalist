using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoeDeathRespawn : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;

    [SerializeField] float death_time;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        Global.PlayingCutscene = false;
        if (Global.CurrentCheckpoint != null)
        {
            transform.position = new Vector3(Global.CurrentCheckpoint.transform.position.x, Global.CurrentCheckpoint.transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = Global.LevelEntryCoords;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != null)
        {
            if (collision.gameObject.CompareTag("Death"))
            {
                StartCoroutine(Die(death_time));
            }
        } 
    }

    private void Update()
    {
        if (Input.GetKeyDown(Global.kcRetry)) // Retry a level TODO: Should not be able to retry in hub
        {
            Global.ReloadCurrentScene();
        }
    }

    IEnumerator Die(float time)
    {
        Global.PlayingCutscene = true;
        rb.bodyType = RigidbodyType2D.Static; // player floats when killed
        anim.SetTrigger("death"); // death animation
        yield return new WaitForSeconds(time);
        Global.ReloadCurrentScene();
    }

    

}
