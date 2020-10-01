using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField]
        float moveSpeed = 1f;
        [SerializeField]
        private float minRandomRot = 1;
        [SerializeField]
        private float maxRandomRot = 4;

        private float rotTime;

        Rigidbody2D myRigidbody;
        // Use this for initialization
        void Start()
        {
            myRigidbody = GetComponent<Rigidbody2D>();
            rotTime = Time.time + UnityEngine.Random.Range(minRandomRot, maxRandomRot);
        }

        // Update is called once per frame
        void Update()
        {
            Move();
            UpdateRandom();
        }

        private void UpdateRandom()
        {
            if(Time.time > rotTime)
            {
                rotTime = Time.time + UnityEngine.Random.Range(minRandomRot, maxRandomRot);
                transform.localScale = new Vector2(-transform.localScale.x,transform.localScale.y);
            }
        }

        private void Move()
        {

            if (isFacingRight())
            {
                myRigidbody.velocity = new Vector2(moveSpeed, 0f);
            }
            else
            {
                myRigidbody.velocity = new Vector2(-moveSpeed, 0f);
            }

        }

        private bool isFacingRight()
        {
            return transform.localScale.x > 0;
        }

        private void OnTriggerExit2D(Collider2D collison)
        {
            transform.localScale = new Vector2(-(Mathf.Sign(myRigidbody.velocity.x)), 1f);
        }


    } 
}
