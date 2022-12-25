using System.Collections;
using UnityEngine;

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
                points[i].SetActive(false);
            }
            StartCoroutine(SpeachBalloonDisplayAnim());
        }
    }

    IEnumerator SpeachBalloonDisplayAnim()   // �����o���_�\���A�j���[�V����
    {
        for (int i = 0; i < pointNum; i++)
        {
            yield return new WaitForSeconds(0.5f);
            points[i].SetActive(true);
        }
        yield return new WaitForSeconds(0.5f);
        isPointAnim = false;
    }
}
