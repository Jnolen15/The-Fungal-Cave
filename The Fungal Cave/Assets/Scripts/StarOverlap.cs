using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarOverlap : MonoBehaviour
{
    public PlayerController pc;

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Star")
        {
            //Debug.Log(col.gameObject.tag);
            pc.starOverlap = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Star")
        {
            //Debug.Log(col.gameObject.tag);
            pc.starOverlap = false;
        }
    }
}
