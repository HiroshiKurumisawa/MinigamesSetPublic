using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// �Z�[�u�f�[�^�Ǘ��N���X
// �e�V�[����Prefab����C���X�^���X�����Ďg�p
public class SaveManager : MonoBehaviour
{
    // �Z�[�u�t�@�C����
    const string FileName = "/savedata.dat";
    // �Z�[�u�f�[�^�̃f�t�H���g�l
    const float DefaultVolumeMaster = 1.0f;
    const float DefaultVolumeBgm = 1.0f;
    const float DefaultVolumeSe = 1.0f;

    FileStream file;
    BinaryFormatter bf;
    string filePath;

    private void Awake()
    {
        // �t�@�C���p�X�Z�b�g
        // Application.dataPath �� Applicatin.persistentDataPath �ɂ����
        // ���[�U�[��AppData/LocalLow���̃t�H���_�ɃZ�[�u�f�[�^�������
        filePath = Application.dataPath + FileName;
    }

    // �t�@�C���X�V���ʏ���
    void InitFileSave()
    {
        bf = new BinaryFormatter();
        file = File.Create(filePath);
    }

    // �t�@�C�����[�h���ʏ���
    void InitFileLoad()
    {
        bf = new BinaryFormatter();
        file = File.Open(filePath, FileMode.Open);
    }

    // �t�@�C���N���[�Y����
    void CloseFile()
    {
        file.Close();
        file = null;
    }

    // �t�@�C�����݃`�F�b�N
    public bool SaveDataCheck()
    {
        // �t�@�C���������true
        if (File.Exists(filePath)) { return true; }
        return false;
    }

    // �V�K�f�[�^����
    public void CreatesaveData()
    {
        try
        {
            InitFileSave();

            // �Z�[�u�f�[�^�𐶐�
            SaveData data = new SaveData();
            data.volMaster = DefaultVolumeMaster;
            data.volBgm = DefaultVolumeBgm;
            data.volSe = DefaultVolumeSe;
            bf.Serialize(file, data);
        }
        catch (IOException)
        {
            Debug.LogError("failed to open file");
        }
        finally
        {
            // FileStream���g�p������K���Ō��Close���邱��
            if (file != null) { CloseFile(); }
        }
    }

    // �f�[�^�Z�[�u
    public void Save(float vm, float vb, float vs)
    {
        try
        {
            InitFileSave();

            // �Z�[�u�f�[�^���Z�[�u
            SaveData data = new SaveData();
            data.volMaster = vm;
            data.volBgm = vb;
            data.volSe = vs;
            bf.Serialize(file, data);
        }
        catch (IOException)
        {
            Debug.LogError("failed to open file");
        }
        finally
        {
            // FileStream���g�p������K���Ō��Close���邱��
            if (file != null) { CloseFile(); }
        }
    }

    // �f�[�^���[�h
    public void LoadData(ref float vm, ref float vb, ref float vs)
    {
        try
        {
            InitFileLoad();

            // �Z�[�u�f�[�^��ǂݍ���
            SaveData data = bf.Deserialize(file) as SaveData;
            vm = data.volMaster;
            vb = data.volBgm;
            vs = data.volSe;
        }
        catch (IOException)
        {
            Debug.LogError("failed to open file");
        }
        finally
        {
            // FileStream���g�p������K���Ō��Close���邱��
            if (file != null) { CloseFile(); }
        }
    }
}
