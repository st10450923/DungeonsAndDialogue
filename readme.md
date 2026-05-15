# Dungeons and Dialogue

An AI-powered no-combat dungeon crawler where you have to persuade your way through the dungeon by getting to know the guards personalities and convincing them to let you through. 

## Overview

A dungeon escape game where combat is replaced entirely with conversation.
Each room is guarded by an NPC driven by a locally running LLM via Ollama.
Read the environment for clues, then talk your way through.

## Quick Start

1. Install Ollama: https://ollama.com
2. Pull the model:

```bash
ollama pull llama3.2:3b
```

3. Start Ollama:

```bash
ollama serve
```

4. Run `<!-- TODO: exe name -->.exe` from the `Build` folder

> Full setup instructions: see `setup.md`

## How to Play

* **WASD** — Move
* **E** — Interact with objects and NPCs
* Read clues in each room to understand the guard's personality
* Talk to the guard and convince them to let you pass
* Three strikes and the room resets

## Project Structure

```
Assets/
├── Scenes/          
├── Scripts/
│   ├── AI/          
│   ├── UI/          
│   ├── Guard.cs     
│   ├── Clue.cs      
│   ├── Door.cs      
│   ├── PlayerController.cs
│   └── GameManager.cs
├── ScriptableObjects/
│   └── RoomData/    
└── Sprites/         
```

## Dependencies

* Unity 6000.3.10f1
* TextMeshPro
* Ollama (local, not included)
* Model: llama3.2:3b (not included, pulled via Ollama)

## Documentation

|File|Contents|
|-|-|
|`high-concept.md`|Game concept and LLM design rationale|
|`ollama-plan.md`|Model choice, data flow, prompt structure|
|`setup.md`|Full installation and run instructions|
|`refinements-changes.md`|Iteration log and scope changes|
|`prompts-used.md`|All guard prompts with examples|
|`llm-integration-report.md`|Full integration report|

## Credits

* Developed by Ember Willow Jones
* Unity 6000.3.10f1
* LLM: Ollama (https://ollama.com) — locally hosted, no data sent externally
* Brackey's Mega Asset Pack (https://assetstore.unity.com/packages/2d/free-2d-mega-pack-177430)
* 2D Sprite Outline by Hannah Fiani(https://assetstore.unity.com/packages/vfx/shaders/2d-sprite-outline-109669#publisher)
* AI assistance: Claude (Anthropic) used for code generation and debugging during development

## AI Transparency

All NPC dialogue is generated in real time by a locally running large language model.
Responses are not scripted and will vary between playthroughs. The model runs entirely
on the player's machine — no data is sent to any external server.

