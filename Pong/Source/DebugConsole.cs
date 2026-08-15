using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGuiNet;
using System.Diagnostics;
using LanMonoGameLibrary;
using System;

public class DebugConsole
{
    //-----------------------------------PROPERTIES------------------------------------
    public ImGuiRenderer ImGuiRenderer { get; private set; }

    //--------------------------------------FIELDS--------------------------------------
    private string[] resolutionPresets = new string[] { "1920x1080", "1280x720", "800x600" };
    private Vector2[] resolutions = new Vector2[] { new Vector2 { X = 1920, Y = 1080 }, new Vector2 { X = 1280, Y = 720 }, new Vector2 { X = 800, Y = 600 } };
    private int currentResolution = 1; // default is 720p
    private bool fullScreen = false;
    private GraphicsDeviceManager graphicsDeviceManager;
    private PhysicsManager physicsManager;
    private PhysicsManager.MovingEntities[] movingEntities;
    private PhysicsManager.StaticEntities[] staticEntities;
    private ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.DefaultOpen;
    private System.Numerics.Vector2 debugConsoleSize;
    private Core core;

    public DebugConsole(GraphicsDeviceManager graphicsDeviceManager)
    {
        this.graphicsDeviceManager = graphicsDeviceManager;
        physicsManager = PhysicsManager.GetInstance();
        movingEntities = Enum.GetValues<PhysicsManager.MovingEntities>();
        staticEntities = Enum.GetValues<PhysicsManager.StaticEntities>();
        core = Core.GetInstance();
    }

    public void Initialize(Game game)
    {
        ImGuiRenderer = new ImGuiRenderer(game);
        ImGuiRenderer.RebuildFontAtlas();
    }

    public void UpdateDraw(bool toolActive)
    {
        // width and height of game window
        int screenWidth = graphicsDeviceManager.PreferredBackBufferWidth;
        int screenHeight = graphicsDeviceManager.PreferredBackBufferHeight;

        // resize the debug window
        debugConsoleSize = new System.Numerics.Vector2(screenWidth / 1.5f, screenHeight * 2 / 3);

        System.Numerics.Vector2 childSize = new System.Numerics.Vector2(debugConsoleSize.X - 20.0f, debugConsoleSize.Y) / 2;

        System.Numerics.Vector2 buttonSize = new System.Numerics.Vector2(childSize.X, debugConsoleSize.Y / 10);

        if (toolActive)
        {
            ImGui.SetNextWindowSize(debugConsoleSize); // set the next window size (debug console in this case)

            ImGui.Begin("Debug Console", ref toolActive, ImGuiWindowFlags.MenuBar); // begin the window "debug console"

            ImGui.BeginChild("Entities debug info", childSize, ImGuiChildFlags.FrameStyle);
            if (ImGui.CollapsingHeader("Moving entities", flags))
            {
                for (int i = 0; i < movingEntities.Length; i++)
                {
                    ImGui.TextWrapped($"---------------Entity name {movingEntities[i]}--------------");
                    ImGui.TextWrapped($"Speed {physicsManager.GetEntityPhysics(i).Speed}");
                    ImGui.TextWrapped($"Velocity {physicsManager.GetEntityPhysics(i).Velocity}");
                    ImGui.TextWrapped($"Current position {physicsManager.GetEntityPhysics(i).Position}");
                    ImGui.TextWrapped($"Current direction {physicsManager.GetEntityPhysics(i).Direction}");
                    ImGui.TextWrapped($"Collider information {physicsManager.GetColliderInfo(i)}");
                }
            }

            if (ImGui.CollapsingHeader("Static entities"))
            {
                // debug information
                for (int i = 0; i < staticEntities.Length; i++)
                {
                    ImGui.TextWrapped($"---------------Entity name {staticEntities[i].ToString()}--------------");
                    ImGui.TextWrapped($"Current position {physicsManager.GetStaticPosition(i)}");
                }
            }

            ImGui.EndChild();

            ImGui.SameLine();

            ImGui.BeginChild("Output window", childSize, ImGuiChildFlags.Border);
            if (ImGui.CollapsingHeader("Output window"))
            {
                // debug information

            }

            ImGui.EndChild();

            // Buttons
            if (ImGui.Button("Reset ball position", buttonSize))
            {
                physicsManager.ResetPosition((int)PhysicsManager.MovingEntities.Ball);
            }

            if (ImGui.Button("Reset ball position", buttonSize))
            {

            }

            // Menu bar
            if (ImGui.BeginMenuBar())
            {
                ImGui.Text($"FPS {ImGui.GetIO().Framerate}");
                ImGui.Text($"Frame time {ImGui.GetIO().DeltaTime}");

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
                                int width = (int)resolutions[i].X;
                                int height = (int)resolutions[i].Y;

                                core.SetScreenResolution(width, height);

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
                    // Adaptable per game
                    for (int i = 0; i < movingEntities.Length; i++)
                    {
                        if (ImGui.TreeNodeEx($"{movingEntities[i].ToString()}"))
                        {
                            // implementation here
                            ImGui.Text("Content for Child regions");

                            ImGui.TreePop();
                        }
                    }

                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }
            ImGui.End();
        }

        float remainingWidth = ImGui.GetContentRegionAvail().X;
    }
}
