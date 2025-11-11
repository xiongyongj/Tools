using System.Collections;
using System.Collections.Generic;
using BookCurlPro;
using English.Readbook;
using UnityEngine;

namespace English.Readbook {
    public class AutoFlipBook : MonoBehaviour {
        [HideInInspector] public BookPro Book;

        public float PageFlipTime = 1;

        private void Awake() {
            BookSystem.IsPageFlipping = false;
            Register();
        }

        private void OnDestroy() {
            Unregister();
        }

        private void Update() {
            if (Book == null) {
                return;
            }

            if (Book.interactable && Book.OnMouseDragging && BookSystem.IsAutoRead) {
                BookSystem.IsAutoRead = false;
                BookSystem.OnSwitchAutoRead();
            }
        }

        private void Register() {
            BookSystem.EventPageReadComplete += OnPageReadComplete;
            BookSystem.EventSwtichAutoRead += OnSwitchAutoRead;
        }

        private void Unregister() {
            BookSystem.EventPageReadComplete -= OnPageReadComplete;
            BookSystem.EventSwtichAutoRead -= OnSwitchAutoRead;
        }

        private void OnSwitchAutoRead() {
            if (!BookSystem.IsAutoRead) {
                return;
            }

            if (BookSystem.IsPageFlipping || BookSystem.IsReading) {
                return;
            }

            OnPageReadComplete(Book.currentPaper);
        }

        private void OnPageReadComplete(int pageIndex) {
            if (!BookSystem.IsAutoRead) {
                return;
            }

            if (pageIndex > Book.EndFlippingPaper) {
                return;
            }

            if (BookSystem.IsPageFlipping) {
                return;
            }
            BookSystem.IsPageFlipping = true;
            Book.interactable = false;

            PageFlipper.FlipPage(Book, PageFlipTime, FlipMode.RightToLeft, OnFlipComplete);
        }

        private void OnFlipComplete() {
            BookSystem.IsPageFlipping = false;
            Book.interactable = true;
        }
    }
}
