using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCamerController : MonoBehaviour
{
    public GameObject VirtualCam;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            VirtualCam.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("player") && !other.isTrigger)
        {
            VirtualCam.SetActive(false);
        }
    }


}
