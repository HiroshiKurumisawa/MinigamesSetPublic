using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// �V�[���J�ڗp�ȈՃt�F�[�h�N���X
/// </summary>
public class FadeManager : MonoBehaviour
{
    private static Canvas canvas;
    private static Image image;
    private static Text loadingText;

    private static FadeManager instance;
    public static FadeManager Instance
    {
        get
        {
            if (instance == null) { Init(); }
            return instance;
        }
    }

    IEnumerator fadeCoroutine = null;
    AsyncOperation async;

    private FadeManager() { }

    private static void Init()
    {
        // Canvas�쐬
        GameObject canvasObject = new GameObject("CanvasFade");
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        // Image�쐬
        image = new GameObject("ImageFade").AddComponent<Image>();
        image.transform.SetParent(canvas.transform, false);

        // ��ʒ������A���J�[�Ƃ��AImage�̃T�C�Y���X�N���[���T�C�Y�ɍ��킹��
        image.rectTransform.anchoredPosition = Vector3.zero;
        image.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

        // Text�쐬(Now Loding)
        loadingText = new GameObject("LoadingText").AddComponent<Text>();
        loadingText.transform.SetParent(canvas.transform, false);
        //�t�H���g�A���A�����T�C�Y�C�������낦�Ȃǂ�������
        loadingText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        loadingText.rectTransform.pivot = new Vector2(1, 0);
        loadingText.rectTransform.anchorMax = new Vector2(1, 0);
        loadingText.rectTransform.anchorMin = new Vector2(1, 0);
        loadingText.rectTransform.sizeDelta = new Vector2(500, 200);
        loadingText.fontSize = 60;
        loadingText.alignment = TextAnchor.MiddleLeft;
        loadingText.color = Color.white;
        loadingText.enabled = false;

        // �J�ڐ�V�[���ł��I�u�W�F�N�g��j�����Ȃ�
        DontDestroyOnLoad(canvas.gameObject);

        // �V���O���g���I�u�W�F�N�g��ێ�
        canvasObject.AddComponent<FadeManager>();
        instance = canvasObject.GetComponent<FadeManager>();
    }

    // �t�F�[�h�t���V�[���J�ڂ��s��
    public void LoadScene(string sceneName, float interval = 1f)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = null;

        fadeCoroutine = Fade(sceneName, interval);
        StartCoroutine(fadeCoroutine);
    }
    private IEnumerator Fade(string sceneName, float interval)
    {
        float time = 0f;
        canvas.enabled = true;

        // �t�F�[�h�A�E�g
        while (time <= interval)
        {
            float fadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
            image.color = new Color(0.0f, 0f, 0f, fadeAlpha);
            time += Time.deltaTime;
            yield return null;
        }

        //�V�[���̓ǂݍ��݂��J�n
        async = SceneManager.LoadSceneAsync(sceneName);
        //���[�h���������Ă������V�[���̃A�N�e�B�u�������Ȃ�
        async.allowSceneActivation = false;

        time = 0f;
        loadingText.enabled = true;
        while (async.progress < 0.5f)
        {
            time += Time.deltaTime;
            //�e�L�X�g�\��
            loadingText.text = "Now loading"; 
            yield return null;
        }
        //���[�h������A�O�D�T�b�҂��Ă���V�[���ڍs
        yield return new WaitForSeconds(1f);
        loadingText.enabled = false;
        async.allowSceneActivation = true;

        // �t�F�[�h�C��
        time = 0f;
        while (time <= interval)
        {
            float fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
            image.color = new Color(0f, 0f, 0f, fadeAlpha);
            time += Time.deltaTime;
            yield return null;
        }

        // �`����X�V���Ȃ�
        canvas.enabled = false;
    }
}
