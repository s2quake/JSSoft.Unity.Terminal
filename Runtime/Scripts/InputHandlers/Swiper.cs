////////////////////////////////////////////////////////////////////////////////
//                                                                              
// ██╗   ██╗   ████████╗███████╗██████╗ ███╗   ███╗██╗███╗   ██╗ █████╗ ██╗     
// ██║   ██║   ╚══██╔══╝██╔════╝██╔══██╗████╗ ████║██║████╗  ██║██╔══██╗██║     
// ██║   ██║█████╗██║   █████╗  ██████╔╝██╔████╔██║██║██╔██╗ ██║███████║██║     
// ██║   ██║╚════╝██║   ██╔══╝  ██╔══██╗██║╚██╔╝██║██║██║╚██╗██║██╔══██║██║     
// ╚██████╔╝      ██║   ███████╗██║  ██║██║ ╚═╝ ██║██║██║ ╚████║██║  ██║███████╗
//  ╚═════╝       ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝
//
// Copyright (c) 2020 Jeesu Choi
// E-mail: han0210@netsgo.com
// Site : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;

namespace JSSoft.Unity.Terminal.InputHandlers
{
    class Swiper
    {
        private float startTime;
        private Vector2 startPos;

        public void Update()
        {
            if (Input.touchCount > 0 && Input.GetTouch(0) is Touch touch)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    this.OnBegan(touch);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    this.OnEnded(touch);
                }
            }
        }

        public float MaxTime { get; set; } = 1.0f;

        public float MinSwipeDist { get; set; } = 2.0f;

        public event EventHandler<SwipedEventArgs> Swiped;

        protected virtual void OnSwiped(SwipedEventArgs e)
        {
            this.Swiped?.Invoke(this, e);
        }

        private void OnBegan(Touch touch)
        {
            this.startTime = Time.time;
            this.startPos = touch.position;
        }

        private void OnEnded(Touch touch)
        {
            var startTime = this.startTime;
            var startPos = this.startPos;
            var endTime = Time.time;
            var endPos = touch.position;
            var swipeDistance = (endPos - startPos).magnitude;
            var swipeTime = endTime - startTime;

            if (swipeTime < this.MaxTime && swipeDistance > this.MinSwipeDist)
            {
                this.Swipe(startPos, endPos);
            }
        }

        private void Swipe(Vector2 startPos, Vector2 endPos)
        {
            var distance = endPos - startPos;
            if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
            {
                if (startPos.x < endPos.x)
                {
                    this.OnSwiped(new SwipedEventArgs(SwipeDirection.Right, startPos, endPos));
                }
                else
                {
                    this.OnSwiped(new SwipedEventArgs(SwipeDirection.Left, startPos, endPos));
                }
            }
            else if (Mathf.Abs(distance.x) < Mathf.Abs(distance.y))
            {
                if (startPos.y < endPos.y)
                {
                    this.OnSwiped(new SwipedEventArgs(SwipeDirection.Up, startPos, endPos));
                }
                else
                {
                    this.OnSwiped(new SwipedEventArgs(SwipeDirection.Down, startPos, endPos));
                }
            }
        }
    }
}
