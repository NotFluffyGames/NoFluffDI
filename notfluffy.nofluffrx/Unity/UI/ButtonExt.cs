using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Subjects;
using UnityEngine;
using UnityEngine.UI;

namespace NotFluffy.NoFluffRx.Unity
{
    public static class ButtonExt
    {
        public static IObservable<Unit> OnClickAsObservable(this Button button)
        {
            LazySubject<Unit> subject = null;
            subject = new LazySubject<Unit>(OnHot, OnCold);
            subject.AddTo(button);
            return subject;

            void OnHot()
            {
                button.onClick.AddListener(OnClick);
            }

            void OnCold()
            {
                button.onClick.RemoveListener(OnClick);
            }

            void OnClick()
            {
                subject.OnNext();
            }
        }
    }
}
