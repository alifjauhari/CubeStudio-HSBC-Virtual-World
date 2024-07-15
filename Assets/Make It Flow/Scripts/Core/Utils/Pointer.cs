using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeadowGames.MakeItFlow
{
    public class Pointer : MonoBehaviour, IUpdateEvent
    {
        CanvasManager _canvasManager;
        
        void Awake()
        {
            _canvasManager = transform.GetComponentInParent<CanvasManager>();
        }

        void OnEnable()
        {
            MFSystemManager.AddToUpdate(this);
        }
        void OnDisable()
        {
            MFSystemManager.RemoveFromUpdate(this);
        }

        void Start()
        {
            Cursor.visible = false;
        }

        public void OnUpdate()
        {
            transform.position = InputManager.Instance.GetCanvasPointerPosition(_canvasManager);
        }
    }
}