using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RubiksCube.Solver.Utility;

namespace RubiksCube.Solver
{
    public class SolverEngine
    {
        #region Class variables

        public RubiksCubeTechnical<byte> rubiksCubeTechnical;
        public GameManager gameManager;

        #endregion

        #region Class construction

        public SolverEngine(GameManager gameManager)
        {
            this.gameManager = gameManager;
            rubiksCubeTechnical = new RubiksCubeTechnical<byte>(RubiksCubeTechnical<byte>.defultByteCube);
        }

        #endregion

        #region Core

        // Solves the technical move and enqueues all the move to the visual cube
        public void Solve()
        {
            Task.Run(() => AssembleWhiteCross());
        }

        // Used to shuffle the cube
        public void MakeMove(string moveName)
        {
            switch (moveName)
            {
                case "MoveR":
                    rubiksCubeTechnical.MoveR();
                    break;
                case "MoveRi":
                    rubiksCubeTechnical.MoveRi();
                    break;

                case "MoveU":
                    rubiksCubeTechnical.MoveU();
                    break;
                case "MoveUi":
                    rubiksCubeTechnical.MoveUi();
                    break;

                case "MoveD":
                    rubiksCubeTechnical.MoveD();
                    break;
                case "MoveDi":
                    rubiksCubeTechnical.MoveDi();
                    break;

                case "MoveL":
                    rubiksCubeTechnical.MoveL();
                    break;
                case "MoveLi":
                    rubiksCubeTechnical.MoveLi();
                    break;

                case "MoveF":
                    rubiksCubeTechnical.MoveF();
                    break;
                case "MoveFi":
                    rubiksCubeTechnical.MoveFi();
                    break;

                case "MoveX":
                    rubiksCubeTechnical.MoveX();
                    break;
                case "MoveXi":
                    rubiksCubeTechnical.MoveXi();
                    break;

                case "MoveY":
                    rubiksCubeTechnical.MoveY();
                    break;
                case "MoveYi":
                    rubiksCubeTechnical.MoveYi();
                    break;
            }
        }

        #endregion

        #region Sub-Core
        
        private void AssembleWhiteCross()
        {
            // Helper function, returns true if a given face contains an edge of a specific color
            bool DoesContainEdge(byte faceIndex, byte colorIndex)
            {
                for (int i = 1; i < 9; i += 2)
                {
                    if (rubiksCubeTechnical.sides[faceIndex][i] == colorIndex)
                    {
                        return true;
                    }
                }
                return false;
            }

            // Assembles the white cross on the yellow face
            do
            {
                // Moves edges form the side faces
                for (int i = 0; i < 4; i++)
                {
                    while (DoesContainEdge(0, 5))
                    {
                        while (rubiksCubeTechnical.sides[4][7] == 5)
                        {
                            gameManager.MoveU();
                        }

                        while (rubiksCubeTechnical.sides[0][3] != 5)
                        {
                            gameManager.MoveF();
                        }

                        gameManager.MoveU();
                        gameManager.MoveLi();
                    }
                    gameManager.MoveY();
                }

                // Moves edges from the white face
                while (DoesContainEdge(5, 5))
                {
                    while (rubiksCubeTechnical.sides[4][7] == 5)
                    {
                        gameManager.MoveU();
                    }

                    while (rubiksCubeTechnical.sides[5][1] != 5)
                    {
                        gameManager.MoveD();
                    }

                    gameManager.MoveF();
                    gameManager.MoveF();
                }
            } while (rubiksCubeTechnical.sides[4][1] != 5 || rubiksCubeTechnical.sides[4][3] != 5 || rubiksCubeTechnical.sides[4][5] != 5 || rubiksCubeTechnical.sides[4][7] != 5);


            // Relocates the white edges form the yellow face to the white face
            for (int i = 0; i < 4; i++)
            {
                while (rubiksCubeTechnical.sides[0][1] != rubiksCubeTechnical.sides[0][4] || rubiksCubeTechnical.sides[4][7] != 5)
                {
                    gameManager.MoveU();
                }

                gameManager.MoveF();
                gameManager.MoveF();
                gameManager.MoveY();
            }

            // Calls the next stage in solving the cube
            FitWhiteCorners();
        }

        private void FitWhiteCorners()
        {
            // Returns true if correct piece is above the slot
            bool CorrectPieceAboveSlot()
            {
                return
                    (
                    rubiksCubeTechnical.sides[0][2] == rubiksCubeTechnical.sides[0][4] ||
                    rubiksCubeTechnical.sides[4][8] == rubiksCubeTechnical.sides[0][4] ||
                    rubiksCubeTechnical.sides[3][0] == rubiksCubeTechnical.sides[0][4]
                    ) &&
                    (
                    rubiksCubeTechnical.sides[0][2] == rubiksCubeTechnical.sides[5][4] ||
                    rubiksCubeTechnical.sides[4][8] == rubiksCubeTechnical.sides[5][4] ||
                    rubiksCubeTechnical.sides[3][0] == rubiksCubeTechnical.sides[5][4]
                    ) &&
                    (
                    rubiksCubeTechnical.sides[0][2] == rubiksCubeTechnical.sides[3][4] ||
                    rubiksCubeTechnical.sides[4][8] == rubiksCubeTechnical.sides[3][4] ||
                    rubiksCubeTechnical.sides[3][0] == rubiksCubeTechnical.sides[3][4]
                    );
            }

            // Returns true if the corner piece is oriented correctly
            bool CorrectlyOriented()
            {
                return rubiksCubeTechnical.sides[0][8] == rubiksCubeTechnical.sides[0][4] &&
                       rubiksCubeTechnical.sides[5][2] == rubiksCubeTechnical.sides[5][4] &&
                       rubiksCubeTechnical.sides[3][7] == rubiksCubeTechnical.sides[3][4];
            }

            // Moves white corner pieces form white side to yellow side
            for (int i = 0; i < 4; i++)
            {
                if ((rubiksCubeTechnical.sides[0][8] == 5) || (rubiksCubeTechnical.sides[5][2] == 5) || (rubiksCubeTechnical.sides[3][6] == 5))
                {
                    while ((rubiksCubeTechnical.sides[0][2] == 5) || (rubiksCubeTechnical.sides[4][8] == 5) || (rubiksCubeTechnical.sides[3][0] == 5))
                    {
                        gameManager.MoveUi();
                    }
                    gameManager.MoveR();
                    gameManager.MoveU();
                    gameManager.MoveRi();
                    gameManager.MoveUi();
                }
                gameManager.MoveY();
            }

            // Moves white corner pieces form yellow side to white side correctly orientated and permuted
            for (int i = 0; i < 4; i++)
            {
                while (!CorrectPieceAboveSlot())
                {
                    gameManager.MoveUi();
                }

                while (!CorrectlyOriented())
                {
                    gameManager.MoveR();
                    gameManager.MoveU();
                    gameManager.MoveRi();
                    gameManager.MoveUi();
                }

                gameManager.MoveY();
            }

            // Calls the next stage in solving the cube
            FinishSecondLayer();
        }

        private void FinishSecondLayer()
        {
            // Returns true if the yellow face contains an edge that should be inserted from this view axis 
            bool EdgeForInsertionExists()
            {
                return ((rubiksCubeTechnical.sides[0][1] == rubiksCubeTechnical.sides[0][4]) && ((rubiksCubeTechnical.sides[4][7] == rubiksCubeTechnical.sides[1][4]) || (rubiksCubeTechnical.sides[4][7] == rubiksCubeTechnical.sides[3][4]))) ||
                       ((rubiksCubeTechnical.sides[1][1] == rubiksCubeTechnical.sides[0][4]) && ((rubiksCubeTechnical.sides[4][3] == rubiksCubeTechnical.sides[1][4]) || (rubiksCubeTechnical.sides[4][3] == rubiksCubeTechnical.sides[3][4]))) ||
                       ((rubiksCubeTechnical.sides[2][1] == rubiksCubeTechnical.sides[0][4]) && ((rubiksCubeTechnical.sides[4][1] == rubiksCubeTechnical.sides[1][4]) || (rubiksCubeTechnical.sides[4][1] == rubiksCubeTechnical.sides[3][4]))) ||
                       ((rubiksCubeTechnical.sides[3][1] == rubiksCubeTechnical.sides[0][4]) && ((rubiksCubeTechnical.sides[4][5] == rubiksCubeTechnical.sides[1][4]) || (rubiksCubeTechnical.sides[4][5] == rubiksCubeTechnical.sides[3][4])));
            }

            // Returns true if F2L stage is finished
            bool F2LFinished()
            {
                return (rubiksCubeTechnical.sides[0][1] == 4 || rubiksCubeTechnical.sides[4][7] == 4) &&
                       (rubiksCubeTechnical.sides[1][1] == 4 || rubiksCubeTechnical.sides[4][3] == 4) &&
                       (rubiksCubeTechnical.sides[2][1] == 4 || rubiksCubeTechnical.sides[4][1] == 4) &&
                       (rubiksCubeTechnical.sides[3][1] == 4 || rubiksCubeTechnical.sides[4][5] == 4);
            }

            // Moves all non-yellow edges to the yellow face
            for (int i = 0; i < 4; i++)
            {
                if (rubiksCubeTechnical.sides[0][5] != 4 && rubiksCubeTechnical.sides[3][3] != 4)
                {
                    while (!((rubiksCubeTechnical.sides[1][1] == 4) || (rubiksCubeTechnical.sides[4][3] == 4)))
                    {
                        gameManager.MoveU();
                    }

                    gameManager.MoveR();
                    gameManager.MoveUi();
                    gameManager.MoveRi();
                    gameManager.MoveUi();
                    gameManager.MoveFi();
                    gameManager.MoveU();
                    gameManager.MoveF();
                }

                gameManager.MoveY();
            }

            while (!F2LFinished())
            {
                // Inserts the non-yellow edges correctly oriented and premeditated
                for (int i = 0; i < 4; i++)
                {
                    while (EdgeForInsertionExists())
                    {
                        while (!(rubiksCubeTechnical.sides[0][1] == rubiksCubeTechnical.sides[0][4] && ((rubiksCubeTechnical.sides[4][7] == rubiksCubeTechnical.sides[1][4]) || (rubiksCubeTechnical.sides[4][7] == rubiksCubeTechnical.sides[3][4]))))
                        {
                            gameManager.MoveU();
                        }

                        if (rubiksCubeTechnical.sides[4][7] == rubiksCubeTechnical.sides[1][4]) // Insertion to the left slot
                        {
                            gameManager.MoveUi();
                            gameManager.MoveLi();
                            gameManager.MoveU();
                            gameManager.MoveL();
                            gameManager.MoveU();
                            gameManager.MoveF();
                            gameManager.MoveUi();
                            gameManager.MoveFi();

                        }
                        else // Insertion to the right slot
                        {
                            gameManager.MoveU();
                            gameManager.MoveR();
                            gameManager.MoveUi();
                            gameManager.MoveRi();
                            gameManager.MoveUi();
                            gameManager.MoveFi();
                            gameManager.MoveU();
                            gameManager.MoveF();
                        }
                    }
                    gameManager.MoveY();
                }
            }

            // Calls the next stage in solving the cube
            AssembleYellowCross();
        }

        private void AssembleYellowCross()
        {
            // Thats the official name of this algorithm
            void SexyMove()
            {
                gameManager.MoveR();
                gameManager.MoveU();
                gameManager.MoveRi();
                gameManager.MoveUi();
            }

            // Orientates the yellow cross
            if ((rubiksCubeTechnical.sides[4][1] != 4) && (rubiksCubeTechnical.sides[4][3] != 4) && (rubiksCubeTechnical.sides[4][5] != 4) && (rubiksCubeTechnical.sides[4][7] != 4))
            {
                gameManager.MoveF();
                SexyMove();
                gameManager.MoveFi();
                gameManager.MoveU();
                gameManager.MoveU();
                gameManager.MoveF();
                SexyMove();
                SexyMove();
                gameManager.MoveFi();
            }
            else if (!((rubiksCubeTechnical.sides[4][1] == 4) && (rubiksCubeTechnical.sides[4][3] == 4) && (rubiksCubeTechnical.sides[4][5] == 4) && (rubiksCubeTechnical.sides[4][7] == 4)))
            {
                while (true)
                {
                    if (rubiksCubeTechnical.sides[4][1] == 4 && rubiksCubeTechnical.sides[4][3] == 4)
                    {
                        gameManager.MoveF();
                        SexyMove();
                        SexyMove();
                        gameManager.MoveFi();
                        break;
                    }
                    else if (rubiksCubeTechnical.sides[4][3] == 4 && rubiksCubeTechnical.sides[4][5] == 4)
                    {
                        gameManager.MoveF();
                        SexyMove();
                        gameManager.MoveFi();
                        break;
                    }
                    gameManager.MoveU();
                }
            }

            // Permeates the yellow cross
            while (rubiksCubeTechnical.sides[0][1] != rubiksCubeTechnical.sides[0][4])
            {
                gameManager.MoveU();
            }

            if (rubiksCubeTechnical.sides[1][1] == rubiksCubeTechnical.sides[2][4] && rubiksCubeTechnical.sides[2][1] == rubiksCubeTechnical.sides[3][4])
            {
                gameManager.MoveLi();
                gameManager.MoveUi();
                gameManager.MoveL();
                gameManager.MoveUi();
                gameManager.MoveLi();
                gameManager.MoveU();
                gameManager.MoveU();
                gameManager.MoveL();
            }
            else if (rubiksCubeTechnical.sides[3][1] == rubiksCubeTechnical.sides[2][4] && rubiksCubeTechnical.sides[2][1] == rubiksCubeTechnical.sides[1][4])
            {
                gameManager.MoveR();
                gameManager.MoveU();
                gameManager.MoveRi();
                gameManager.MoveU();
                gameManager.MoveR();
                gameManager.MoveUi();
                gameManager.MoveUi();
                gameManager.MoveRi();
            }
            else if (rubiksCubeTechnical.sides[1][1] == rubiksCubeTechnical.sides[3][4] && rubiksCubeTechnical.sides[3][1] == rubiksCubeTechnical.sides[1][4])
            {
                gameManager.MoveLi();
                gameManager.MoveUi();
                gameManager.MoveL();
                gameManager.MoveUi();
                gameManager.MoveLi();
                gameManager.MoveU();
                gameManager.MoveU();
                gameManager.MoveL();

                gameManager.MoveY();
                gameManager.MoveLi();
                gameManager.MoveUi();
                gameManager.MoveL();
                gameManager.MoveUi();
                gameManager.MoveLi();
                gameManager.MoveU();
                gameManager.MoveU();
                gameManager.MoveL();
                gameManager.MoveUi();
                gameManager.MoveYi();
            }
            else if (rubiksCubeTechnical.sides[1][1] == rubiksCubeTechnical.sides[2][4] && rubiksCubeTechnical.sides[2][1] == rubiksCubeTechnical.sides[1][4])
            {
                gameManager.MoveYi();
                gameManager.MoveR();
                gameManager.MoveU();
                gameManager.MoveRi();
                gameManager.MoveU();
                gameManager.MoveR();
                gameManager.MoveUi();
                gameManager.MoveUi();
                gameManager.MoveRi();
                gameManager.MoveU();
                gameManager.MoveY();
            }
            else if (rubiksCubeTechnical.sides[3][1] == rubiksCubeTechnical.sides[2][4] && rubiksCubeTechnical.sides[2][1] == rubiksCubeTechnical.sides[3][4])
            {
                gameManager.MoveY();
                gameManager.MoveLi();
                gameManager.MoveUi();
                gameManager.MoveL();
                gameManager.MoveUi();
                gameManager.MoveLi();
                gameManager.MoveU();
                gameManager.MoveU();
                gameManager.MoveL();
                gameManager.MoveUi();
                gameManager.MoveYi();
            }

            // Calls the final stage in solving the cube
            FitYellowCorners();
        }

        private void FitYellowCorners()
        {
            // Rotates the 3 corners, exulting the corner at [0][2]
            void RotateCorners()
            {
                gameManager.MoveU();
                gameManager.MoveR();
                gameManager.MoveUi();
                gameManager.MoveLi();
                gameManager.MoveU();
                gameManager.MoveRi();
                gameManager.MoveUi();
                gameManager.MoveL();
            }

            // Returns true if the [0][2] corner is correctly
            bool CorrectlyPermmutated02()
            {
                return ((rubiksCubeTechnical.sides[0][2] == rubiksCubeTechnical.sides[0][4]) || (rubiksCubeTechnical.sides[4][8] == rubiksCubeTechnical.sides[0][4]) || (rubiksCubeTechnical.sides[3][0] == rubiksCubeTechnical.sides[0][4])) &&
                       ((rubiksCubeTechnical.sides[0][2] == rubiksCubeTechnical.sides[3][4]) || (rubiksCubeTechnical.sides[4][8] == rubiksCubeTechnical.sides[3][4]) || (rubiksCubeTechnical.sides[3][0] == rubiksCubeTechnical.sides[3][4]));
            }

            // Returns true if the [0][0] corner is correctly
            bool CorrectlyPermmutated00()
            {
                return ((rubiksCubeTechnical.sides[0][0] == rubiksCubeTechnical.sides[0][4]) || (rubiksCubeTechnical.sides[4][6] == rubiksCubeTechnical.sides[0][4]) || (rubiksCubeTechnical.sides[1][2] == rubiksCubeTechnical.sides[0][4])) &&
                       ((rubiksCubeTechnical.sides[0][0] == rubiksCubeTechnical.sides[1][4]) || (rubiksCubeTechnical.sides[4][6] == rubiksCubeTechnical.sides[1][4]) || (rubiksCubeTechnical.sides[1][1] == rubiksCubeTechnical.sides[1][4]));
            }

            // Sexy move with 180 rotation with respect to the z-axis
            void SexyMoveR180()
            {
                gameManager.MoveL();
                gameManager.MoveD();
                gameManager.MoveLi();
                gameManager.MoveDi();
            }

            // Places the corners in the correct place
            if (!CorrectlyPermmutated02())
            {
                if (
                   ((rubiksCubeTechnical.sides[4][2] == rubiksCubeTechnical.sides[0][4]) || (rubiksCubeTechnical.sides[2][0] == rubiksCubeTechnical.sides[0][4]) || (rubiksCubeTechnical.sides[3][2] == rubiksCubeTechnical.sides[0][4])) &&
                   ((rubiksCubeTechnical.sides[4][2] == rubiksCubeTechnical.sides[3][4]) || (rubiksCubeTechnical.sides[2][0] == rubiksCubeTechnical.sides[3][4]) || (rubiksCubeTechnical.sides[3][2] == rubiksCubeTechnical.sides[3][4]))
                   )
                {
                    while (!CorrectlyPermmutated02())
                    {
                        gameManager.MoveYi();
                        RotateCorners();
                        gameManager.MoveY();
                    }
                }
                else
                {
                    while (!CorrectlyPermmutated02())
                    {
                        gameManager.MoveY();
                        RotateCorners();
                        gameManager.MoveYi();
                    }
                }
            }

            while (!CorrectlyPermmutated00())
            {
                RotateCorners();
            }

            // Orientates the corner pieces
            for (int i = 0; i < 4; i++)
            {
                while (rubiksCubeTechnical.sides[4][6] != 4)
                {
                    SexyMoveR180();
                }
                gameManager.MoveU();
            }
        }

        #endregion
    }
}

