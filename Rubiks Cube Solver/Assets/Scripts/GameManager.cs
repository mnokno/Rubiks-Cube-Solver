using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RubiksCube.Visualization;
using RubiksCube.Solver.Utility;
using RubiksCube.Solver;

namespace RubiksCube
{
    public class GameManager : MonoBehaviour
    {
        #region Class variables

        public RubiksCubeVisualization rubiksCubeVisualization;
        public SolverEngine solverEngine;

        #endregion

        #region Start

        // Start is called before the first frame update
        void Start()
        {
            solverEngine = new SolverEngine(this);
        }

        #endregion

        #region Class utility

        // Shuffles the cube  
        public void Shuffle(byte shuffleComplexity)
        {
            foreach(string eventName in RubiksCubeShuffler.GenerateShuffle(shuffleComplexity))
            {
                solverEngine.MakeMove(eventName);
                rubiksCubeVisualization.EnqueueCubeEvent(new RubiksCubeVisualization.CubeEvent() { eventName = eventName, parameters = new object[] { } });
            }
        }

        #endregion

        #region Button Events (For testing in a test scene)

        public void ShuffleBtnAct()
        {
            Shuffle(20);
        }

        public void SolveBtnAct()
        {
            solverEngine.Solve();
        }

        #endregion

        #region Moves

        public void MoveR()
        {
            rubiksCubeVisualization.EnqueueCubeEvent(new RubiksCubeVisualization.CubeEvent() { eventName = "MoveR", parameters = new object[] { } });
            solverEngine.rubiksCubeTechnical.MoveR();
        }

        public void MoveRi()
        {
            rubiksCubeVisualization.EnqueueCubeEvent(new RubiksCubeVisualization.CubeEvent() { eventName = "MoveRi", parameters = new object[] { } });
            solverEngine.rubiksCubeTechnical.MoveRi();
        }

        public void MoveU()
        {
            rubiksCubeVisualization.EnqueueCubeEvent(new RubiksCubeVisualization.CubeEvent() { eventName = "MoveU", parameters = new object[] { } });
            solverEngine.rubiksCubeTechnical.MoveU();
        }

        public void MoveUi()
        {
            rubiksCubeVisualization.EnqueueCubeEvent(new RubiksCubeVisualization.CubeEvent() { eventName = "MoveUi", parameters = new object[] { } });
            solverEngine.rubiksCubeTechnical.MoveUi();
        }

        public void MoveL()
        {
            rubiksCubeVisualization.EnqueueCubeEvent(new RubiksCubeVisualization.CubeEvent() { eventName = "MoveL", parameters = new object[] { } });
            solverEngine.rubiksCubeTechnical.MoveL();
        }

        public void MoveLi()
        {
            rubiksCubeVisualization.EnqueueCubeEvent(new RubiksCubeVisualization.CubeEvent() { eventName = "MoveLi", parameters = new object[] { } });
            solverEngine.rubiksCubeTechnical.MoveLi();
        }

        public void MoveD()
        {
            rubiksCubeVisualization.EnqueueCubeEvent(new RubiksCubeVisualization.CubeEvent() { eventName = "MoveD", parameters = new object[] { } });
            solverEngine.rubiksCubeTechnical.MoveD();
        }

        public void MoveDi()
        {
            rubiksCubeVisualization.EnqueueCubeEvent(new RubiksCubeVisualization.CubeEvent() { eventName = "MoveDi", parameters = new object[] { } });
            solverEngine.rubiksCubeTechnical.MoveDi();
        }

        public void MoveF()
        {
            rubiksCubeVisualization.EnqueueCubeEvent(new RubiksCubeVisualization.CubeEvent() { eventName = "MoveF", parameters = new object[] { } });
            solverEngine.rubiksCubeTechnical.MoveF();
        }

        public void MoveFi()
        {
            rubiksCubeVisualization.EnqueueCubeEvent(new RubiksCubeVisualization.CubeEvent() { eventName = "MoveFi", parameters = new object[] { } });
            solverEngine.rubiksCubeTechnical.MoveFi();
        }

        public void MoveX()
        {
            rubiksCubeVisualization.EnqueueCubeEvent(new RubiksCubeVisualization.CubeEvent() { eventName = "MoveX", parameters = new object[] { } });
            solverEngine.rubiksCubeTechnical.MoveX();
        }

        public void MoveXi()
        {
            rubiksCubeVisualization.EnqueueCubeEvent(new RubiksCubeVisualization.CubeEvent() { eventName = "MoveXi", parameters = new object[] { } });
            solverEngine.rubiksCubeTechnical.MoveXi();
        }

        public void MoveY()
        {
            rubiksCubeVisualization.EnqueueCubeEvent(new RubiksCubeVisualization.CubeEvent() { eventName = "MoveY", parameters = new object[] { } });
            solverEngine.rubiksCubeTechnical.MoveY();
        }

        public void MoveYi()
        {
            rubiksCubeVisualization.EnqueueCubeEvent(new RubiksCubeVisualization.CubeEvent() { eventName = "MoveYi", parameters = new object[] { } });
            solverEngine.rubiksCubeTechnical.MoveYi();
        }

        #endregion
    }
}

