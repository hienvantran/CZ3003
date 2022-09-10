using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Option : MonoBehaviour
{
    private int value;
    private float speed;
    private Rigidbody2D rb;

    //Start
    void Start()
    {
        this.speed = QuestionManager.instance.GetGameSpeed();
        rb = GetComponent<Rigidbody2D>();
    }

    //Fixed Update
    private void FixedUpdate()
    {
        //move it left according to speed
        rb.velocity = Vector2.left * speed;

        //check if somehow the player dodge the options and gone past the screen
        if (transform.position.x < -11f)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            //put back at spawn and have them cycle back in
            transform.position = new Vector2(15f, transform.position.y);
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    public void setVal(int v)
    {
        this.value = v;
        GetComponentInChildren<TextMeshPro>().text = v.ToString();
    }

    //trigger enter
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            QuestionManager.instance.AnswerCollided(this.value);
        }
    }
}
