using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.Util
{
    public class ParallaxLayer : MonoBehaviour
    {
        [SerializeField] bool lockVertical = true;
        [SerializeField] bool lockHorizontal = false;

        protected Transform cameraTransform;

        protected Vector3 startCameraPos;
        protected Vector3 startPos;
        protected Vector3 lastCameraPos;

        protected virtual void Start()
        {
            cameraTransform = Camera.main.transform;
            startCameraPos = cameraTransform.position;
            startPos = transform.position;
        }


        protected virtual void LateUpdate()
        {
            //var position = startPos;
            Vector3 trans = Vector3.zero;

            float multiplier = GetMultiplier();

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

        protected float GetMultiplier()
        {
            return 1 - (-cameraTransform.position.z / (transform.position.z - cameraTransform.position.z));
        }

    } 
}
