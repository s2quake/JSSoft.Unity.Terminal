// MIT License
// 
// Copyright (c) 2019 Jeesu Choi
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using UnityEngine;

namespace JSSoft.UI.InputHandlers
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
