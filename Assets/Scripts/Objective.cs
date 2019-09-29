public class Objective
{
    struct ObjectiveInfo
    {
        public string description;
        public Types type;
        public int goal;
        public bool isVolatile;
    }
    static ObjectiveInfo[,] Objectives = new ObjectiveInfo[2,3];

    public enum Types{AllKills, SwordKills, GunKills, ShieldBlocks, DarkRushKills, BottomHalfKills, 
    CrossOvers, Escapes, SlowMoCount, ComboCount}

    
    private ObjectiveManager objectiveManager;
    private int _index;
    private int _goal;
    private bool _isCompleted;
    private bool _isVolatile;
    private string _description;
    private Types _type;
    public int completion
    {
        get => ProgressManager.runtimePlayerProgress.objectiveCompletion[_index];
        set
        {
            ProgressManager.runtimePlayerProgress.objectiveCompletion[_index] = value;
            
            if(ProgressManager.runtimePlayerProgress.objectiveCompletion[_index] >= _goal){
                ProgressManager.runtimePlayerProgress.objectiveCompletion[_index] = _goal;
                if(!isCompleted){
                    isCompleted = true;
                    objectiveManager.ObjectiveComplete(_index);
                }
            }
            objectiveManager.Refresh(_index);
        }
    }
    public string description
    {
        get
        {
            string _perComp = " - " + ((int)(100 * (float)completion/(float)_goal)).ToString() + "%";
            return _description + _perComp;
        }

        private set => _description = value;
    }
    public bool isCompleted { get => _isCompleted; private set => _isCompleted = value; }
    public bool isVolatile { get => _isVolatile; private set => _isVolatile = value; }
    public Types type { get => _type; set => _type = value; }

    public Objective(int level, int index, ObjectiveManager objMan)
    {
        DescribeObjectives();

        objectiveManager = objMan;

        _index = index;

        description = Objectives[level, index].description;
        type = Objectives[level, index].type;
        _goal = Objectives[level, index].goal;
        isVolatile = Objectives[level, index].isVolatile;
    }

    //Extract content into a scriptable object
    private static void DescribeObjectives()
    {
        Objectives[0, 0].description = "Kill fifty enemies in one run";
        Objectives[0, 0].goal = 50;
        Objectives[0, 0].type = Types.AllKills;
        Objectives[0, 0].isVolatile = true;

        Objectives[0, 1].description = "Kill ten enemies with the double sword";
        Objectives[0, 1].goal = 10;
        Objectives[0, 1].type = Types.SwordKills;
        Objectives[0, 1].isVolatile = false;

        Objectives[0, 2].description = "Kill ten enemies with the machine gun";
        Objectives[0, 2].goal = 10;
        Objectives[0, 2].type = Types.GunKills;
        Objectives[0, 2].isVolatile = false;

        Objectives[1, 0].description = "Block five times with shield in one run";
        Objectives[1, 0].goal = 5;
        Objectives[1, 0].type = Types.ShieldBlocks;
        Objectives[1, 0].isVolatile = true;

        Objectives[1, 1].description = "Kill twenty enemies in the bottom half in one run";
        Objectives[1, 1].goal = 20;
        Objectives[1, 1].type = Types.BottomHalfKills;
        Objectives[1, 1].isVolatile = true;

        Objectives[1, 2].description = "Allow twenty enemies to escape in one run";
        Objectives[1, 2].goal = 20;
        Objectives[1, 2].type = Types.Escapes;
        Objectives[1, 2].isVolatile = true;
    }
}
