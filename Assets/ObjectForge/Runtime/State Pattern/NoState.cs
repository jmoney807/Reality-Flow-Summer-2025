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
        // Make working directory empty or set to a default state
        LoadPremadesModel.Instance.WorkingDirectory = string.Empty;
        LoadPremadesModel.Instance.IsFavoritesStateActive = false;
        LoadPremadesModel.Instance.IsRecycledStateActive = false;
        LoadPremadesModel.Instance.CurrentState = this;
    }

    public void Exit()
    {
        LoadPremadesModel.Instance.ExitingState = this;
        Debug.Log("Exiting NoState");        
    }
}

    
