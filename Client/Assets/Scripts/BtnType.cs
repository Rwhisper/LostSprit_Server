using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnType : MonoBehaviour
{
    public BTNType currentType;

    public CanvasGroup mainGroup;
    public CanvasGroup optionGroup;
    public void OnBtnClick()
    {
        switch (currentType)
        {
            case BTNType.Fire:
                Debug.Log("��");
                break;
            case BTNType.Water:
                Debug.Log("��");
                break;
            case BTNType.Quit:
                Debug.Log("����");
                break;
        }
    }

  

}
