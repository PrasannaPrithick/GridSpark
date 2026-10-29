using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Aura
{
    public static class Motion
    {
        public static float OutCubic(float t)
        {
            t = Mathf.Clamp01(t);
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        public static float OutQuad(float t)
        {
            t = Mathf.Clamp01(t);
            return 1f - (1f - t) * (1f - t);
        }

        public static float OutBack(float t)
        {
            t = Mathf.Clamp01(t);
            const float s = 1.45f;
            t -= 1f;
            return t * t * ((s + 1f) * t + s) + 1f;
        }

        public static float InCubic(float t)
        {
            t = Mathf.Clamp01(t);
            return t * t * t;
        }

        public static IEnumerator Wait(float seconds)
        {
            float t = 0f;
            while (t < seconds)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        public static IEnumerator Scale(Transform target, Vector3 to, float duration, Func<float, float> ease = null)
        {
            if (target == null)
                yield break;
            Vector3 from = target.localScale;
            yield return Tween(duration, u =>
            {
                if (target != null)
                    target.localScale = Vector3.LerpUnclamped(from, to, ease != null ? ease(u) : OutCubic(u));
            });
            if (target != null)
                target.localScale = to;
        }

        public static IEnumerator Move(RectTransform target, Vector2 to, float duration, Func<float, float> ease = null)
        {
            if (target == null)
                yield break;
            Vector2 from = target.anchoredPosition;
            yield return Tween(duration, u =>
            {
                if (target != null)
                    target.anchoredPosition = Vector2.LerpUnclamped(from, to, ease != null ? ease(u) : OutCubic(u));
            });
            if (target != null)
                target.anchoredPosition = to;
        }

        public static IEnumerator Size(RectTransform target, Vector2 to, float duration, Func<float, float> ease = null)
        {
            if (target == null)
                yield break;
            Vector2 from = target.sizeDelta;
            yield return Tween(duration, u =>
            {
                if (target != null)
                    target.sizeDelta = Vector2.LerpUnclamped(from, to, ease != null ? ease(u) : OutCubic(u));
            });
            if (target != null)
                target.sizeDelta = to;
        }

        public static IEnumerator Alpha(CanvasGroup group, float to, float duration)
        {
            if (group == null)
                yield break;
            float from = group.alpha;
            yield return Tween(duration, u =>
            {
                if (group != null)
                    group.alpha = Mathf.Lerp(from, to, OutCubic(u));
            });
            if (group != null)
                group.alpha = to;
        }

        public static IEnumerator ColorTo(Graphic graphic, Color to, float duration)
        {
            if (graphic == null)
                yield break;
            Color from = graphic.color;
            yield return Tween(duration, u =>
            {
                if (graphic != null)
                    graphic.color = Color.Lerp(from, to, OutCubic(u));
            });
            if (graphic != null)
                graphic.color = to;
        }

        public static IEnumerator Punch(Transform target, float peak, float duration)
        {
            if (target == null)
                yield break;
            Vector3 baseScale = Vector3.one;
            float up = duration * 0.4f;
            yield return Scale(target, baseScale * peak, up, OutCubic);
            yield return Scale(target, baseScale, Mathf.Max(0.04f, duration - up), OutCubic);
        }

        public static void HideForSit(RectTransform target)
        {
            if (target == null)
                return;
            target.localScale = Vector3.zero;
            target.localRotation = Quaternion.identity;
        }

        public static IEnumerator FallFromCamera(RectTransform target, float duration = 0.4f, float height = 560f, float startScale = 3.2f)
        {
            if (target == null)
                yield break;
            Vector2 rest = target.anchoredPosition;
            target.anchoredPosition = rest + new Vector2(0f, height);
            target.localScale = Vector3.one * startScale;
            target.localRotation = Quaternion.Euler(0f, 0f, 16f);
            yield return Tween(duration, u =>
            {
                if (target == null)
                    return;
                float drop = InCubic(u);
                float settle = OutCubic(u);
                target.anchoredPosition = Vector2.LerpUnclamped(rest + new Vector2(0f, height), rest, drop);
                target.localScale = Vector3.one * Mathf.LerpUnclamped(startScale, 1f, settle);
                target.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(16f, 0f, settle));
            });
            if (target == null)
                yield break;
            target.anchoredPosition = rest;
            target.localScale = Vector3.one;
            target.localRotation = Quaternion.identity;
        }

        public static IEnumerator ImpactSquash(RectTransform target, float duration = 0.22f)
        {
            if (target == null)
                yield break;
            yield return Tween(duration, u =>
            {
                if (target == null)
                    return;
                if (u < 0.34f)
                {
                    float k = u / 0.34f;
                    target.localScale = new Vector3(
                        Mathf.Lerp(1f, 1.32f, k),
                        Mathf.Lerp(1f, 0.62f, k),
                        1f);
                }
                else
                {
                    float k = OutBack((u - 0.34f) / 0.66f);
                    target.localScale = new Vector3(
                        Mathf.LerpUnclamped(1.32f, 1f, k),
                        Mathf.LerpUnclamped(0.62f, 1f, k),
                        1f);
                }
            });
            if (target != null)
                target.localScale = Vector3.one;
        }

        public static IEnumerator Lift(RectTransform target, float height, float peakScale, float duration)
        {
            if (target == null)
                yield break;
            Vector2 rest = target.anchoredPosition;
            Vector2 up = rest + new Vector2(0f, height);
            yield return Tween(duration, u =>
            {
                if (target == null)
                    return;
                float k = OutCubic(u);
                target.anchoredPosition = Vector2.LerpUnclamped(rest, up, k);
                target.localScale = Vector3.one * Mathf.LerpUnclamped(1f, peakScale, k);
            });
            if (target == null)
                yield break;
            target.anchoredPosition = up;
            target.localScale = Vector3.one * peakScale;
        }

        public static IEnumerator Sit(RectTransform target, float duration = 0.3f, float drop = 18f, float twist = 6f)
        {
            if (target == null)
                yield break;
            Vector2 rest = target.anchoredPosition;
            target.anchoredPosition = rest + new Vector2(0f, drop);
            target.localRotation = Quaternion.Euler(0f, 0f, twist * 0.5f);
            yield return Tween(duration, u =>
            {
                if (target == null)
                    return;
                float dropK = OutCubic(u);
                target.anchoredPosition = Vector2.LerpUnclamped(rest + new Vector2(0f, drop), rest, dropK);
                float grow = Mathf.LerpUnclamped(0.4f, 1f, OutBack(u));
                float damp = 1f - u;
                float wobble = Mathf.Sin(u * Mathf.PI * 2.4f) * damp;
                target.localScale = new Vector3(grow * (1f + wobble * 0.1f), grow * (1f - wobble * 0.1f), 1f);
                target.localRotation = Quaternion.Euler(0f, 0f, twist * wobble);
            });
            if (target == null)
                yield break;
            target.anchoredPosition = rest;
            target.localScale = Vector3.one;
            target.localRotation = Quaternion.identity;
        }

        public static IEnumerator PopIn(Transform target, float duration = 0.24f)
        {
            if (target == null)
                yield break;
            target.localScale = Vector3.one * 0.84f;
            yield return Scale(target, Vector3.one, duration, OutBack);
        }

        public static void HideForDeal(RectTransform target)
        {
            if (target == null)
                return;
            target.localScale = Vector3.one;
            target.localRotation = Quaternion.identity;
            var group = target.GetComponent<CanvasGroup>();
            if (group == null)
                group = target.gameObject.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            group.blocksRaycasts = false;
        }

        public static IEnumerator DealIn(RectTransform target, bool fromLeft = true, float duration = 0.34f, float slide = 110f)
        {
            if (target == null)
                yield break;
            var group = target.GetComponent<CanvasGroup>();
            if (group == null)
                group = target.gameObject.AddComponent<CanvasGroup>();
            Vector2 rest = target.anchoredPosition;
            Vector2 start = rest + new Vector2(fromLeft ? -slide : slide, 0f);
            target.anchoredPosition = start;
            target.localScale = Vector3.one;
            target.localRotation = Quaternion.identity;
            group.alpha = 0f;
            yield return Tween(duration, u =>
            {
                if (target == null)
                    return;
                float k = OutCubic(u);
                float arc = Mathf.Sin(k * Mathf.PI) * 22f;
                target.anchoredPosition = Vector2.LerpUnclamped(start, rest, k) + new Vector2(0f, arc);
                group.alpha = k;
            });
            if (target == null)
                yield break;
            target.anchoredPosition = rest;
            target.localScale = Vector3.one;
            target.localRotation = Quaternion.identity;
            group.alpha = 1f;
            group.blocksRaycasts = true;
        }

        public static IEnumerator RotateZ(Transform target, float from, float to, float duration)
        {
            if (target == null)
                yield break;
            yield return Tween(duration, u =>
            {
                if (target != null)
                    target.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(from, to, OutCubic(u)));
            });
            if (target != null)
                target.localRotation = Quaternion.Euler(0f, 0f, to);
        }

        public static IEnumerator SpinTurn(RectTransform target, Vector2 toSize, float duration = 0.2f, float angle = -90f)
        {
            if (target == null)
                yield break;
            yield return Tween(duration, u =>
            {
                if (target == null)
                    return;
                float k = OutCubic(u);
                target.localRotation = Quaternion.Euler(0f, 0f, angle * k);
                float pulse = 1f + 0.16f * Mathf.Sin(k * Mathf.PI);
                target.localScale = Vector3.one * pulse;
            });
            if (target == null)
                yield break;
            target.localRotation = Quaternion.identity;
            target.sizeDelta = toSize;
            target.localScale = Vector3.one;
        }

        public static IEnumerator Arc(RectTransform target, Vector2 from, Vector2 to, Vector2 fromSize, Vector2 toSize, float lift, float duration)
        {
            if (target == null)
                yield break;
            yield return Tween(duration, u =>
            {
                if (target == null)
                    return;
                float s = OutCubic(u);
                float arc = Mathf.Sin(s * Mathf.PI) * lift;
                target.anchoredPosition = Vector2.Lerp(from, to, s) + new Vector2(0f, arc);
                target.sizeDelta = Vector2.Lerp(fromSize, toSize, s);
                target.localScale = Vector3.one * (1f + 0.08f * Mathf.Sin(s * Mathf.PI));
            });
            if (target == null)
                yield break;
            target.anchoredPosition = to;
            target.sizeDelta = toSize;
            target.localScale = Vector3.one;
        }

        public static IEnumerator ShakeX(RectTransform target, float amount, float duration)
        {
            if (target == null)
                yield break;
            Vector2 origin = target.anchoredPosition;
            yield return Tween(duration, u =>
            {
                if (target == null)
                    return;
                float wave = Mathf.Sin(u * Mathf.PI * 3f) * (1f - u);
                target.anchoredPosition = origin + new Vector2(wave * amount, 0f);
            });
            if (target != null)
                target.anchoredPosition = origin;
        }

        public static IEnumerator Tween(float duration, Action<float> step)
        {
            float t = 0f;
            duration = Mathf.Max(0.01f, duration);
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / duration;
                step(Mathf.Clamp01(t));
                yield return null;
            }
            step(1f);
        }
    }
}
