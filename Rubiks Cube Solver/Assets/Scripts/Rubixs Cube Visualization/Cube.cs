using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RubiksCube.Visualization 
{
    public class Cube : MonoBehaviour
    {
        private static readonly int[] cubeTechnicalIndexToAssetIndex = new int[6] { 2, 3, 4, 1, 5, 6 };
        public GameObject cube;
        public int index;
        public RubiksCubeVisualization parentCube;

        /// <summary>
        /// Creates a cube where each side can have its color change independently and has a center at the parent transform
        /// </summary>
        /// <param name="parent"> Used as a parent for this cube </param>
        public void Awake()
        {
            CreateCube();
        }

        private void CreateCube()
        {
            cube = Instantiate(FindObjectOfType<RubiksCubeVisualization>().cubePrefab);
            cube.transform.SetParent(this.transform);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localScale = new Vector3(45, 45, 45);
            cube.transform.localRotation = Quaternion.Euler(0, 0, 0);
            cube.AddComponent<BoxCollider>();
            cube.tag = "Square";

            foreach (Material m in cube.GetComponent<Renderer>().materials)
            {
                m.color = Color.black;
            }
        }

        public void SetStickerColor(int sideNumber, Color color)
        {
            foreach (Material m in cube.GetComponent<Renderer>().materials)
            {
               if (m.name == $"Side {cubeTechnicalIndexToAssetIndex[sideNumber]} - Sticker (Instance)")
                {
                    m.color = color;
                }
            }
        }

        public void SetBaseColor(int sideNumber, Color color)
        {
            foreach (Material m in cube.GetComponent<Renderer>().materials)
            {
                if (m.name == $"Side {cubeTechnicalIndexToAssetIndex[sideNumber]} - Base (Instance)")
                {
                    m.color = color;
                }
            }
        }

        public void SetIndex(int index)
        {
            this.index = index;
        }


        #region Rotations

        public void RotateHorizontally(Transform around, bool animate = true, int rotationQuantity = 1)
        {
            if (animate)
            {
                StartCoroutine(Rotate(around, rotationQuantity, Vector3.up));
            }
            else
            {
                this.transform.RotateAround(around.position, around.rotation * Vector3.up, 90 * rotationQuantity);
                this.transform.localPosition = Round(this.transform.localPosition);
                this.transform.localRotation = Round(this.transform.localRotation);
            }
        }

        public void RotateVertically(Transform around, bool animate = true, int rotationQuantity = 1)
        {
            if (animate)
            {
                StartCoroutine(Rotate(around, rotationQuantity, Vector3.right));
            }
            else
            {
                this.transform.RotateAround(around.position, around.rotation * Vector3.right, 90 * rotationQuantity);
                this.transform.localPosition = Round(this.transform.localPosition);
                this.transform.localRotation = Round(this.transform.localRotation);
            }
        }

        public void RotateForward(Transform around, bool animate = true, int rotationQuantity = 1) 
        {
            if (animate)
            {
                StartCoroutine(Rotate(around, rotationQuantity, Vector3.forward));
            }
            else
            {
                this.transform.RotateAround(around.position, around.rotation * Vector3.forward, 90 * rotationQuantity);
                this.transform.localPosition = Round(this.transform.localPosition);
                this.transform.localRotation = Round(this.transform.localRotation);
            }
        }

        private IEnumerator Rotate(Transform around, int rotationQuantity, Vector3 primitiveAround)
        {
            parentCube.currentlyAnimating = true;
            float t = 0; // Timer
            Vector3 tmpPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.transform.localPosition.z);
            Quaternion tmpQuaternion = new Quaternion(this.transform.localRotation.x, this.transform.localRotation.y, this.transform.localRotation.z, this.transform.localRotation.w);
            int rotationDirection = rotationQuantity < 0 ? -1 : 1; 
            float animationDuration = Mathf.Abs(FindObjectOfType<RubiksCubeVisualization>().animationSpeed * rotationQuantity);

            while (t <= 1)
            {
                yield return null;
                t += Time.deltaTime / animationDuration;
                this.transform.RotateAround(around.position, around.rotation * primitiveAround, (90 * rotationDirection) / FindObjectOfType<RubiksCubeVisualization>().animationSpeed * Time.deltaTime);
            }

            this.transform.localPosition = tmpPosition;
            this.transform.localRotation = tmpQuaternion;
            this.transform.RotateAround(around.position, around.rotation * primitiveAround, 90 * rotationQuantity);
            this.transform.localPosition = Round(this.transform.localPosition);
            this.transform.localRotation = Round(this.transform.localRotation);
            parentCube.currentlyAnimating = false;
        }

        #endregion

        #region Rotations utility functions

        private Vector3 Round(Vector3 vector)
        {
            return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
        }
        private Quaternion Round(Quaternion quaternion)
        {
            return Quaternion.Euler(Mathf.Round(quaternion.eulerAngles.x / 10f) * 10, Mathf.Round(quaternion.eulerAngles.y / 10f) * 10, Mathf.Round(quaternion.eulerAngles.z / 10f) * 10);
        }

        #endregion
    }
}

