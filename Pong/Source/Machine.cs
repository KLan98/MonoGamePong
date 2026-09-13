using System;
using System.Data;

public struct Machine
{
    public float StateTime; // time since the state started
    public float StateBeginCountdown; // time countdown for starting the state
    public float StateEndCountdown; // time countdown for ending the state

    public Machine(float stateTime, float stateBeginCountdown, float stateEndCountdown)
    {
        StateTime = stateTime;
        StateBeginCountdown = stateBeginCountdown;
        StateEndCountdown = stateEndCountdown;
    }
}
