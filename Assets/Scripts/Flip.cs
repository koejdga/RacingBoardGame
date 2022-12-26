using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FeedFrogGame
{
    public class Flip : MonoBehaviour, IPointerClickHandler
    {
        Image myImage;
        Sprite face;
        private Sprite back;
        public int clickCount = 1;
        GameManager gameManager;


        bool _faceFront = true;
        public bool FaceFront
        {
            get { return _faceFront; }
            set
            {
                _faceFront = value; GetComponent<Card>().FaceFront = value; if (value) myImage.sprite = face;
                else myImage.sprite = back;
            }
        }

        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            myImage = GetComponent<Image>();
            face = myImage.sprite;
            back = GameObject.FindGameObjectWithTag(GetComponent<Card>().superpower + "InCircle").GetComponent<Image>().sprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == clickCount && gameManager.gameState == GameManager.GameState.PlayersMakeChoise)
            {
                FlipCard();
            }
        }

        public void FlipCard()
        {
            FaceFront = !FaceFront;
            Debug.Log(GetComponent<Card>().FaceFront + " чи стороною €блука карта");
            if (!FaceFront) transform.localScale = new Vector3(0.5f, 0.5f, 0);
            else transform.localScale = new Vector3(1, 1, 0);
        }


        /*
        Image image;

        [SerializeField]
        private Sprite food, superpower;

        private bool coroutineAllowed, foodUp;

        private void Start()
        {
            image = GetComponent<Image>();
            image.sprite = food;
            coroutineAllowed = true;
            foodUp = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("кл≥к" + eventData.clickCount);
            if (eventData.clickCount == 2)
            {
                Debug.Log(coroutineAllowed + " чи дозволено карантин");
                if (coroutineAllowed)
                {
                    StartCoroutine(RotateCard());
                }
            }
        }

        private IEnumerator RotateCard()
        {
            coroutineAllowed = false;

            if(!foodUp)
            {
                for (int i = 0; i <= 180; i += 10)
                {
                    transform.rotation = Quaternion.Euler(0, i, 0);
                    if (i == 90)
                    {
                        image.sprite = food;
                    }
                    yield return new WaitForSeconds(0.01f);
                }
            }
            else if (foodUp)
            {
                for (int i = 180; i >= 0; i -= 10)
                {
                    transform.rotation = Quaternion.Euler(0, i, 0);
                    if (i == 90)
                    {
                        image.sprite = superpower;
                    }
                    yield return new WaitForSeconds(0.01f);
                }
            }

            coroutineAllowed = true;
            foodUp = !foodUp;
        }
        */
    }
}
