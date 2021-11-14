using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RubiksCube.Solver.Utility
{
    /// <summary>
    /// This rubiks cube implementation supports the flowing moves: U, U', L, L', D, D', R, R', F, F', B, B', X, X', Y, Y'
    /// You can defines you own moves and algorithms combining supported moves
    /// e.g MoveM can you defined as L' R X'  
    /// </summary>
    public class RubiksCubeTechnical<T>
    {
        #region Class variables

        private static readonly Dictionary<byte, byte> faceRationCornerMap = new Dictionary<byte, byte>(4) { [0] = 2, [2] = 8, [8] = 6, [6] = 0}; // Maps rotations of corner pieces in a clockwise direction
        private static readonly Dictionary<byte, byte> faceRationEdgeMap = new Dictionary<byte, byte>(4) { [1] = 5, [5] = 7, [7] = 3, [3] = 1 }; // Maps rotations of edge pieces in a clockwise direction
        public static readonly byte[][] defultByteCube = new byte[6][]
        {
            new byte[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new byte[]{ 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            new byte[]{ 2, 2, 2, 2, 2, 2, 2, 2, 2 },
            new byte[]{ 3, 3, 3, 3, 3, 3, 3, 3, 3 },
            new byte[]{ 4, 4, 4, 4, 4, 4, 4, 4, 4 },
            new byte[]{ 5, 5, 5, 5, 5, 5, 5, 5, 5 }
        };

        /// <summary>
        /// On a standard cube 
        /// 
        ///              index[4]=yellow
        /// index[1]=red index[0]=green index[3]=orange index[2]=blue                 
        ///              index[5]=white
        ///              
        /// And color follow
        /// 0=green, 1=red, 2=blue, 3=orange, 4=yellow, 5=white
        /// </summary>
        public T[][] sides { get; private set; } = new T[6][];

        #endregion

        #region Constructors

        public RubiksCubeTechnical(T[][] sides)
        {
            // Checks if the given sides array is dimensional valid, it can still represent an impossible position a on rubiks cube
            if (sides.Length == 6)
            {
                foreach(T[] subArray in sides)
                {
                    if (subArray.Length != 9)
                    {
                        throw new System.Exception($"Invalid array dimension! Each sub array of sides array most contain exactly 9 bytes!");
                    }
                }

                this.sides = sides;
            }
            else
            {
                throw new System.Exception($"Invalid array dimension! Sides array most contain exactly 6 sub arrays !");
            }
        }

        #endregion

        #region Utility

        public void Copy<CT>(CT[] copyFrom, out CT[] copyTo) 
        {
            copyTo = new CT[copyFrom.Length];
            for (int i = 0; i < copyFrom.Length; i++)
            {
                copyTo[i] = copyFrom[i];
            }
        }

        private void FaceRotation(byte faceIndex, bool clockwise = true)
        {
            Copy(sides[faceIndex], out T[] tmpSide);
            foreach (byte key in faceRationCornerMap.Keys) // For each corner
            {
                byte from = key; // Position on the original cube
                faceRationCornerMap.TryGetValue(key, out byte to); // Position after the rotation
                if (clockwise)
                {
                    tmpSide[to] = sides[faceIndex][from]; // Rotates this corner clockwise and save the result to tmpSide
                }
                else
                {
                    tmpSide[from] = sides[faceIndex][to]; // Rotates this corner anti-clockwise and save the result to tmpSide
                }
            }
            foreach (byte key in faceRationEdgeMap.Keys) // For each edge
            {
                byte from = key; // Position on the original cube
                faceRationEdgeMap.TryGetValue(key, out byte to); // Position after the rotation
                if (clockwise)
                {
                    tmpSide[to] = sides[faceIndex][from]; // Rotates this edge clockwise and save the result to tmpSide
                }
                else
                {
                    tmpSide[from] = sides[faceIndex][to]; // Rotates this edge anti-clockwise and save the result to tmpSide
                }
            }
            sides[faceIndex] = tmpSide; // Updates the central cube
        }

        private void FlipIndexing(byte faceIndex)
        {
            T[] tmpSide = new T[9];
            for (int i = 0; i < 9; i++)
            {
                tmpSide[i] = sides[faceIndex][8 - i];
            }
            sides[faceIndex] = tmpSide;
        }

        #endregion

        #region Primitive Moves (U, X, Y)

        public void MoveU()
        {
            // Face rotation
            FaceRotation(4);

            // Ring rotation
            T[][] tmpSides = new T[4][];
            for (int i = 0; i < 4; i++) // For each face (green, red, blue, orange)
            {
                Copy(sides[i], out tmpSides[i]); // For the first 3 squares
                for (int j = 0; j < 3; j++)
                {
                    tmpSides[i][j] = sides[(i + 3) % 4][j];
                }
            }
            for (int i = 0; i < 4; i++)
            {
                sides[i] = tmpSides[i];
            }
        }

        public void MoveX()
        {
            // Creates a temporary cube
            T[][] tmpSides = new T[6][];

            // Rotates the sides in respect to x-axis
            tmpSides[4] = sides[0];
            tmpSides[2] = sides[4];
            tmpSides[5] = sides[2];
            tmpSides[0] = sides[5];
            tmpSides[1] = sides[1];
            tmpSides[3] = sides[3];

            // Updates the central cube
            sides = tmpSides;

            // Sides 1 (red) and 3 (orange) don't change their position on the cube they are rotated by 90 degrees
            FaceRotation(1, clockwise : false);
            FaceRotation(3);

            // Fixes indexing issues
            FlipIndexing(2);
            FlipIndexing(5);
        }

        public void MoveY()
        {
            // Creates a temporary cube
            T[][] tmpSides = new T[6][];

            // Rotates the sides in respect to x-axis
            tmpSides[1] = sides[0];
            tmpSides[2] = sides[1];
            tmpSides[3] = sides[2];
            tmpSides[0] = sides[3];
            tmpSides[4] = sides[4];
            tmpSides[5] = sides[5];

            // Updates the central cube
            sides = tmpSides;

            // Sides 4 (yellow) and 5 (white) don't change their position on the cube they are rotated by 90 degrees 
            FaceRotation(4);
            FaceRotation(5, clockwise: false);
        }

        #endregion

        #region Composite Moves

        #region Moves Ui, Xi, Yi

        public void MoveUi()
        {
            MoveU(); MoveU(); MoveU();
        }
        
        public void MoveXi()
        { 
            MoveX(); MoveX(); MoveX();
        }

        public void MoveYi()
        { 
            MoveY(); MoveY(); MoveY();
        }

        #endregion

        #region Moves D, Di

        public void MoveD()
        {
            MoveX(); MoveX(); MoveU(); MoveX(); MoveX();
        }

        public void MoveDi()
        {
            MoveD(); MoveD(); MoveD();
        }

        #endregion

        #region Moves L, Li

        public void MoveL()
        {
            MoveYi(); MoveX(); MoveU(); MoveXi(); MoveY();
        }

        public void MoveLi()
        {
            MoveL(); MoveL(); MoveL();
        }

        #endregion

        #region Moves R, Ri

        public void MoveR()
        {
            MoveY(); MoveX(); MoveU(); MoveXi(); MoveYi();
        }

        public void MoveRi()
        {
            MoveR(); MoveR(); MoveR();
        }

        #endregion

        #region Moves F, Fi

        public void MoveF()
        {
            MoveX(); MoveU(); MoveXi();
        }

        public void MoveFi()
        {
            MoveF(); MoveF(); MoveF();
        }

        #endregion

        #region Moves B, Bi

        public void MoveB()
        {
            MoveXi(); MoveU(); MoveX();
        }

        public void MoveBi()
        {
            MoveB(); MoveB(); MoveB();
        }

        #endregion

        #endregion

        #region Console support

        public override string ToString()
        {
            string formated = "";

            for (int i = 0; i < 6; i++) // For each side
            {
                string side = "";
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        side += $"{sides[i][x * 3 + y]} ";
                    }
                    side += "\n";
                }
                formated += "\n" + side;
            }

            return formated;
        }

        private enum PrimitiveColorsToIndex
        {
            Green = 0,
            Red = 1,
            Blue = 2,
            Orange = 3,
            Yellow = 4,
            White = 5
        }

        #endregion
    }
}

