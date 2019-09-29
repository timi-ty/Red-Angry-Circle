[System.Serializable]
public class PlayerProgress
{
    private int _playerLevel;
    private int[] _objCompletion = new int[3];
    private int _highScore;

    private static PlayerProgress currentProgress = new PlayerProgress();

    public int playerLevel { get => _playerLevel; set => _playerLevel = value; }
    public int[] objectiveCompletion { get => _objCompletion; set => _objCompletion = value; }
    public int highScore { get => _highScore; set => _highScore = value; }

    //Player progress is a Singleton to ensure only one instance exists per session
    private PlayerProgress(){}

    public void ResetAll(){
        playerLevel = 0;
        for(int i = 0; i < objectiveCompletion.Length; i++){
            objectiveCompletion[i] = 0;
        }
    }

    public void ResetObjectiveCompletion()
    {
        for(int i = 0; i < objectiveCompletion.Length; i++){
            objectiveCompletion[i] = 0;
        }
    }

    ///<summary>
    ///This method returns only instance of PlayerProgress. 
    ///Consider using ProgressManager to manage this instance
    ///</sumarry>
    public static PlayerProgress getProgress() => currentProgress;
}
