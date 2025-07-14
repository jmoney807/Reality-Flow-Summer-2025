using UnityEngine;

public class NoState : IState
{
    private LoadPremadesPresenter loadPremadesPresenter;
    public NoState(LoadPremadesPresenter loadPremadesPresenter)
    {
        this.loadPremadesPresenter = loadPremadesPresenter;
    }

    public void Enter()
    {
        LoadPremadesModel.Instance.CurrentState = this;
    }

    public void Exit()
    {
        // Logic for exiting the NoState
    }
}

    
