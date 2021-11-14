using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RubiksCube.UI
{
    public class UIManager : MonoBehaviour
    {
        #region Class variables

        public GameObject shuffleBtn;
        public GameObject solveBtn;
        public GameObject showBtn;
        public GameObject hideBtn;
        public GameObject shuffleSpeeedSld;
        public GameObject solveSpeedSld;

        public GameManager gameManager;
        public Animator animator;

        private bool shuffling = false ;
        private bool solving = false;

        #endregion

        #region Updates

        public void Update()
        {
            if (gameManager.rubiksCubeVisualization.cubeActionQueue.Count == 0)
            {
                shuffleBtn.GetComponent<Button>().interactable = true;
                solveBtn.GetComponent<Button>().interactable = true;
            }
            else
            {
                shuffleBtn.GetComponent<Button>().interactable = false;
                solveBtn.GetComponent<Button>().interactable = false;
            }
        }

        #endregion

        #region Class functions

        public void ShuffleBtnAct()
        {
            SetAnimationSpeed(shuffleSpeeedSld.GetComponent<Slider>().value);
            gameManager.Shuffle(30);
            shuffling = true;
            solving = false;
        }

        public void SolveBtnAct()
        {
            SetAnimationSpeed(solveSpeedSld.GetComponent<Slider>().value);
            gameManager.solverEngine.Solve();
            solving = true;
            shuffling = false;
        }

        public void ShowBtnAct()
        {
            animator.SetTrigger("Show");
        }

        public void HideBtnAct()
        {
            animator.SetTrigger("Hide");
        }

        public void ShuffleSpeedSldValCngAct(float speed)
        {
            if (shuffling)
            {
                SetAnimationSpeed(speed);
            }
        }

        public void SolveSpeedSldValCngAct(float speed)
        {
            if (solving)
            {
                SetAnimationSpeed(speed);
            }
        }

        #endregion

        #region Helper function

        private void SetAnimationSpeed(float value)
        {
            if (value == 0)
            {
                gameManager.rubiksCubeVisualization.animate = false;
            }
            else
            {
                gameManager.rubiksCubeVisualization.animate = true;
                gameManager.rubiksCubeVisualization.animationSpeed = value;
            }
        }

        #endregion
    }
}

