using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SpeachBalloon : MonoBehaviour
{
    [SerializeField] GameObject[] points;
    bool isPointAnim = false;
    const int pointNum = 3;

    private void OnDisable()
    {
        isPointAnim = false;        // ��\�����t���O��false�ɂ���
    }

    private void FixedUpdate()
    {
        SpeachBalloonAnim();
    }

    private void SpeachBalloonAnim() // �����o���A�j���[�V�����֐�
    {
        if (!isPointAnim)
        {
            isPointAnim = true;
            for (int i = 0; i < pointNum; i++)
            {
                points[i].transform.DOScale(0, 0.1f).SetLink(gameObject);
            }
            StartCoroutine(SpeachBalloonDisplayAnim());
        }
    }

    IEnumerator SpeachBalloonDisplayAnim()   // �����o���_�\���A�j���[�V����
    {
        for (int i = 0; i < pointNum; i++)
        {
            yield return new WaitForSeconds(0.5f);
            points[i].transform.DOScale(1, 0.1f).SetLink(gameObject);
        }
        yield return new WaitForSeconds(0.5f);
        isPointAnim = false;
    }
}
