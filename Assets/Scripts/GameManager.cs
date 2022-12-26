using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FeedFrogGame
{
    public class GameManager : MonoBehaviour
    {
        public bool isFinished = false;

        public GameObject remotePlayerObject;
        public GameObject localPlayerObject;

        public GameObject winWindow;
        public TextMeshProUGUI resultText;
        public TextMeshProUGUI ratingText;

        public GameObject messageWindow;
        public TextMeshProUGUI messageText;
        string mistakeMessage = "Усі фішки мають бути на слотах";
        string gameStartedMessage = "Гра почалася!\r\nНатисніть на коробку";
        string newRoundMessage = "Новий раунд";
        string playersMadeChoiseMessage = "Всі зробили вибір";

        public static bool MovementIsAllowed = true;
        public static bool SpawnFoodAllowed = true;

        int amountOfAnimals = 5;

        string[] tagForPlate = new string[3];
        public GameObject NewRoundPanel;


        public Transform[] SlotPositionsForFood;
        int numberOfSlotsForFood;

        public GameObject slotsForFood;
        //GameObject[] food = new GameObject[3];

        int[] FoodLocations = new int[3];
        int[] moves = new int[5];

        public GameObject[] superpowersOfAnimal1, superpowersOfAnimal2, superpowersOfAnimal3, superpowersOfAnimal4, superpowersOfAnimal5;

        Dictionary<string, GameObject[]> findArray = new();
        public GameObject arrow;
        public Transform startPosForArrow;
        public Transform endPosForArrow;

        public Transform[] slots;
        public GameObject[] animals;

        public int[] currentIndexesOfAnimals = new int[5];
        int moveDuration = 2;

        Superpowers superpowers;

        Player localPlayer;
        Player remotePlayer;

        public enum GameState
        {
            Idle,
            GameStarted,
            PlayersMakeChoise,
            LocalPlayerMadeChoise,
            AllPlayersMadeChoise,
            LocalPlayersCardsMove,
            RemotePlayersCardsMove,
            AnimalsMove,
            NewRoundStarted,
            GameFinished
        };
        public enum TurnState
        {
            None,
            AnimalMovementStarted,
            AnimalMovementFinished,
            PauseStarted,
            PauseFinished,
            PowersStarted,
            PowersFinished
        };

        public TurnState turnState = TurnState.None;
        public GameState gameState = GameState.Idle;

        void Start()
        {
            gameState = GameState.GameStarted;
            GameFlow();
        }

        //****************** Game Flow *********************//
        public void GameFlow()
        {
            switch (gameState)
            {
                case GameState.Idle:
                    {
                        Debug.Log("IDLE");
                        break;
                    }
                case GameState.GameStarted:
                    {
                        Debug.Log("GameStarted");
                        StartCoroutine(OnGameStarted());
                        break;
                    }
                case GameState.PlayersMakeChoise:
                    {
                        Debug.Log("PlayersMakeChoise");
                        remotePlayer.MakeChoiseRandom();
                        break;
                    }
                case GameState.AllPlayersMadeChoise:
                    {
                        Debug.Log("AllPlayersMadeChoise");
                        StartCoroutine(OnAllPlayersMadeChoise());
                        break;
                    }
                case GameState.LocalPlayersCardsMove:
                    {
                        Debug.Log("LocalPlayersCardsMove");
                        StartCoroutine(FoodIsMovedToCenter(localPlayer));
                        break;
                    }
                case GameState.RemotePlayersCardsMove:
                    {
                        Debug.Log("RemotePlayersCardsMove");
                        StartCoroutine(FoodIsMovedToCenter(remotePlayer));
                        break;
                    }
                case GameState.AnimalsMove:
                    {
                        Debug.Log("AnimalsMove");
                        turnState = TurnState.AnimalMovementStarted;
                        TurnFlow();
                        break;
                    }
                case GameState.NewRoundStarted:
                    {
                        if (!isFinished)
                        {
                            Debug.Log("NewRoundStarted");
                            StartCoroutine(NewRound());
                        }
                        else
                        {
                            gameState = GameState.GameFinished;
                            GameFlow();
                        }
                        break;
                    }
                case GameState.GameFinished:
                    {
                        Debug.Log("GameFinished");
                        OnGameFinished();
                        break;
                    }
            }
        }

        IEnumerator OnAllPlayersMadeChoise()
        {
            messageText.text = playersMadeChoiseMessage;
            LeanTween.scale(messageWindow, Vector3.one, moveDuration * 0.5f).setEaseOutBack();
            yield return new WaitForSeconds(moveDuration * 0.5f + 0.2f);
            LeanTween.scale(messageWindow, Vector3.zero, moveDuration * 0.5f).setEaseInBack();
            yield return new WaitForSeconds(moveDuration - 0.2f);

            gameState = GameState.LocalPlayersCardsMove;
            GameFlow();
        }

        IEnumerator OnGameStarted()
        {
            messageText.text = gameStartedMessage;
            StartCoroutine(arrowMoves());

            LeanTween.scale(messageWindow, Vector3.one, moveDuration).setEaseOutBack();
            yield return new WaitForSeconds(moveDuration + 5);
            LeanTween.scale(messageWindow, Vector3.zero, moveDuration).setEaseInBack();
        }

        IEnumerator arrowMoves()
        {
            arrow.SetActive(true);
            while (gameState == GameState.GameStarted)
            {
                LeanTween.move(arrow, startPosForArrow, moveDuration * 0.5f);
                LeanTween.scale(arrow, new Vector3(0.6f, 0.6f, 0.6f), moveDuration * 0.5f);
                yield return new WaitForSeconds(moveDuration * 0.5f);

                LeanTween.move(arrow, endPosForArrow, moveDuration * 0.5f);
                LeanTween.scale(arrow, Vector3.one, moveDuration * 0.5f);
                yield return new WaitForSeconds(moveDuration * 0.5f);

            }

            arrow.SetActive(false);
        }

        public void OnGameFinished()
        {
            // вікно, де рейтинг 1, 2, 3 місце й вітання
            if (Winner() == localPlayer)
            {
                resultText.text = "Ви виграли!";
                ratingText.text += "1 - Ви\r\n" +
                    "2 - Бот";
            }
            else if (Winner() == remotePlayer)
            {
                resultText.text = "Ви програли!";
                ratingText.text += "1 - Бот\r\n" +
                    "2 - Ви";
            }
            else
            {
                resultText.text = "Абсолютно неймовірно, але нічия!";
                ratingText.text += "1 - Ви, Бот";
            }

            winWindow.SetActive(true);
            LeanTween.scale(winWindow, Vector3.one, moveDuration).setEaseOutBack();
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public Player Winner()
        {
            int winnerAnimalIndex = -1;
            int maxCurrentIndex = -1;
            for (int i = 0; i < amountOfAnimals; i++)
            {
                if (currentIndexesOfAnimals[i] >= maxCurrentIndex)
                {
                    maxCurrentIndex = currentIndexesOfAnimals[i];
                    winnerAnimalIndex = i;
                }
            }

            if (localPlayer.foodCount[winnerAnimalIndex] > remotePlayer.foodCount[winnerAnimalIndex])
            {
                return localPlayer;
            }
            if (localPlayer.foodCount[winnerAnimalIndex] < remotePlayer.foodCount[winnerAnimalIndex])
            {
                return remotePlayer;
            }
            if (localPlayer.foodCount[winnerAnimalIndex] == remotePlayer.foodCount[winnerAnimalIndex])
            {
                currentIndexesOfAnimals[winnerAnimalIndex] = 0;
                return Winner();
            }
            return null;
        }

        public void TurnFlow()
        {
            switch (turnState)
            {
                case TurnState.AnimalMovementStarted:
                    {
                        animals[current].GetComponent<FrogMovement>().Move(moves[current]);
                        moves[current] = 0;
                        break;
                    }
                case TurnState.PauseStarted:
                    {
                        timer.StartTimer(0.2f);
                        break;
                    }
                case TurnState.PowersStarted:
                    {
                        superpowers.currentSuperpower = Superpowers.CurrentSuperpower.Shield;
                        superpowers.PowersFlow();
                        break;
                    }
                case TurnState.PowersFinished:
                    {
                        current++;
                        if (current >= 5)
                        {
                            current = 0;
                            turnState = TurnState.None;
                            gameState = GameState.NewRoundStarted;
                            GameFlow();
                        }
                        else
                        {
                            turnState = TurnState.AnimalMovementStarted;
                            TurnFlow();
                        }
                        break;
                    }
            }
        }

        private void Awake()
        {
            timer = GetComponent<Timer>();
            superpowers = GetComponent<Superpowers>();
            numberOfSlotsForFood = SlotPositionsForFood.Length;

            for (int i = 0; i < moves.Length; i++)
            {
                moves[i] = 5;
                animals[i].transform.position = slots[i].position;
                currentIndexesOfAnimals[i] = i;
            }

            findArray["1animal"] = superpowersOfAnimal1;
            findArray["2animal"] = superpowersOfAnimal2;
            findArray["3animal"] = superpowersOfAnimal3;
            findArray["4animal"] = superpowersOfAnimal4;
            findArray["5animal"] = superpowersOfAnimal5;


            localPlayer = localPlayerObject.GetComponent<Player>();
            localPlayer.PlayerId = "offline-player";
            localPlayer.PlayerName = "Player";

            remotePlayer = remotePlayerObject.GetComponent<Player>();
            remotePlayer.PlayerId = "offline-bot";
            remotePlayer.PlayerName = "Bot";
            remotePlayer.IsAI = true;
        }

        public int current = 0;
        Timer timer;

        private void Update()
        {
            if (timer.isActive)
            {
                if (timer.CurrentTime == 0)
                {
                    timer.Stop();
                    turnState = TurnState.PowersStarted;
                    TurnFlow();
                }
            }

        }

        public void ReadyButtonClicked()
        {
            GameObject appleHeap = GameObject.FindGameObjectsWithTag("food heap")[0];

            int index = slotsForFood.transform.GetSiblingIndex();
            slotsForFood.transform.SetSiblingIndex(appleHeap.transform.GetSiblingIndex());
            appleHeap.transform.SetSiblingIndex(index);

            for (int i = 0; i < Constants.NUMBER_OF_FOOD; i++)
            {
                localPlayer.cards[i] = appleHeap.GetComponent<SpawnFood>().food[i];
            }

            if (isOnSlot(localPlayer.cards[0]) && isOnSlot(localPlayer.cards[1]) && isOnSlot(localPlayer.cards[2]))
            {
                MovementIsAllowed = false;
                messageWindow.transform.localScale = Vector3.zero;

                for (int i = 0; i < Constants.NUMBER_OF_FOOD; i++)
                {
                    localPlayer.cards[i].GetComponent<Card>().RemoveDragDrop();
                    localPlayer.cards[i].GetComponent<Card>().WebCheck();
                }

                gameState = GameState.AllPlayersMadeChoise;
                GameFlow();
            }
            else
            {
                messageText.text = mistakeMessage;
                LeanTween.scale(messageWindow, Vector3.one, 0.6f);
            }

        }

        private IEnumerator NewRound()
        {
            superpowers.powersDisappearance(superpowers.shields);

            GameObject appleHeap = GameObject.FindGameObjectsWithTag("food heap")[0];

            Array.Clear(remotePlayer.cards, 0, remotePlayer.cards.Length);

            int index = slotsForFood.transform.GetSiblingIndex();
            slotsForFood.transform.SetSiblingIndex(appleHeap.transform.GetSiblingIndex());
            appleHeap.transform.SetSiblingIndex(index);

            for (int i = 0; i < Constants.NUMBER_OF_FOOD; i++)
            {
                MoveFoodToPlate(i, localPlayer);
                yield return new WaitForSeconds(moveDuration * 0.5f + 0.1f);
            }

            for (int i = 0; i < Constants.NUMBER_OF_FOOD; i++)
            {
                MoveFoodToPlate(i, remotePlayer);
                yield return new WaitForSeconds(moveDuration * 0.5f + 0.1f);
            }

            for (int i = 0; i < amountOfAnimals; i++)
            {
                moves[i] = 5;
            }

            SpawnFoodAllowed = true;
            MovementIsAllowed = true;

            messageText.text = newRoundMessage;
            LeanTween.scale(messageWindow, Vector3.one, moveDuration).setEaseOutBack();
            yield return new WaitForSeconds(moveDuration + 0.2f);
            LeanTween.scale(messageWindow, Vector3.zero, moveDuration).setEaseInBack();

        }

        private void MoveFoodToPlate(int i, Player player)
        {
            if (player.cards[i] != null && player.cards[i].GetComponent<Card>().FaceFront)
            {
                LeanTween.move(player.cards[i], GameObject.FindGameObjectsWithTag(tagForPlate[i])[0].transform.position, moveDuration * 0.5f);
            }
        }

        int counter = 0;
        bool isOnSlot(GameObject food)
        {
            if (food == null) return false;
            Vector3 foodPosition = food.transform.position;

            for (int i = 0; i < numberOfSlotsForFood; i++)
            {
                if (foodPosition == SlotPositionsForFood[i].position)
                {
                    FoodLocations[counter] = i;
                    counter++;
                    if (counter == 3)
                    {
                        counter = 0;
                    }
                    return true;
                }
            }

            return false;
        }

        private IEnumerator FoodIsMovedToCenter(Player player)
        {
            OneFoodMovement(player);
            yield return new WaitForSeconds(moveDuration * 0.75f);
            OneFoodMovement(player);
            yield return new WaitForSeconds(moveDuration * 0.75f);
            OneFoodMovement(player);
            yield return new WaitForSeconds(moveDuration);

            gameState++;
            GameFlow();
        }

        void OneFoodMovement(Player player)
        {
            int animalNumber;
            string tag;
            string newTag;
            int currentFood;

            if (player == localPlayer)
            {
                int maximum = 15;
                int min = maximum;
                currentFood = 0;

                for (int i = 0; i < FoodLocations.Length; i++)
                {
                    if (FoodLocations[i] < min)
                    {
                        min = FoodLocations[i];
                        currentFood = i;
                    }
                }
                FoodLocations[currentFood] = maximum;

                tag = SlotPositionsForFood[min].tag;
                string[] numbers = tag.Split("a");
                animalNumber = Int32.Parse(numbers[0]);
            }
            else
            {
                currentFood = counter;
                animalNumber = player.cards[currentFood].GetComponent<Card>().AnimalChoise;
                tag = animalNumber + "animal";

            }

            newTag = tag + "InCenter";
            tagForPlate[currentFood] = tag + "Plate";

            float duration = player == localPlayer ? moveDuration : moveDuration * 0.5f;

            Debug.Log(player.cards[currentFood].GetComponent<Card>().FaceFront + " чи стороною їжі карта");

            if (player.cards[currentFood].GetComponent<Card>().FaceFront)
            {
                GameObject newSlot = GameObject.FindGameObjectsWithTag(newTag)[5 - moves[animalNumber - 1]];
                superpowers.animalFood[animalNumber - 1].Add(player.cards[currentFood]);
                moves[animalNumber - 1]--;
                if (animalNumber == 0)
                {
                    animalNumber = 1;
                }

                LeanTween.move(player.cards[currentFood], newSlot.transform.position, duration);
            }
            else
            {
                // якщо стороною суперсили перевернута фішка
                Debug.Log("якщо стороною суперсили перевернута фішка");
                string superpower = player.cards[currentFood].GetComponent<Card>().superpower;
                superpowers.findArray[superpower][animalNumber - 1].Add(player.cards[currentFood]);
                Vector3 newPosition = FindSuperpowerInArray(superpower, findArray[tag]);
                LeanTween.move(player.cards[currentFood], newPosition, duration);
            }

            counter++;
            if (counter == 3)
            {
                counter = 0;
            }
        }

        Vector3 FindSuperpowerInArray(string superpower, GameObject[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].tag == superpower)
                {
                    return array[i].transform.position;
                }
            }
            return Vector3.zero;
        }

    }
}
