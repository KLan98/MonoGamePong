using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using MonoGame.ImGuiNet;
using System.Diagnostics;
using LanMonoGameLibrary;

public class DebugConsole
{
    //-----------------------------------PROPERTIES------------------------------------
    public ImGuiRenderer ImGuiRenderer { get; private set; }

    //--------------------------------------FIELDS--------------------------------------
    private string[] resolutionPresets = new string[] { "1920x1080", "1280x720", "800x600" };
    private Vector2[] resolutions = new Vector2[] { new Vector2 { X = 1920, Y = 1080 }, new Vector2 { X = 1280, Y = 720 }, new Vector2 { X = 800, Y = 600 } };
    private int currentResolution = 1;
    private bool fullScreen = false;
    private GraphicsDeviceManager graphicsDeviceManager;

    public DebugConsole(GraphicsDeviceManager graphicsDeviceManager)
    {
        this.graphicsDeviceManager = graphicsDeviceManager;
    }

    public void Initialize(Game game)
    {
        ImGuiRenderer = new ImGuiRenderer(game);
        ImGuiRenderer.RebuildFontAtlas();
    }

    public void UpdateDraw(bool toolActive)
    {
        if (toolActive)
        {

            ImGui.Begin("Debug Console", ref toolActive, ImGuiWindowFlags.MenuBar);
            if (ImGui.BeginMenuBar())
            {
                ImGui.Text($"FPS {ImGui.GetIO().Framerate}");
                if (ImGui.BeginMenu("UI Settings"))
                {
                    if (ImGui.BeginCombo("Resolution", resolutionPresets[currentResolution]))
                    {
                        // go through every presets
                        for (int i = 0; i < resolutionPresets.Length; i++)
                        {
                            // check if preset is selected
                            if (ImGui.Selectable(resolutionPresets[i], currentResolution == i))
                            {
                                currentResolution = i; // selection changed, only runs if a res is chosen
                                graphicsDeviceManager.PreferredBackBufferWidth = (int)resolutions[i].X;
                                graphicsDeviceManager.PreferredBackBufferHeight = (int)resolutions[i].Y;
                                graphicsDeviceManager.ApplyChanges();
                                Core.GetInstance().UpdateScaleMatrix();
                                Debug.WriteLine($"Resolution {resolutions[i].X}x{resolutions[i].Y} selected");
                            }
                        }
                        ImGui.EndCombo();
                    }

                    if (ImGui.Checkbox("FullScreen", ref fullScreen))
                    {
                        graphicsDeviceManager.IsFullScreen = fullScreen;
                        graphicsDeviceManager.ApplyChanges();
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Gameplay Settings"))
                {
                    if (ImGui.TreeNodeEx("Player"))
                    {
                        ImGui.Text("Content for Child regions");
                        ImGui.TreePop();
                    }

                    if (ImGui.TreeNodeEx("Com"))
                    {
                        ImGui.Text("Content for Child regions");
                        ImGui.TreePop();
                    }
                    
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }
            ImGui.End();
        }
    }
}