using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RubiksCube.Visualization
{
    public class RotateRubiksCube : MonoBehaviour
    {
        float roatationSpeed = 360;
        public bool validSwipe = false;

        // Updates is called once per frame
        private void Update()
        {
            if (Input.touchCount == 1) // Only one finger is touching the screen
            {
                if (validSwipe)
                {
                    Rotate();
                }
                else
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.gameObject.tag == "Square")
                        {
                            validSwipe = true;
                            Rotate();
                        }
                    }
                }
            }
            else
            {
                validSwipe = false;
            }
        }

        public void Rotate()
        {
            this.transform.Rotate(Vector3.up, -Input.touches[0].deltaPosition.x * roatationSpeed * Mathf.Deg2Rad * Time.deltaTime, Space.World);
            this.transform.Rotate(Vector3.right, Input.touches[0].deltaPosition.y * roatationSpeed * Mathf.Deg2Rad * Time.deltaTime, Space.World);
        }
    }
}