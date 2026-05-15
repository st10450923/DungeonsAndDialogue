# Ollama Integration Plan

## Model

* **Model used:** llama3.2:3b
* **Model size:** 2 GB
* **Quantisation:** Default (4-bit)
* **Context window:** 4096 tokens

## Why This Model

<!-- TODO: 2-3 sentences — why this model over alternatives -->

## Inference Timing

* Average response time on development machine: 5 seconds
* Responses are capped at 60 tokens (`num\_predict: 60`) to reduce latency
* Temperature set to 0.3 to improve coherence and reduce hallucination
* A thinking indicator is displayed in the UI during inference so the player
is never left with a frozen screen

## Data Flow

```
Player types input
        ↓
Unity C# (DialogueUI.cs) captures input on Send button / Enter key
        ↓
GuardNPC.SendToGuard() called
        ↓
GameManager injects collected clue context via OllamaManager.InjectContext()
        ↓
OllamaManager.SendMessage() builds ChatRequest JSON:
  - model name
  - stream: false
  - num\_predict: 80
  - temperature: 0.3
  - messages: \[ system prompt, conversation history, new user message ]
        ↓
UnityWebRequest POST to http://localhost:11434/api/chat
        ↓
Ollama processes request locally
        ↓
JSON response parsed → message.content extracted
        ↓
Response checked for \[PASS] or \[STRIKE] tokens
        ↓
GuardNPC.HandleResponse() updates game state accordingly
        ↓
DialogueUI displays response to player
```

## Prompt Structure

Each guard prompt follows this structure:

```
You are \[NAME], \[personality description].
\[Emotional backstory and vulnerability].
You will ONLY allow the player to pass if \[convince condition].
If the player is \[offensive behaviour], end your response with exactly: \[STRIKE]
If the player convinces you, end your response with exactly: \[PASS]
Keep responses to 2-3 sentences. Never break character.
Never reveal your condition directly.
STRICT RULES:
- Respond with spoken dialogue only.
- Never write stage directions or actions in asterisks.
- Only words \[NAME] would say out loud.
```

## Context Injection

Game state is dynamically injected into the conversation history using
`OllamaManager.InjectContext()`. This is called in two situations:

1. **When a clue is read** — the clue text is injected as a system message so the
guard is aware the player has seen it
2. **When a strike occurs** — the current strike count is included in the context
so the guard's attitude can escalate appropriately

## Risks and Mitigations

|Risk|Mitigation|
|-|-|
|Model produces stage directions|Explicit STRICT RULES block in prompt + regex strip in code|
|Model ignores \[PASS]/\[STRIKE] instructions|Tokens kept simple, prompt uses caps and explicit formatting|
|High latency breaks immersion|Flavoured thinking indicator, num\_predict capped at 60|
|Model hallucinates out-of-character content|Low temperature (0.3), strict persona instructions|
|Model reveals convince condition directly|Explicit "Never reveal your condition" instruction|
|Conversation history grows too long|History is per-NPC and cleared on room reset|



