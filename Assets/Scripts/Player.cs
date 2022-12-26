using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FeedFrogGame
{
    public class Player : MonoBehaviour
    {
        public string PlayerId;
        public string PlayerName;
        public bool IsAI;

        public GameObject cardSample;
        public GameObject[] cards = new GameObject[Constants.NUMBER_OF_FOOD];

        public int[] foodCount = new int[Constants.NUMBER_OF_ANIMALS];

        public void MakeChoiseRandom()
        {
            cards = new GameObject[Constants.NUMBER_OF_FOOD];
            for (int i = 0; i < Constants.NUMBER_OF_FOOD; i++)
            {
                cards[i] = Instantiate(cardSample, transform, false);
                cards[i].transform.position = gameObject.transform.position;
                cards[i].transform.SetParent(gameObject.transform.parent);
                cards[i].transform.SetAsFirstSibling();

                Debug.Log(cards[i] == null);

                cards[i].GetComponent<Card>().FaceFront = Random.value >= 0.5;
                cards[i].GetComponent<Card>().AnimalChoise = Random.Range(1, 6);

                if (!cards[i].GetComponent<Card>().FaceFront)
                {
                    cards[i].GetComponent<Flip>().FlipCard();
                    cards[i].GetComponent<Card>().WebCheck();
                }
            }
        }
    }
}
