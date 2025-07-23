using UnityEngine;
using System;

[Serializable]
public class UIStateMachine
{
    public IState CurrentState { get; private set; }

    public AllPremadesState AllPremadesState;
    public FavoritesState FavoritesState;
    public RecycleBinState RecycleBinState;
    public NoState NoState;

    public UIStateMachine(LoadPremadesPresenter loadPremadesPresenter)
    {
        Debug.Log("UIStateMachine constructor called");

        // Initialize the AllPremadesState with the presenter if needed
        this.AllPremadesState = new AllPremadesState(loadPremadesPresenter);
        this.FavoritesState = new FavoritesState(loadPremadesPresenter);
        this.RecycleBinState = new RecycleBinState(loadPremadesPresenter);
        this.NoState = new NoState(loadPremadesPresenter);   
    }

    // For now startingState will be AllPremadesState
    public void Initialize(IState startingState)
    {
        CurrentState = startingState;

        Debug.Log($"UIStateMachine initialized with state: {CurrentState.GetType().Name}");
        
        startingState.Enter();
    }

    public void ChangeState(IState newState)
    {
        Debug.Log($"Changing state from: {CurrentState.GetType().Name} to: {newState.GetType().Name}");
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
