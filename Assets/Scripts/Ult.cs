using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class Ult: MonoBehaviour
{
    public Image ultImage;
    public float constReloadTime = 25.0f;
    public float constUltimateTime = 10.0f;

    private Letter highlightLetter;
    public Letter HighlightLetter
    {
        get
        {
            return highlightLetter;
        }
        set
        {
            if(highlightLetter != null)
            {
                highlightLetter.LightOff();
            }
            highlightLetter = value;
        }
    }


    private float reloadTime = 25f;
    private float ultimateTime = 10f;

    public bool isUltNow = false;

    private void Awake()
    {
        reloadTime = constReloadTime;
        ultimateTime = constUltimateTime;
    }

    private void Update()
    {
        if (isUltNow)
        {
            ActiveUlt();
            ultimateTime -= Time.deltaTime;
            if (ultimateTime < 0)
            {
                UltOff();
             
            }
        }
        else
        {
            reloadTime -= Time.deltaTime;
            if (reloadTime < 0 && Input.GetKeyDown(KeyCode.F)) {
                UltOn();
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
            ultImage.fillAmount = Mathf.Clamp01((constReloadTime-reloadTime)/constReloadTime);
        }
    }

    public void UltOn()
    {
        isUltNow = true;
        ultimateTime = constUltimateTime;
    }

    public void UltOff()
    {
        isUltNow = false;
        HighlightLetter = null;
        reloadTime = constReloadTime;
    }

    public void ActiveUlt()
    {
        if(highlightLetter != null)
        {
            highlightLetter.LightOn();
        }
    }
}
