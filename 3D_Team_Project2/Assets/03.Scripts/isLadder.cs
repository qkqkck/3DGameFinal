using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isLadder : MonoBehaviour
{
    private int inRange = 0;

    public GameObject player, pos;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("in");
        inRange = 1;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("out 0");
            inRange = 0;
        }
    }
    public void Act(bool act)
    {
        Debug.Log("onride");
        player.GetComponent<PlayerCtrl>().onladder = act;
        player.GetComponent<PlayerCtrl>().isGravity(!act);
        if (act == true)
        {
            player.transform.position =
                new Vector3(pos.transform.position.x, player.transform.position.y, pos.transform.position.z);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange == 1 && Input.GetKeyUp(KeyCode.E))
        {
            Debug.Log("in");
            inRange = 2;
            Act(true);
        }else if(inRange == 2 && Input.GetKeyUp(KeyCode.E))
        {
            Debug.Log("out");
            inRange = 1;
            Act(false);
        }

        if (inRange == 0)
        {
            player.GetComponent<PlayerCtrl>().onladder = false;
            player.GetComponent<PlayerCtrl>().isGravity(true);
        }
    }
}
