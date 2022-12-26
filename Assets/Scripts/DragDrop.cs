using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FeedFrogGame
{
    public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public Canvas canvas;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();

            if (canvas == null)
            {
                Transform testCanvasTransform = transform.parent;
                while (testCanvasTransform != null)
                {
                    canvas = testCanvasTransform.GetComponent<Canvas>();
                    if (canvas != null)
                    {
                        break;
                    }
                    testCanvasTransform = testCanvasTransform.parent;
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (GameManager.MovementIsAllowed && canvasGroup != null)
            {
                canvasGroup.alpha = 0.75f;
                canvasGroup.blocksRaycasts = false;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1;
                canvasGroup.blocksRaycasts = true;
            }
        }
    }
}
