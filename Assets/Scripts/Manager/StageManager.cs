/*using System;
using ManilaFSM;

public class Stage
{
    public float progress = 0.0f;

    public delegate void ExecuteStage();
    public ExecuteStage executing;

    public Stage()
    {
        
    }
}

public enum StageCommand
{
    START,
    FINISHED,
    STOP,
    STANDBY,
}

public delegate void StageEventHandler(object sender, EventArgs e);

public class StageFSM : FSMBase<StageEventHandler, StageCommand>
{
    public event StageEventHandler _None;
    public event StageEventHandler _Loading;
    public event StageEventHandler _Initializing;
    public event StageEventHandler _Activing;
    public event StageEventHandler _InActiving;

    public class StageEventArgs : EventArgs
    {
        private float progress = 0.0f;

        public StageEventArgs() { }

        public float ProgressValue
        {
            set
            {
                progress += value;
                if (progress >= 1.0f)
                {
                    progress = 1.0f;
                }
            }
        }
    }

    public StageFSM()
    {
        transitions.Add(new StateTransition(_None, StageCommand.START), _Loading);
        transitions.Add(new StateTransition(_Loading, StageCommand.FINISHED), _Initializing);
        transitions.Add(new StateTransition(_Initializing, StageCommand.FINISHED), _Activing);
        transitions.Add(new StateTransition(_Activing, StageCommand.STOP), _InActiving);
        transitions.Add(new StateTransition(_InActiving, StageCommand.START), _Activing);
        Current = _None;
    }

    public void RemoveAllListener()
    {
        _Loading = null;
        _Initializing = null;
    }

    public void OnLoading()
    {
        if(_Loading != null)
        {
            _Loading(this, new StageEventArgs());
        }
    }

    public void OnInitializing()
    {
        if(_Initializing != null)
        {
            _Initializing(this, new StageEventArgs());   
        }
    }

    public void OnActiving()
    {
        if(_Activing != null)
        {
            _Activing(this, new StageEventArgs());
        }
    }

    public void OnInActiving()
    {
		if (_InActiving != null)
		{
			_InActiving(this, new StageEventArgs());
		}
    }

    public override StageEventHandler GetNext(StageCommand command)
	{
        return base.GetNext(command);
	}

    public override StageEventHandler MoveNext(StageCommand command)
    {
        Current = GetNext(command);
        return Current;
    }
}

public class StageManager : SingletonBase<StageManager> 
{
    //static public StageManager Singleton;

    public event EventHandler ProgressChanged;
	public class ProgressArgs : EventArgs
	{
		private float progress = 0.0f;

		public float ProgressPercentage
		{
			get
			{
				return progress * 100;
			}
			private set
			{
				progress += value;
			}
		}
	}
    private StageFSM fsm;

    void Awake()
    {
		fsm = new StageFSM();
    }

    void Start()
    {
        
    }

    public void GoToNextStage(StageCommand command)
    {
        
    }
}*/