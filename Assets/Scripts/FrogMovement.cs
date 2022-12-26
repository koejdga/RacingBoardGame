using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace FeedFrogGame
{
    public class FrogMovement : MonoBehaviour
    {

        Animator animator;
        float grey_frog_right_jump;
        float grey_frog_left_jump;
        float grey_frog_idle2;

        Dictionary<Player, int> playersFood;

        Vector3 controller;
        Vector3 target;
        Vector3 position;
        float t = 0;
        GameManager gameManager;

        public bool moves = false;

        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            animator = GetComponent<Animator>();
        }
        int numberOfMoves = 0;

        private void Update()
        {
            if (numberOfMoves > 0 && !moves)
            {
                animator.SetBool("isJumping", true);
                position = transform.position;
                if (gameManager.currentIndexesOfAnimals[gameManager.current] + 1 >= gameManager.slots.Length)
                {
                    target = gameManager.slots[gameManager.currentIndexesOfAnimals[gameManager.current] + 1 - gameManager.slots.Length].position;
                }
                else target = gameManager.slots[gameManager.currentIndexesOfAnimals[gameManager.current] + 1].position;
                gameManager.currentIndexesOfAnimals[gameManager.current]++;
                controller = new Vector3((position.x + target.x) / 2, position.y + 1.5f);
                moves = true;
            }

            if (moves && numberOfMoves > 0)
            {
                if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("grey_frog_right_jump"))
                {
                    animator.SetBool("isJumping", false);
                }
                t += Time.deltaTime;
                transform.position = QuadraticCurve(target, controller, t);
            }
            if (t >= 1)
            {
                moves = false;
                numberOfMoves--;
                if (numberOfMoves == 0)
                {
                    gameManager.GetComponent<Superpowers>().PowersFlow();
                    // отут треба змінити
                    // gameManager.MoveEnded = true;
                }
                animator.SetBool("isJumping", false);
                t = 0;
            }

            if (numberOfMoves == 0)
            {
                gameManager.GetComponent<Superpowers>().animIsactive = false;
            }
        }

        public void Move(Vector3 target, bool top)
        {
            numberOfMoves = 1;
            animator.SetBool("isJumping", true);
            if (top)
            {
                animator.SetBool("toRight", true);
            }
            else
            {
                animator.SetBool("toRight", false);
            }
            this.target = target;
            position = transform.position;
            int offset = top ? 1 : -1;
            controller = new Vector3((transform.position.x + target.x) / 2, transform.position.y + offset);
            moves = true;
        }

        public void Move(int numberOfMoves)
        {
            this.numberOfMoves = numberOfMoves;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.name == "Apple(Clone)" && gameManager.gameState == GameManager.GameState.AnimalsMove)
            {
                Debug.Log(other.gameObject.name + " --- " + gameObject.name);
                //moves = false;
                //gameManager.currentIndexesOfAnimals[gameManager.current]--;
                //StartCoroutine(movementAfterWeb());
                //Destroy(other.gameObject);
                //numberOfMoves = 0;
            }

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // може тут ще якісь частинки додати (particles)
            gameManager.isFinished = true;
        }

        IEnumerator movementAfterWeb()
        {
            LeanTween.move(gameObject, gameManager.slots[gameManager.currentIndexesOfAnimals[gameManager.current]], 0.4f);
            yield return new WaitForSeconds(0.4f);
            Debug.Log("Frog is in the right place");
        }

        Vector3 QuadraticCurve(Vector3 target, Vector3 controller, float t)
        {
            Vector3 ab = Vector3.Lerp(position, controller, t);
            Vector3 bc = Vector3.Lerp(controller, target, t);

            return Vector3.Lerp(ab, bc, t);
        }
    }
}
