# Setup Guide

## System Requirements

|Component|Minimum|
|-|-|
|OS|Windows 10 / macOS 12 / Ubuntu 20.04|
|RAM|8GB (16GB recommended)|
|Storage|<!-- TODO: Add final build size + model size -->|
|GPU|Not required — CPU inference supported|
|Unity Version|6000.3.10f1|

## 

## Step 1 — Install Ollama

1. Download Ollama from https://ollama.com
2. Run the installer
3. Verify installation
4. 

## Step 2 — Pull the Model

Open a terminal and run:

```bash
ollama pull llama3.2:3b
```

Wait for the download to complete (\~2GB). 

Verify with:

```bash
ollama list
```

You should see `llama3.2:3b` in the list.



## Step 3 — Start Ollama

Ollama must be running before launching the game. In a terminal run:

```bash
ollama serve
```

Leave this terminal open while playing.

## Step 4 — Run the Game

### Option A — Play the Build

1. Navigate to the `Build` folder
2. Run `<!-- TODO: Add your exe name -->.exe`

### Option B — Run in Unity Editor

1. Open Unity Hub
2. Click `Add` and select the project folder
3. Open the project in Unity `6000.3.10f1`
4. Open the scene at `Assets/Scenes/Room1`
5. Press Play

## Dependencies

* Unity `6000.3.10f1`
* TextMeshPro (included in project)
* Ollama
* Model: `llama3.2:3b`
* No other external packages required

## Troubleshooting

**Game shows no NPC response / stuck on "Thinking..."**

* Make sure `ollama serve` is running in a terminal
* Make sure the correct model is pulled (`ollama list`)
* Check the Unity Console for connection errors

**Very slow responses**

* This is normal on CPU-only machines
* Responses typically take \[5–10] seconds on an average machine

**NPC responses feel incoherent**

* This is a known limitation of smaller local models
* Prompt engineering has been applied to mitigate this — see `ollama-plan.md`

## Credits

* Game developed by Ember Willow Jones
* Built with Unity 6000.3.10f1
* LLM integration via Ollama 
* Brackey's Mega Asset Pack (https://assetstore.unity.com/packages/2d/free-2d-mega-pack-177430) 
* 2D Sprite Outline by Hannah Fiani(https://assetstore.unity.com/packages/vfx/shaders/2d-sprite-outline-109669#publisher)
* AI tools used in development: Claude (Anthropic) for code assistance

