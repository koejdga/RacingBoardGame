using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FeedFrogGame
{
    public class Superpowers : MonoBehaviour
    {
        public Transform openedCan;
        public GameObject choiseForTeleport;

        GameManager gameManager;

        public Dictionary<string, List<GameObject>[]> findArray = new();

        public List<GameObject>[] animalFood = new List<GameObject>[5];
        public List<GameObject>[] shields = new List<GameObject>[5];
        public List<GameObject>[] axes = new List<GameObject>[5];
        public List<GameObject>[] teleports = new List<GameObject>[5];
        public List<GameObject>[] webs = new List<GameObject>[5];
        public List<GameObject>[] speedups = new List<GameObject>[5];

        public GameObject[] choiseButtons;
        public GameObject activeShield;

        int amountOfAnimals = 5;
        float duration = 1.5f;


        bool shieldOn = false;
        public bool animIsactive = false;
        public int speed;
        int current;

        public GameObject lightning;
        public GameObject sparks;

        public Sprite[] frogSprites;

        public enum CurrentSuperpower
        {
            None,
            Shield,
            Axe,
            Teleport,
            Web,
            Speedup
        };

        public CurrentSuperpower currentSuperpower = CurrentSuperpower.None;

        // треба ще зробити кнопку фінішу
        // і ще прописати павутину щоб вона зупиняла тварину

        void Awake()
        {
            gameManager = GetComponent<GameManager>();

            for (int i = 0; i < amountOfAnimals; i++)
            {
                animalFood[i] = new List<GameObject>();
                shields[i] = new List<GameObject>();
                axes[i] = new List<GameObject>();
                teleports[i] = new List<GameObject>();
                webs[i] = new List<GameObject>();
                speedups[i] = new List<GameObject>();
            }

            findArray["shield"] = shields;
            findArray["axe"] = axes;
            findArray["teleport"] = teleports;
            findArray["web"] = webs;
            findArray["speedup"] = speedups;
        }

        public void PowersFlow()
        {
            current = gameManager.current;
            if (!shieldOn && shields[current].Count > 0)
            {
                shieldOn = true;
                StartCoroutine(shieldAnim());
            }
            else if (axes[current].Count > 0)
            {
                StartCoroutine(axeAnim());
                axes[current].Clear();
            }
            else if (teleports[current].Count > 0)
            {
                StartCoroutine(teleportAnim());
            }
            else if (!animIsactive && webs[current].Count > 0)
            {
                StartCoroutine(webAnim());
            }
            else if (speedups[current].Count > 0)
            {
                StartCoroutine(speedupAnim());
            }
            else
            {
                gameManager.turnState = GameManager.TurnState.PowersFinished;
                gameManager.TurnFlow();
            }
        }

        public Material material;

        IEnumerator shieldAnim()
        {
            GameObject lastShield = shields[current][shields[current].Count - 1];
            StartCoroutine(AccentOnCard(lastShield));
            yield return new WaitForSeconds(0.6f);

            float distance = Vector3.Distance(lastShield.transform.position, gameManager.animals[current].transform.position);
            LeanTween.move(lastShield, gameManager.animals[current].transform.position, distance / speed);
            yield return new WaitForSeconds(distance / speed);

            gameManager.animals[current].GetComponent<Image>().material = material;

            int duration = 1;
            LeanTween.alpha(lastShield.GetComponent<Image>().rectTransform, 0, duration);
            yield return new WaitForSeconds(duration);
            Destroy(lastShield);

            PowersFlow();
        }

        IEnumerator axeAnim()
        {
            // треба додати штуку коли сокира не працює бо щит
            GameObject lastAxe = axes[current][axes[current].Count - 1];
            StartCoroutine(AccentOnCard(lastAxe));
            yield return new WaitForSeconds(0.6f);

            lightning.SetActive(true);
            sparks.SetActive(true);
            yield return new WaitForSeconds(1);
            lightning.SetActive(false);

            for (int i = 0; i < animalFood[current].Count; i++)
            {
                animalFood[current][i].GetComponent<Dissolve>().isDissolving = true;
                GameObject currentFood = animalFood[current][i];
                animalFood[current].Remove(currentFood);
                Destroy(currentFood);
            }
            yield return new WaitForSeconds(0.5f);
            powersDisappearance(axes);
            sparks.SetActive(false);
            PowersFlow();
        }

        IEnumerator teleportAnim()
        {
            GameObject lastTeleport = teleports[current][teleports[current].Count - 1];
            StartCoroutine(AccentOnCard(lastTeleport));
            yield return new WaitForSeconds(0.6f);

            while (teleports[current].Count != 0)
            {
                int positionForTeleport = gameManager.currentIndexesOfAnimals.Max();
                for (int i = 0; i < amountOfAnimals; i++)
                {
                    if (gameManager.currentIndexesOfAnimals[i] > gameManager.currentIndexesOfAnimals[current] &&
                        gameManager.currentIndexesOfAnimals[i] <= positionForTeleport)
                    {
                        positionForTeleport = gameManager.currentIndexesOfAnimals[i];
                    }
                }

                //Debug.Log(positionForTeleport + " - позиція для телепорту");
                //Debug.Log(gameManager.currentIndexesOfAnimals[current] + " - позиція нинішня");

                if (positionForTeleport == gameManager.currentIndexesOfAnimals[current])
                {
                    StartCoroutine(RemoveTeleport(lastTeleport));
                    yield return new WaitForSeconds(0.8f);
                    PowersFlow();
                }
                else
                {
                    List<int> candidatesForTeleport = new List<int>();
                    for (int i = 0; i < amountOfAnimals; i++)
                    {
                        if (gameManager.currentIndexesOfAnimals[i] == positionForTeleport)
                        {
                            candidatesForTeleport.Add(i);
                        }
                    }

                    if (candidatesForTeleport.Count == 1)
                    {
                        if (shields[candidatesForTeleport[0]].Count == 0)
                        {
                            SwapAnimals(candidatesForTeleport[0]);
                            StartCoroutine(RemoveTeleport(lastTeleport));
                        }
                        else
                        {
                            // якісь ефектики типу щит спрацював, а телепорт ні, ха
                            StartCoroutine(moveToBox(shields[candidatesForTeleport[0]][0]));
                            StartCoroutine(moveToBox(teleports[current][0]));
                            yield return new WaitForSeconds(duration);
                            PowersFlow();
                        }
                    }

                    else if (candidatesForTeleport.Count > 1)
                    {
                        // можна зробити анімацією
                        choiseForTeleport.SetActive(true);
                        for (int i = 0; i < candidatesForTeleport.Count; i++)
                        {
                            choiseButtons[i].SetActive(true);
                            choiseButtons[i].GetComponent<Image>().sprite = frogSprites[candidatesForTeleport[i]];
                        }
                    }
                    StartCoroutine(RemoveTeleport(lastTeleport));
                }
            }
        }

        IEnumerator webAnim()
        {
            GameObject lastWeb = webs[current][webs[current].Count - 1];
            webs[current].Remove(lastWeb);
            StartCoroutine(AccentOnCard(lastWeb));
            yield return new WaitForSeconds(0.6f);

            Vector3 newPosition = (gameManager.slots[gameManager.currentIndexesOfAnimals[current]].position + gameManager.slots[gameManager.currentIndexesOfAnimals[current] - 1].position) / 2;
            float distance = Vector3.Distance(lastWeb.transform.position, newPosition);
            LeanTween.move(lastWeb, newPosition, distance / speed);
            yield return new WaitForSeconds(distance / speed);

            PowersFlow();

        }

        IEnumerator speedupAnim()
        {
            GameObject lastSpeedup = speedups[current][speedups[current].Count - 1];
            StartCoroutine(AccentOnCard(lastSpeedup));

            yield return new WaitForSeconds(0.6f);
            gameManager.animals[current].GetComponent<FrogMovement>().Move(2);
            yield return new WaitForSeconds(2.5f);

            LeanTween.scale(lastSpeedup, Vector3.zero, 0.8f).setEase(LeanTweenType.easeInOutBack);
            yield return new WaitForSeconds(0.8f);
            speedups[current].Remove(lastSpeedup);
            Destroy(lastSpeedup);

            PowersFlow();
        }

        public IEnumerator powersDisappearance(List<GameObject>[] powers)
        {
            for (int i = 0; i < amountOfAnimals; i++)
            {
                for (int j = 0; j < powers[i].Count; j++)
                {
                    LeanTween.scale(powers[i][j], Vector3.zero, 0.8f).setEase(LeanTweenType.easeInOutBack);
                    yield return new WaitForSeconds(0.8f);
                    powers[i].Remove(powers[i][j]);
                    Destroy(powers[i][j]);
                }
            }
        }

        public void chooseAnimalButtonClicked()
        {
            int animalForTeleport = -1;
            for (int i = 0; i < amountOfAnimals; i++)
            {
                if (GetComponent<Image>().sprite == frogSprites[i])
                {
                    animalForTeleport = i;
                    break;
                }
            }

            if (shields[animalForTeleport].Count == 0)
            {
                SwapAnimals(animalForTeleport);
                StartCoroutine(RemoveTeleport(teleports[current][teleports[current].Count - 1]));
            }
            else
            {
                StartCoroutine(moveToBox(shields[animalForTeleport][0]));
                StartCoroutine(moveToBox(teleports[current][0]));
            }
        }

        //****************** Допоміжні методи *********************//
        public IEnumerator RemoveTeleport(GameObject power)
        {
            LeanTween.scale(power, Vector3.zero, 0.8f).setEase(LeanTweenType.easeInOutBack);
            yield return new WaitForSeconds(0.8f);
            teleports[current].Remove(power);
            Destroy(power);
        }

        void SwapAnimals(int animalForTeleport)
        {
            gameManager.animals[current].GetComponent<FrogMovement>().Move(gameManager.animals[animalForTeleport].transform.position, true);
            gameManager.animals[animalForTeleport].GetComponent<FrogMovement>().Move(gameManager.animals[current].transform.position, false);

            int temp = gameManager.currentIndexesOfAnimals[current];
            gameManager.currentIndexesOfAnimals[animalForTeleport] = gameManager.currentIndexesOfAnimals[current];
            gameManager.currentIndexesOfAnimals[current] = temp;

        }

        public IEnumerator AccentOnCard(GameObject card)
        {
            LeanTween.scale(card, card.transform.localScale * 1.3f, 0.2f);
            yield return new WaitForSeconds(0.2f);
            LeanTween.scale(card, card.transform.localScale / 1.3f, 0.2f);
        }

        IEnumerator moveToBox(GameObject gameObject, List<GameObject> list)
        {
            LeanTween.move(gameObject, openedCan.position, duration);
            list.Remove(gameObject);
            yield return new WaitForSeconds(duration);
            Destroy(gameObject);
        }

        public IEnumerator moveToBox(GameObject gameObject)
        {
            LeanTween.move(gameObject, openedCan.position, duration);
            yield return new WaitForSeconds(duration);
            Destroy(gameObject);
        }
    }
}
