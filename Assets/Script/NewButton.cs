using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Engine
{
    public class NewButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        private enum ActionType
        {
            PointerDown,
            PointerUp,
        }

        [HideInInspector]
        public UnityEvent onClick = new UnityEvent();
        [HideInInspector]
        public UnityEvent onPointerDown = new UnityEvent();
        [HideInInspector]
        public UnityEvent onPointerUp = new UnityEvent();

        // anim
        public bool isAnim = false;
        private Vector3 originScale = Vector3.one;
        private Vector3 animScale = Vector3.one;
        public float scale = 1.0f;
        public float playAnimTime = 0.2f;

        // lick limit
        //[Range(0.0f, float.MaxValue)]
        public float clickInternal = 0.0f;
        private float disableClickTime = 0.0f;

        private Tween tween;
        private RectTransform rt;

        private bool _interactable = true;
        public bool interactable
        {
            get
            {
                return _interactable;
            }
            set
            {
                _interactable = value;
            }
        }

        private void OnEnable()
        {
            
        }

        private void Awake()
        {
            clickInternal = clickInternal > 0.0f ? clickInternal : 0.0f;
            if (isAnim)
            {
                originScale = transform.localScale;
                animScale = originScale * scale;
            }

            rt = transform as RectTransform;
        }

        private void PlayAnim(ActionType actionType)
        {
            if (!isAnim)
            {
                return;
            }

            Vector3 targetScale = actionType == ActionType.PointerDown ? animScale : originScale;
            ClearAnim();

            tween = transform.DOScale(targetScale, playAnimTime);
        }

        private void ClearAnim()
        {
            if (tween != null && tween.IsActive())
            {
                tween.Pause();
                tween.Kill();
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }

            if (Time.time < disableClickTime)
            {
                return;
            }

            disableClickTime = Time.time + clickInternal;
            PlayAnim(ActionType.PointerDown);
            Debug.Log("NewButton OnPointerDown");
            onPointerDown.Invoke();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }
            PlayAnim(ActionType.PointerUp);
            Debug.Log("NewButton OnPointerUp");
            onPointerUp.Invoke();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }
            Debug.Log("NewButton OnPointerClick");
            onClick.Invoke();
        }

        public void RemoveAllListeners()
        {
            onClick.RemoveAllListeners();
            onPointerUp.RemoveAllListeners();
            onPointerDown.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            RemoveAllListeners();
            ClearAnim();
        }
    }
}


