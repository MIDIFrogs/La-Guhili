using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Ult : MonoBehaviour
{
    public Image ultImage;
    public float constReloadTime = 25.0f;
    public float constUltimateTime = 10.0f;

    public Animator frogHeadAnimator;
    public RawImage headRawImage;

    private Coroutine headFadeCoroutine;
    private Coroutine ultSequenceCoroutine;

    private Letter highlightLetter;
    public Letter HighlightLetter
    {
        get => highlightLetter;
        set
        {
            if (highlightLetter != null)
                highlightLetter.LightOff();
            highlightLetter = value;
        }
    }

    private float reloadTime;
    private float ultimateTime;
    public bool isUltNow = false;

    private void Awake()
    {
        reloadTime = constReloadTime;
        ultimateTime = constUltimateTime;

        // Установить прозрачность головы в 0 при старте
        if (headRawImage != null)
        {
            Color c = headRawImage.color;
            headRawImage.color = new Color(c.r, c.g, c.b, 0f);
        }
    }

    private void Update()
    {
        if (!isUltNow)
        {
            reloadTime -= Time.deltaTime;
            if (reloadTime < 0 && Input.GetKeyDown(KeyCode.F))
            {
                UltOn();
            }
        }
        else
        {
            ultimateTime -= Time.deltaTime;
            ActiveUlt();

            if (ultimateTime <= 0f)
            {
                UltOff();
            }
        }

        Display();
    }


    public void Display()
    {
        if (isUltNow)
        {
            ultImage.fillAmount = Mathf.Clamp01(ultimateTime / constUltimateTime);
        }
        else
        {
            ultImage.fillAmount = Mathf.Clamp01((constReloadTime - reloadTime) / constReloadTime);
        }
    }

    public void UltOn()
    {
        isUltNow = true;
        reloadTime = constReloadTime;
        ultimateTime = constUltimateTime;

        if (ultSequenceCoroutine != null)
            StopCoroutine(ultSequenceCoroutine);

        ultSequenceCoroutine = StartCoroutine(UltSequence());
    }

    public void UltOff()
    {
        isUltNow = false;
        HighlightLetter = null;
    }

    public void ActiveUlt()
    {
        if (highlightLetter != null)
        {
            highlightLetter.LightOn();
        }
    }

    private IEnumerator UltSequence()
    {
        // Плавное проявление головы и запуск анимации одновременно
        FadeHeadTo(1f, 0.5f);
        frogHeadAnimator.Play("enlightenment", -1, 0f);

        // Ждём окончания анимации
        AnimatorStateInfo stateInfo = frogHeadAnimator.GetCurrentAnimatorStateInfo(0);
        while (stateInfo.IsName("enlightenment") && stateInfo.normalizedTime < 1f)
        {
            stateInfo = frogHeadAnimator.GetCurrentAnimatorStateInfo(0);
            ActiveUlt();
            Display();
            yield return null;
        }

        // Небольшая пауза после завершения анимации
        yield return new WaitForSeconds(0.4f);


        // Плавное исчезновение головы
        FadeHeadTo(0f, 0.5f);
    }


    public void FadeHeadTo(float targetAlpha, float duration)
    {
        if (headFadeCoroutine != null)
            StopCoroutine(headFadeCoroutine);

        float currentAlpha = headRawImage.color.a;
        headFadeCoroutine = StartCoroutine(FadeRoutine(currentAlpha, targetAlpha, duration));
    }

    private IEnumerator FadeRoutine(float from, float to, float duration)
    {
        float t = 0f;
        Color baseColor = headRawImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            headRawImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
            yield return null;
        }

        headRawImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, to);
    }
}
