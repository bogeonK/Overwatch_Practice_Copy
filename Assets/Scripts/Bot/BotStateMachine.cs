public class BotStateMachine
{
    public IBotState CurrentState { get; private set; }

    public void Initialize(IBotState startState)
    {
        CurrentState = startState;
        CurrentState.Enter();
    }

    public void ChangeState(IBotState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}