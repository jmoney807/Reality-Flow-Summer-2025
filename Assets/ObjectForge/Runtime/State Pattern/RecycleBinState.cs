using UnityEngine;

public class RecycleBinState : IState
{
    private LoadPremadesPresenter loadPremadesPresenter;
    public RecycleBinState(LoadPremadesPresenter loadPremadesPresenter)
    {
        this.loadPremadesPresenter = loadPremadesPresenter;
    }

    public void Enter()
    {
        // Set the model's current working directory to Recycle Bin
        LoadPremadesModel.Instance.WorkingDirectory = "Recycle Bin";

        // Set the boolean to track if the Favorites state is active
        LoadPremadesModel.Instance.IsFavoritesStateActive = false;
        // Set the boolean to track if the Recycle Bin state is active
        LoadPremadesModel.Instance.IsRecycledStateActive = true;

        LoadPremadesModel.Instance.CurrentState = this;
    }

    public void Exit()
    {
        LoadPremadesModel.Instance.ExitingState = this;
    }
}
