using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NearbyText : MonoBehaviour
{

    // text to show on items in player is nearby using speechbubbles

    public SpeechBubble SpeechBubblePreFab;

    private bool IsShowingText = false;

    private SpeechBubble text;

    private ItemController BaseItem;

    private Transform TextTransform;

    void Start()
    {
        BaseItem = this.gameObject.GetComponentInParent<ItemController>();
        TextTransform = this.gameObject.GetComponent<Transform>();
    }

    void Update()
    {
        if (IsShowingText)
        {

            //TextTransform.position = new Vector3(TextTransform.position.x, BaseItem.GetItemTransform().position.y, TextTransform.position.z);

            if (BaseItem.GetIsPickedUp())
            {
                DisableText();
                IsShowingText = false;

            }
        }

    }
    private void OnTriggerEnter(Collider EnteringCharacter)
    {
        if (!IsShowingText && !BaseItem.GetIsPickedUp())
        {
            //Debug.Log("enter" + EnteringCharacter);

            CharacterController EnteringCharacterController = EnteringCharacter.GetComponent<CharacterController>();
            if (EnteringCharacterController != null)
            {
                if (EnteringCharacterController.GetIsPlayer())
                {
                    EnableText();

                }
            }

        }
    }

    private void OnTriggerExit(Collider EnteringCharacter)
    {
        if (IsShowingText)
        {
            //Debug.Log("exit" + EnteringCharacter);

            CharacterController EnteringCharacterController = EnteringCharacter.GetComponent<CharacterController>();
            if (EnteringCharacterController != null)
            {
                //disable text
                if (EnteringCharacterController.GetIsPlayer())
                {
                    DisableText();
                }
            }

        }
    }


    public void EnableText()
    {
        IsShowingText = true;

        string WhatToSay = BaseItem.GetSummaryString();
        Vector3 SummonPositon = this.transform.position + new Vector3(0.0f, 2.0f, 0.0f);
        text = Instantiate(SpeechBubblePreFab, SummonPositon, Quaternion.identity);
        text.GetComponent<SpeechBubble>().SetText(WhatToSay);
        text.GetComponent<SpeechBubble>().SetDoCountdown(false);
        //text.gameObject.GetComponent<Transform>().parent = this.transform;

    }

    private void DisableText()
    {
        IsShowingText = false;

        if (text != null)
        {
            text.GetComponent<SpeechBubble>().SetDoCountdown(true);
            text.GetComponent<SpeechBubble>().SetTime(0);

        }
    }
}
