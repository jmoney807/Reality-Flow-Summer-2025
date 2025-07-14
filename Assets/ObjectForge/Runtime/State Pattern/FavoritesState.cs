using UnityEngine;

public class FavoritesState : IState
{
    private LoadPremadesPresenter loadPremadesPresenter;
    public FavoritesState(LoadPremadesPresenter loadPremadesPresenter)
    {
        // You can use the loadPremadesPresenter if needed
        // For example, you might want to initialize some data or UI elements here
        this.loadPremadesPresenter = loadPremadesPresenter;
    }

    public void Enter()
    {
        // Set the model's current working directory to All Premades/Favorites
        LoadPremadesModel.Instance.WorkingDirectory = "All Premades/Favorites";

        // Set the boolean to track if the Favorites state is active
        LoadPremadesModel.Instance.isFavoritesStateActive = true;
        LoadPremadesModel.Instance.isRecycledStateActive = false;

        LoadPremadesModel.Instance.CurrentState = this;
    }

    public void Exit()
    {
        // Force the Pressable button to untoggle
        LoadPremadesModel.Instance.stateButtons[1].ForceSetToggled(false);
    }
}

