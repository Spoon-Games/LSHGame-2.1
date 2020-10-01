using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.PlayerN
{
    public class FollowCam : MonoBehaviour
    {

        private Player player;
        void Start()
        {

            player = Player.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 newCamPos = new Vector2(player.transform.position.x, player.transform.position.y);
            transform.position = new Vector3(newCamPos.x, newCamPos.y, transform.position.z);
        }
    } 
}
