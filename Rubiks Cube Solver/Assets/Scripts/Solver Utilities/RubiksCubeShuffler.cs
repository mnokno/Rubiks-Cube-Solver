using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RubiksCube.Solver.Utility
{
    public class RubiksCubeShuffler
    {
        #region Class variables

        private static string[] moves = new string[] { "MoveU", "MoveUi", "MoveD", "MoveDi", "MoveR", "MoveRi", "MoveL", "MoveLi", "MoveF", "MoveFi" };
        private static string[][] sideSwitches = new string[][] 
        { 
            new string[] { },
            new string[] { "MoveY" },
            new string[] { "MoveYi" },
            new string[] { "MoveX" },
            new string[] { "MoveXi" }
        };
        
        #endregion

        #region Shuffler

        public static Queue<string> GenerateShuffle(int movesPerShuffle)
        {
            Stack<string> rotationStack = new Stack<string>();
            Queue<string> cubeEventsQueue = new Queue<string>();
            string previousMove = null;
            string newMove;

            for (int i = 0; i < movesPerShuffle; i++)
            {
                // Choses a random side on the cube
                for (int j = 0; j < 3; j++)
                {
                    foreach (string eventName in sideSwitches[Random.Range(0, sideSwitches.Length)])
                    {
                        rotationStack.Push(eventName);
                        cubeEventsQueue.Enqueue(eventName);
                    }
                }

                // Choses a random move
                do
                {
                    newMove = moves[Random.Range(0, moves.Length)];
                }
                while (previousMove + 'i' == newMove || previousMove == newMove + 'i');
                cubeEventsQueue.Enqueue(newMove);
                previousMove = newMove;

                // Un-does the rotations
                while (rotationStack.Count > 0)
                {
                    string move = rotationStack.Pop();
                    if (move[move.Length - 1] == 'i')
                    {
                        cubeEventsQueue.Enqueue(move.Substring(0, move.Length - 1));
                    }
                    else
                    {
                        cubeEventsQueue.Enqueue(move + 'i');
                    }
                }
            }

            return cubeEventsQueue;
        }

        #endregion
    }
}
