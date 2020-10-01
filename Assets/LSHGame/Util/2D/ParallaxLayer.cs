using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.Util
{
    public class ParallaxLayer : MonoBehaviour
    {
        [SerializeField] bool lockVertical = true;
        [SerializeField] bool lockHorizontal = false;

        private Transform cameraTransform;

        private Vector3 startCameraPos;
        private Vector3 startPos;
        private Vector3 lastCameraPos;

        void Start()
        {
            cameraTransform = Camera.main.transform;
            startCameraPos = cameraTransform.position;
            startPos = transform.position;
        }


        private void LateUpdate()
        {
            //var position = startPos;
            Vector3 trans = Vector3.zero;

            float multiplier = 1 - (-cameraTransform.position.z / (transform.position.z - cameraTransform.position.z));

            if (!lockHorizontal)
                //trans.x = multiplier * (cameraTransform.position.x - lastCameraPos.x);
                trans.x = multiplier * (cameraTransform.position.x - startCameraPos.x);

            if (!lockVertical)
                //trans.y = multiplier * (cameraTransform.position.y - lastCameraPos.y);
                trans.y = multiplier * (cameraTransform.position.y - startCameraPos.y);

            //Debug.Log("Trans: " + trans);

            //transform.

            //astCameraPos = cameraTransform.position;
            transform.position = trans + startPos;
        }

    } 
}
