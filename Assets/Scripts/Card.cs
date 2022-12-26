using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace FeedFrogGame
{
    public class Card : MonoBehaviour
    {
        public int AnimalChoise;
        public bool FaceFront;

        public string superpower;

        Dictionary<int, string> superpowers = new();
        List<int> probabilities;

        private void Awake()
        {
            probabilities = new List<int> { 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5 };

            superpowers[1] = "shield";
            superpowers[2] = "axe";
            superpowers[3] = "teleport";
            superpowers[4] = "web";
            superpowers[5] = "speedup";

            int number = probabilities[Random.Range(0, probabilities.Count)];
            superpower = superpowers[number];
            probabilities.Remove(number);
            FaceFront = true;

        }

        public void AddDragDrop()
        {
            this.AddComponent<Flip>();
            this.AddComponent<DragDrop>();
        }

        public void RemoveDragDrop()
        {
            Destroy(GetComponent<Flip>());
            Destroy(GetComponent<DragDrop>());
        }

        public void WebCheck()
        {
            if (superpower == "web")
            {
                this.AddComponent<BoxCollider2D>();
                //this.AddComponent<Rigidbody2D>();
                //GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                GetComponent<BoxCollider2D>().size = new Vector2(50, 100);
            }
        }
    }
}
