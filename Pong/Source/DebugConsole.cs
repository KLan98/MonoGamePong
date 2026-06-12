using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGuiNet;
using System;
using System.Diagnostics;

public class DebugConsole
{
    public void Update(bool toolActive)
    {
        if (toolActive)
        {
            ImGui.Begin("Debug Console", ref toolActive, ImGuiWindowFlags.MenuBar);
            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("UI Settings"))
                {

                    ImGui.EndMenu();
                }

                else if (ImGui.BeginMenu("Gameplay Settings"))
                {

                    ImGui.EndMenu();
                }

                ImGui.EndMenuBar();
            }
            ImGui.End();
        }
    }
}