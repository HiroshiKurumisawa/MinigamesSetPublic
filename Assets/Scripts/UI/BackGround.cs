using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BackGround : MonoBehaviour
{
    private const float duration = 100f;
    [SerializeField] Image image;
    [SerializeField] Vector2 step = new Vector2(1, 1);
    private void Start()
    {
        image.material.mainTextureOffset = Vector2.zero;
        image.material.DOOffset(step, duration).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear).SetLink(gameObject);
    }
}
