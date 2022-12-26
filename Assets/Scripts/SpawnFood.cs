using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FeedFrogGame
{
    public class SpawnFood : MonoBehaviour, IPointerClickHandler
    {
        float duration = 0.7f;
        public GameObject foodSample;
        public Transform[] FoodSpawnPlaces;
        [HideInInspector]
        public GameObject[] food = new GameObject[Constants.NUMBER_OF_FOOD];
        GameManager gameManager;

        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {


            if (GameManager.SpawnFoodAllowed)
            {
                if (gameManager.messageWindow.transform.localScale != Vector3.zero)
                {
                    LeanTween.alpha(gameManager.messageWindow.GetComponent<Image>().rectTransform, 0, 3);
                }
                gameManager.arrow.SetActive(false);

                for (int i = 0; i < food.Length; i++)
                {
                    food[i] = Instantiate(foodSample, transform, false);
                    Debug.Log(food[i].GetComponent<Flip>().FaceFront + " чи стороною їжі карта");
                    food[i].transform.position = gameObject.transform.position;
                    food[i].GetComponent<Card>().AddDragDrop();
                    LeanTween.move(food[i], FoodSpawnPlaces[i].position, duration);
                }

                GameManager.SpawnFoodAllowed = false;
                gameManager.gameState = GameManager.GameState.PlayersMakeChoise;
                gameManager.GameFlow();
            }




        }
    }
}
