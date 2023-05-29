using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        rect.position = Camera.main.WorldToScreenPoint(GameManager.instance.player.transform.position);
    }
}
