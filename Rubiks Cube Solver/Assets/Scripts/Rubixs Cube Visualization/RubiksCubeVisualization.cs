using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using RubiksCube.Solver.Utility;
using System.Reflection;

namespace RubiksCube.Visualization
{
    public class RubiksCubeVisualization : MonoBehaviour
    {
        #region Class variables
        private static readonly Vector3[] cubePositions = new Vector3[27]
        {
        new Vector3(-1, 1, -1), new Vector3(0, 1, -1), new Vector3(1, 1, -1),
        new Vector3(-1, 0, -1), new Vector3(0, 0, -1), new Vector3(1, 0, -1),
        new Vector3(-1, -1, -1), new Vector3(0, -1, -1), new Vector3(1, -1, -1),

        new Vector3(-1, 1, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0),
        new Vector3(-1, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 0, 0),
        new Vector3(-1, -1, 0), new Vector3(0, -1, 0), new Vector3(1, -1, 0),

        new Vector3(-1, 1, 1), new Vector3(0, 1, 1), new Vector3(1, 1, 1),
        new Vector3(-1, 0, 1), new Vector3(0, 0, 1), new Vector3(1, 0, 1),
        new Vector3(-1, -1, 1), new Vector3(0, -1, 1), new Vector3(1, -1, 1),
        };
        private GameObject[] cubes = new GameObject[27];
        private RubiksCubeTechnical<RotationData> rubiksCubeDataRef;
        private RubiksCubeTechnical<byte> rubiksCubeTechnical;
        private RubiksCubeTechnical<byte> rubiksCubeColorRef = new RubiksCubeTechnical<byte>(RubiksCubeTechnical<byte>.defultByteCube);
        public bool currentlyAnimating = false;
        private bool acceptingNextItem = true;

        /// On a standard cube 
        /// color[0] = green
        /// color[1] = red
        /// color[2] = blue
        /// color[3] = orange
        /// color[4] = yellow
        /// color[5] = green
        [SerializeField]
        public bool stickerLess = false;
        public Color[] cubeTheme = new Color[6];
        public GameObject cubePrefab;
        public float moveDelay = 0.00f; // The time delay between each move

        public float animationSpeed = 1f; // 90 degrees in animationSpeed seconds
        public bool animate = true;

        public Queue<CubeEvent> cubeActionQueue { private set; get; } = new Queue<CubeEvent>(); // Queue containing moves to be executed, only one animation can be played at a given time 

        #endregion

        #region Start

        // Start is called before the first frame update
        void Start()
        {
            GenerateRubiksCube();
            ColorRubiksCube();
            InitiateTechnicalCube();

            #region TESTING

            //EnqueueCubeEvent(new CubeEvent() { eventName = "SetAnimationSpeed", parameters = new object[] { 5f } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveR", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveX", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveU", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "SetAnimationSpeed", parameters = new object[] { 0.5f } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveD", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveR", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveXi", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveUi", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveL", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "SetAnimationSpeed", parameters = new object[] { 1f } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveU", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveRi", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveYi", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveFi", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveLi", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "SetAnimate", parameters = new object[] { true } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveR", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveU", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveYi", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveL", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveR", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "SetAnimate", parameters = new object[] { true } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveUi", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveR", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveYi", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveF", parameters = new object[] { } });
            //EnqueueCubeEvent(new CubeEvent() { eventName = "MoveDi", parameters = new object[] { } });

            #endregion
        }

        // Updates is called once per frame
        void Update()
        {
            if (cubeActionQueue.Count != 0 && acceptingNextItem && !currentlyAnimating)
            {
                NextInQueue();
            }
        }

        #endregion

        #region Rubiks cube initialization

        private void GenerateRubiksCube()
        {
            for (int i = 0; i < 27; i++)
            {
                cubes[i] = new GameObject($"Cube Index {i}");
                cubes[i].AddComponent<Cube>();
                cubes[i].GetComponent<Cube>().parentCube = this;
                cubes[i].transform.SetParent(this.transform);
                cubes[i].transform.localPosition = cubePositions[i];
                cubes[i].transform.localScale = Vector3.one;
                cubes[i].GetComponent<Cube>().SetIndex(i);
            }

            cubes[13].GetComponent<Cube>().cube.transform.localScale = new Vector3(50, 50, 50);
        }

        private void ColorRubiksCube()
        {
            // Front side (green)
            for (int i = 0; i < 9; i++)
            {
                cubes[i].GetComponent<Cube>().SetStickerColor(0, cubeTheme[0]);
                if (stickerLess)
                {
                    cubes[i].GetComponent<Cube>().SetBaseColor(0, cubeTheme[0]);
                }
            }

            // Left side (red)
            for (int i = 0; i < 27; i += 3)
            {
                cubes[i].GetComponent<Cube>().SetStickerColor(1, cubeTheme[1]);
                if (stickerLess)
                {
                    cubes[i].GetComponent<Cube>().SetBaseColor(1, cubeTheme[1]);
                }
            }

            // Back side (blue)
            for (int i = 18; i < 27; i++)
            {
                cubes[i].GetComponent<Cube>().SetStickerColor(2, cubeTheme[2]);
                if (stickerLess)
                {
                    cubes[i].GetComponent<Cube>().SetBaseColor(2, cubeTheme[2]);
                }
            }

            // Right side (orange)
            for (int i = 2; i < 27; i += 3)
            {
                cubes[i].GetComponent<Cube>().SetStickerColor(3, cubeTheme[3]);
                if (stickerLess)
                {
                    cubes[i].GetComponent<Cube>().SetBaseColor(3, cubeTheme[3]);
                }
            }

            // Top side (yellow)
            int c = 0;
            for (int i = 0; i < 27; i++)
            {
                cubes[i].GetComponent<Cube>().SetStickerColor(4, cubeTheme[4]);
                if (stickerLess)
                {
                    cubes[i].GetComponent<Cube>().SetBaseColor(4, cubeTheme[4]);
                }
                c++;
                if (c == 3)
                {
                    i += 6;
                    c = 0;
                }
            }

            // Bottom side (white)
            for (int i = 6; i < 27; i++)
            {
                cubes[i].GetComponent<Cube>().SetStickerColor(5, cubeTheme[5]);
                if (stickerLess)
                {
                    cubes[i].GetComponent<Cube>().SetBaseColor(5, cubeTheme[5]);
                }
                c++;
                if (c == 3)
                {
                    i += 6;
                    c = 0;
                }
            }
        }

        private void InitiateTechnicalCube()
        {
            byte[][] sides = new byte[6][]
            {
                new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                new byte[] { 18, 9, 0, 21, 12, 3, 24, 15, 6 },
                new byte[] { 20, 19, 18, 23, 22, 21, 26, 25, 24 },
                new byte[] { 2, 11, 20, 5, 14, 23, 8, 17, 26 },
                new byte[] { 18, 19, 20, 9, 10, 11, 0, 1, 2 },
                new byte[] { 6, 7, 8, 15, 16, 17, 24, 25, 26}
            };

            RotationData[][] rotationDataSet = new RotationData[6][]
            {
                new RotationData[9] // Green side
                {
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Horizontal },
                        vertical = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Vertical },
                        forward = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Forward }
                    }, 
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Vertical },
                        vertical = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Horizontal },
                        forward = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Forward }
                    },
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Vertical },
                        vertical = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Horizontal },
                        forward = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Forward }
                    },
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Horizontal },
                        vertical = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Vertical },
                        forward = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Forward }
                    },
                    new RotationData()
                },
                new RotationData[9] // Red side
                {
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Horizontal },
                        vertical = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Forward },
                        forward = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Vertical }
                    },
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Forward },
                        vertical = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Horizontal },
                        forward = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Vertical }
                    },
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Forward },
                        vertical = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Horizontal },
                        forward = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Vertical }
                    },
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Horizontal },
                        vertical = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Forward },
                        forward = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Vertical }
                    },
                    new RotationData()
                },
                new RotationData[9] // Blue side
                {
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Horizontal },
                        vertical = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Vertical },
                        forward = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Forward }
                    },
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Vertical },
                        vertical = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Horizontal },
                        forward = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Forward }
                    },
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Vertical },
                        vertical = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Horizontal },
                        forward = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Forward }
                    },
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Horizontal },
                        vertical = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Vertical },
                        forward = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Forward }
                    },
                    new RotationData()
                },
                new RotationData[9] // Orange side
                {
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Horizontal },
                        vertical = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Forward },
                        forward = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Vertical }
                    },
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Forward },
                        vertical = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Horizontal },
                        forward = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Vertical }
                    },
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Forward },
                        vertical = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Horizontal },
                        forward = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Vertical }
                    },
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Horizontal },
                        vertical = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Forward },
                        forward = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Vertical }
                    },
                    new RotationData()
                },
                new RotationData[9] // Yellow side
                {
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Forward },
                        vertical = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Vertical },
                        forward = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Horizontal }
                    },
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Vertical },
                        vertical = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Forward },
                        forward = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Horizontal }
                    },
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Vertical },
                        vertical = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Forward },
                        forward = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Horizontal }
                    },
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Forward },
                        vertical = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Vertical },
                        forward = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Horizontal }
                    },
                    new RotationData()
                },
                new RotationData[9] // White side
                {
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Forward },
                        vertical = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Vertical },
                        forward = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Horizontal }
                    },
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Vertical },
                        vertical = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Forward },
                        forward = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Horizontal }
                    },
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Vertical },
                        vertical = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Forward },
                        forward = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Horizontal }
                    },
                    new RotationData(),
                    new RotationData()
                    {
                        horizontal = new Rotation() { rotationDirection = RotationDirection.Clockwise, rotationType = RotationType.Forward },
                        vertical = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Vertical },
                        forward = new Rotation() { rotationDirection = RotationDirection.AntiClockwise, rotationType = RotationType.Horizontal }
                    },
                    new RotationData()
                }
            };

            rubiksCubeTechnical = new RubiksCubeTechnical<byte>(sides);
            rubiksCubeDataRef = new RubiksCubeTechnical<RotationData>(rotationDataSet);
        }

        #endregion

        #region Moves

        #region Move U, Ui

        public void MoveU()
        {
            for (int i = 0; i < 9; i++) // Updates the visual rubiks cube
            {
                Rotate(rubiksCubeTechnical.sides[4][i], RotationType.Horizontal, animate: animate);
            }
            rubiksCubeTechnical.MoveU(); // Updates the technical state of the rubiks cube
            rubiksCubeColorRef.MoveU(); // Updates the color reference cube
        }

        public void MoveUi()
        {
            for (int i = 0; i < 9; i++) // Updates the visual rubiks cube
            {
                Rotate(rubiksCubeTechnical.sides[4][i], RotationType.Horizontal, rotationQuantity: -1, animate: animate);
            }
            rubiksCubeTechnical.MoveUi(); // Updates the technical state of the rubiks cube
            rubiksCubeColorRef.MoveUi(); // Updates the color reference cube
        }

        #endregion

        #region Move R, Ri

        public void MoveR()
        {
            for (int i = 0; i < 9; i++) // Updates the visual rubiks cube
            {
                Rotate(rubiksCubeTechnical.sides[3][i], RotationType.Vertical, animate: animate);
            }
            rubiksCubeTechnical.MoveR(); // Updates the technical state of the rubiks cube
            rubiksCubeColorRef.MoveR(); // Updates the color reference cube
        }

        public void MoveRi()
        {
            for (int i = 0; i < 9; i++) // Updates the visual rubiks cube
            {
                Rotate(rubiksCubeTechnical.sides[3][i], RotationType.Vertical, rotationQuantity: -1, animate: animate);
            }
            rubiksCubeTechnical.MoveRi(); // Updates the technical state of the rubiks cube
            rubiksCubeColorRef.MoveRi(); // Updates the color reference cube
        }

        #endregion

        #region Move F, Fi

        public void MoveF()
        {
            for (int i = 0; i < 9; i++) // Updates the visual rubiks cube
            {
                Rotate(rubiksCubeTechnical.sides[0][i], RotationType.Forward, animate: animate);
            }
            rubiksCubeTechnical.MoveF(); // Updates the technical state of the rubiks cube
            rubiksCubeColorRef.MoveF(); // Updates the color reference cube
        }

        public void MoveFi()
        {
            for (int i = 0; i < 9; i++) // Updates the visual rubiks cube
            {
                Rotate(rubiksCubeTechnical.sides[0][i], RotationType.Forward, rotationQuantity: -1, animate: animate);
            }
            rubiksCubeTechnical.MoveFi(); // Updates the technical state of the rubiks cube
            rubiksCubeColorRef.MoveFi(); // Updates the color reference cube
        }

        #endregion

        #region Move L, Li

        public void MoveL()
        {
            for (int i = 0; i < 9; i++) // Updates the visual rubiks cube
            {
                Rotate(rubiksCubeTechnical.sides[1][i], RotationType.Vertical, rotationQuantity: -1, animate: animate);
            }
            rubiksCubeTechnical.MoveL(); // Updates the technical state of the rubiks cube
            rubiksCubeColorRef.MoveL(); // Updates the color reference cube
        }

        public void MoveLi()
        {
            for (int i = 0; i < 9; i++) // Updates the visual rubiks cube
            {
                Rotate(rubiksCubeTechnical.sides[1][i], RotationType.Vertical, animate: animate);
            }
            rubiksCubeTechnical.MoveLi(); // Updates the technical state of the rubiks cube
            rubiksCubeColorRef.MoveLi(); // Updates the color reference cube
        }

        #endregion

        #region Move D, Di

        public void MoveD()
        {
            for (int i = 0; i < 9; i++) // Updates the visual rubiks cube
            {
                Rotate(rubiksCubeTechnical.sides[5][i], RotationType.Horizontal, rotationQuantity: -1, animate: animate);
            }
            rubiksCubeTechnical.MoveD(); // Updates the technical state of the rubiks cube
            rubiksCubeColorRef.MoveD(); // Updates the color reference cube
        }

        public void MoveDi()
        {
            for (int i = 0; i < 9; i++) // Updates the visual rubiks cube
            {
                Rotate(rubiksCubeTechnical.sides[5][i], RotationType.Horizontal, animate: animate);
            }
            rubiksCubeTechnical.MoveDi(); // Updates the technical state of the rubiks cube
            rubiksCubeColorRef.MoveDi(); // Updates the color reference cube
        }

        #endregion

        #region Move X, Xi

        public void MoveX()
        {
            // There is no animation to play
            rubiksCubeTechnical.MoveX(); // Updates the technical state of the rubiks cube
            rubiksCubeColorRef.MoveX(); // Updates the color reference cube
            rubiksCubeDataRef.MoveX(); // Updates the rotation data reference cube
        }

        public void MoveXi()
        {
            // There is no animation to play
            rubiksCubeTechnical.MoveXi(); // Updates the technical state of the rubiks cube
            rubiksCubeColorRef.MoveXi(); // Updates the color reference cube
            rubiksCubeDataRef.MoveXi(); // Updates the rotation data reference cube
        }

        #endregion

        #region Move Y, Yi

        public void MoveY()
        {
            // There is no animation to play
            rubiksCubeTechnical.MoveY(); // Updates the technical state of the rubiks cube
            rubiksCubeColorRef.MoveY(); // Updates the color reference cube
            rubiksCubeDataRef.MoveY(); // Updates the rotation data reference cube
        }

        public void MoveYi()
        {
            // There is no animation to play
            rubiksCubeTechnical.MoveYi(); // Updates the technical state of the rubiks cube
            rubiksCubeColorRef.MoveYi(); // Updates the color reference cube
            rubiksCubeDataRef.MoveYi(); // Updates the rotation data reference cube
        }

        #endregion

        #endregion

        #region Utility

        public void NextInQueue()
        {
            if (cubeActionQueue.Count > 0) // Queue is not empty
            {
                acceptingNextItem = false;

                if (!animate) // If animations are not spoused to be played all moves are made instantly
                {
                    CubeEvent cubeEvent = cubeActionQueue.Dequeue();
                    this.GetType().GetMethod(cubeEvent.eventName).Invoke(this, cubeEvent.parameters);
                    if (moveDelay != 0)
                    {
                        Invoke("AcceptingNextItem", moveDelay);
                    }
                    else
                    {
                        while (!animate && cubeActionQueue.Count > 0)
                        {
                            cubeEvent = cubeActionQueue.Dequeue();
                            this.GetType().GetMethod(cubeEvent.eventName).Invoke(this, cubeEvent.parameters);
                        }
                        AcceptingNextItem();
                    }
                    
                }
                else // Animations are enabled
                {
                    if (!currentlyAnimating)
                    {
                        CubeEvent cubeEvent = cubeActionQueue.Dequeue();
                        this.GetType().GetMethod(cubeEvent.eventName).Invoke(this, cubeEvent.parameters);
                        if (cubeEvent.eventName == "MoveX" || cubeEvent.eventName == "MoveXi" || cubeEvent.eventName == "MoveY" || cubeEvent.eventName == "MoveYi" || cubeEvent.eventName == "SetAnimationSpeed" || cubeEvent.eventName == "SetAnimate")
                        {
                            AcceptingNextItem();
                        }
                        else
                        {
                            Invoke("AcceptingNextItem", animationSpeed + moveDelay);
                        }
                    }
                    AcceptingNextItem();
                }
            }
        }

        public void EnqueueCubeEvent(CubeEvent cubeEvent)
        {
            cubeActionQueue.Enqueue(cubeEvent);
        }

        public void SetAnimationSpeed(float animationSpeed)
        {
            this.animationSpeed = animationSpeed;
        }

        public void SetAnimate(bool animate)
        {
            this.animate = animate;
        }
        
        private void Rotate(int cubeIndex, RotationType rotationType, int rotationQuantity = 1, bool animate = true)
        {
            Rotation rotation;
            switch (rotationType)
            {
                case RotationType.Horizontal:
                    rotation = rubiksCubeDataRef.sides[0][1].horizontal;
                    break;
                case RotationType.Vertical:
                    rotation = rubiksCubeDataRef.sides[0][1].vertical;
                    break;
                default:
                    rotation = rubiksCubeDataRef.sides[0][1].forward;
                    break;
            }

            switch (rotation.rotationType)
            {
                case RotationType.Horizontal:
                    cubes[cubeIndex].GetComponent<Cube>().RotateHorizontally(this.transform, rotationQuantity: rotationQuantity * (int)rotation.rotationDirection, animate: animate);
                    break;
                case RotationType.Vertical:
                    cubes[cubeIndex].GetComponent<Cube>().RotateVertically(this.transform, rotationQuantity: rotationQuantity * (int)rotation.rotationDirection, animate: animate);
                    break;
                case RotationType.Forward:
                    cubes[cubeIndex].GetComponent<Cube>().RotateForward(this.transform, rotationQuantity: rotationQuantity * (int)rotation.rotationDirection, animate: animate);
                    break;
            }
        }

        private void AcceptingNextItem()
        {
            acceptingNextItem = true;
        }

        #endregion

        #region Enums and Structs

        public struct CubeEvent
        {
            public string eventName;
            public object[] parameters;
        } 

        private struct RotationData
        {
            public Rotation horizontal;
            public Rotation vertical;
            public Rotation forward;
        }

        private struct Rotation
        {
            public RotationDirection rotationDirection;
            public RotationType rotationType;
        }

        private enum RotationDirection
        {
            Clockwise = 1,
            AntiClockwise = -1
        }

        private enum RotationType
        {
            Horizontal = 0,
            Vertical = 1,
            Forward = 2
        }

        #endregion
    }
}

