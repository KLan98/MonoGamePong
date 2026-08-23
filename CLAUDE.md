# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

A Pong clone built on MonoGame (DesktopGL, .NET 8), with an ImGui-based in-game debug console. The engine-agnostic bits (`Core`, easing/lerp helpers, scaling) live in the `LanMonoGameLibrary` namespace inside `Pong/Source/Core.cs` and are meant to be reusable across future MonoGame projects; the `Pong` namespace holds the game-specific code.

## Build / run

Solution file is `Pong.slnx` (the newer XML-based `.slnx` format, not `.sln`) and contains two projects: `Pong/Pong.csproj` (the game) and the vendored `MonoGame.ImGuiNet-main/MonoGame.ImGuiNet/Monogame.ImGuiNet.csproj` (ImGui renderer for MonoGame).

```
dotnet build Pong.slnx
dotnet run --project Pong/Pong.csproj
```

There is no test project/framework in this repo.

**Content pipeline**: sprites live in a single spritesheet (`Pong/Content/PongAssets/`) built via MGCB (`Pong/Content/Content.mgcb`). The `dotnet-mgcb` tool is restored automatically on build through the `RestoreDotnetTools` MSBuild target in `Pong.csproj` (`dotnet tool restore`, config in `Pong/.config/dotnet-tools.json`) — this can take a while on a first build.

**Vendored dependency caveat**: `MonoGame.ImGuiNet-main/` is listed in `.gitignore` and is NOT tracked in this repo — it must be present on disk (pulled in manually from its upstream source) for the solution to build. `Pong.csproj` also references it via a hardcoded absolute path (`D:\Game\Pong\MonoGamePong\MonoGame.ImGuiNet-main\...`) in addition to the relative path — if that folder or drive layout is missing/different, restore/build will fail until the absolute `ProjectReference` is fixed or removed.

## Architecture

**Singletons wired up in `MyGame.Initialize()`**: `AssetsManager`, `PhysicsManager`, `InputManager`, and `Core` itself are all effectively singletons (`GetInstance()` + a static `instance` field set in their constructors). Order matters — e.g. `PhysicsManager`'s constructor reads sprite sizes from `AssetsManager`, so `AssetsManager` must be constructed first; `DebugConsole` reads `PhysicsManager.GetInstance()` in its own constructor, so `PhysicsManager` must exist first too.

**Game loop** (`MyGame : Core`, overriding MonoGame's `Update`/`Draw`):
- `Update`: `InputManager.UpdateInput()` (keyboard → `PhysicsManager.SetDirection`) then `PhysicsManager.UpdatePhysics(gameTime)` (single pass that moves every entity and resolves collisions for that frame).
- `Draw`: sprites are drawn through a single `SpriteBatch.Begin(..., transformMatrix: SpriteScaleMatrix)` call, then the ImGui debug console is drawn on top via `debugConsole.ImGuiRenderer`.

**Virtual resolution scaling**: gameplay/physics/UI positions are always expressed in a fixed *virtual* resolution (1280x720, `Core.virtualWidth`/`virtualHeight`, exposed via `GetVirtualResolution()`), independent of the actual window/back-buffer size (`GetScreenResolution()`/`SetScreenResolution()`). `Core.UpdateScaleMatrix()` computes `SpriteScaleMatrix` (virtual → screen) whenever the back buffer changes; this matrix is passed to `SpriteBatch.Begin` so all drawing automatically scales. When touching physics/collision bounds, use `GetVirtualResolution()`, not the back buffer size.

**Entities and physics** (`PhysicsManager`): three "moving" entities share one set of parallel arrays, indexed by the `MovingEntities` enum (`Player = 0, Com = 1, Ball = 2`) — `movingPhysics[]` (`EntityPhysics`: `Position`/`Direction`/`Velocity`/`Speed`), and `movingSprites[]`/`initPos[]` etc. from `AssetsManager` must stay in sync with this same ordering. Three "static" (non-moving) entities (board, player score bar, com score bar) are indexed by the separate `StaticEntities` enum and use `staticPositions[]`/`sprites[]`. `EntityPhysics.Direction` is expected to behave like a unit vector (it's multiplied directly against speed to get displacement) but nothing in the codebase actually normalizes it — collision response code that derives a new `Direction` (e.g. `Vector2.Reflect`) should keep this in mind.

Per-frame collision in `UpdatePhysics` works by building a `predictedRect` from each entity's *next* position before committing the move, then checking that against the four screen-edge `Rectangle` colliders (`topCollider`/`botCollider`/`leftCollider`/`rightCollider`, cached once in the `PhysicsManager` constructor from the virtual resolution) and, for the ball, the paddles' current-frame colliders. Paddles (`i < 2`) get clamped at the top/bottom edges; the ball (`i == 2`) reflects off top/bottom/paddles and only `Debug.WriteLine`s "Player/Enemy scored" at the left/right edges — scoring (incrementing a score, resetting the ball) is not implemented yet, so the ball currently just keeps travelling past goal lines.

**Input**: `InputManager` only drives the player paddle (index 0) via Up/Down; there's no AI/second-player input wired up for the `Com` paddle yet, and no gamepad support. Ctrl+D toggles the ImGui debug console (`InputManager.ToolActive`); Escape exits.

**Rendering**: `Sprite` wraps a `TextureRegion` (a sub-rectangle of the single shared spritesheet loaded in `AssetsManager.InitAssets()`) plus per-instance transform (`Scale`, `Rotation`, `Origin`, `LayerDepth`, `SpriteEffects`). All texture regions are cut from one spritesheet (`Content/PongAssets/spritesheet`) with hardcoded pixel rects in `AssetsManager.InitAssets()` — adding/moving art requires updating those rects to match the spritesheet layout.

**Debug console** (`DebugConsole`, ImGui): shows live per-entity physics state and lets you switch resolution presets, toggle fullscreen, and reset the ball position. Note `PhysicsManager.GetColliderInfo(index)` (used here to display "Collider information") currently always returns `Rectangle(0,0,0,0)` — the code path that populates the backing `rectColliders` array is commented out in `UpdatePhysics`.

**Stub/unused files**: `Player.cs`, `SquareCollider.cs`, `EntityRender.cs`, and `RenderManager.cs` (class is actually named `RenderManagert`, note the typo) are empty placeholders not currently wired into the game loop — don't assume they do anything.

## Git workflow

Branch naming convention seen in history: `bug-<short-description>` and `feature-<short-description>`, merged into `main` via PR.
