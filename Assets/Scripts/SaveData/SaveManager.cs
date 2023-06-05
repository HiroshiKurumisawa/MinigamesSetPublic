using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// セーブデータ管理クラス
// 各シーンでPrefabからインスタンス化して使用
public class SaveManager : MonoBehaviour
{
    // セーブファイル名
    const string FileName = "/savedata.dat";
    // セーブデータのデフォルト値
    const float DefaultVolumeMaster = 1.0f;
    const float DefaultVolumeBgm = 1.0f;
    const float DefaultVolumeSe = 1.0f;

    FileStream file;
    BinaryFormatter bf;
    string filePath;

    private void Awake()
    {
        // ファイルパスセット
        // Application.dataPath を Applicatin.persistentDataPath にすると
        // ユーザーのAppData/LocalLow内のフォルダにセーブデータが作られる
        filePath = Application.dataPath + FileName;
    }

    // ファイル更新共通準備
    void InitFileSave()
    {
        bf = new BinaryFormatter();
        file = File.Create(filePath);
    }

    // ファイルロード共通準備
    void InitFileLoad()
    {
        bf = new BinaryFormatter();
        file = File.Open(filePath, FileMode.Open);
    }

    // ファイルクローズ処理
    void CloseFile()
    {
        file.Close();
        file = null;
    }

    // ファイル存在チェック
    public bool SaveDataCheck()
    {
        // ファイルがあればtrue
        if (File.Exists(filePath)) { return true; }
        return false;
    }

    // 新規データ生成
    public void CreatesaveData()
    {
        try
        {
            InitFileSave();

            // セーブデータを生成
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
            // FileStreamを使用したら必ず最後にCloseすること
            if (file != null) { CloseFile(); }
        }
    }

    // データセーブ
    public void Save(float vm, float vb, float vs)
    {
        try
        {
            InitFileSave();

            // セーブデータをセーブ
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
            // FileStreamを使用したら必ず最後にCloseすること
            if (file != null) { CloseFile(); }
        }
    }

    // データロード
    public void LoadData(ref float vm, ref float vb, ref float vs)
    {
        try
        {
            InitFileLoad();

            // セーブデータを読み込み
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
            // FileStreamを使用したら必ず最後にCloseすること
            if (file != null) { CloseFile(); }
        }
    }
}
