using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class ProgressManager
{
    private static string filePath = Application.persistentDataPath + "/MayDrive.bin";

    private static PlayerProgress _playerProgress = PlayerProgress.getProgress();

    public static PlayerProgress runtimePlayerProgress
    {
        get => _playerProgress; set
        {
            _playerProgress = value;
        }
    }

    public static void SaveProgress()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        FileStream saveStream = new FileStream(filePath, FileMode.Create);

        binaryFormatter.Serialize(saveStream, runtimePlayerProgress);

        saveStream.Close();
    }

    public static void LoadProgress()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        try
        {
            FileStream loadStream = new FileStream(filePath, FileMode.Open);

            runtimePlayerProgress = (PlayerProgress) binaryFormatter.Deserialize(loadStream);

            loadStream.Close();
        }
        catch (System.Exception)
        {
            Debug.LogWarning("Could not find progress data in path" + filePath);
        }
    }

    public static void ResetProgress(){
        runtimePlayerProgress.ResetAll();
        SaveProgress();
    }
}
