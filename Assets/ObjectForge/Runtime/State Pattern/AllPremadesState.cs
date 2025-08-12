using UnityEngine;

public class AllPremadesState : IState
{
    private LoadPremadesPresenter loadPremadesPresenter;
    public AllPremadesState(LoadPremadesPresenter loadPremadesPresenter)
    {
        this.loadPremadesPresenter = loadPremadesPresenter;
    }

    public void Enter()
    {
        SetLoadPremadesModelData();
    }
    
    private void SetLoadPremadesModelData()
    {
        LoadPremadesModel.Instance.WorkingDirectory = "All Premades";
        LoadPremadesModel.Instance.IsFavoritesStateActive = false;
        LoadPremadesModel.Instance.IsRecycledStateActive = false;
        LoadPremadesModel.Instance.CurrentState = this;
    }

    public void Exit()
    {
        LoadPremadesModel.Instance.ExitingState = this;
    }

}
