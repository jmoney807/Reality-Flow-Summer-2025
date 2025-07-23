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
        LoadPremadesModel.Instance.isFavoritesStateActive = false;
        LoadPremadesModel.Instance.isRecycledStateActive = false;
        LoadPremadesModel.Instance.CurrentState = this;
    }

    public void Exit()
    {
        // Force the Pressable button to untoggle
        LoadPremadesModel.Instance.stateButtons[0].ForceSetToggled(false);
    }

}
