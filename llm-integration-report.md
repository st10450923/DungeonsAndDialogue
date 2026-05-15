# LLM Integration Report

**Student:** Ember Willow Jones
**Module:** GADS7321
**Date:** 15/05/2026
**Word count:** 733

\---

## 1\. Technical Decisions

The integration uses Ollama, a locally hosted LLM inference server, communicating
with Unity via its REST API at `http://localhost:11434/api/chat`. Unity's built-in
`UnityWebRequest` sends JSON payloads directly from C# without any middleware,
keeping the integration lightweight and self-contained within the Unity project.

The model selected for the final build was  llama3.2:3b. Initial
development used smollm2, which proved too small for reliable instruction
following — it frequently broke character, ignored prompt tokens, and produced
incoherent responses. Switching to llama3.2:3b produced significantly
more consistent behaviour while remaining within a practical storage footprint
for local deployment.

Two key parameters were tuned to improve performance. `num\\\_predict` was capped at
60 tokens, reducing average response time from approximately 16 seconds
to 5 seconds while keeping responses appropriately brief for a game
context. Temperature was set to 0.3, reducing the model's tendency to hallucinate
off-topic content and improving adherence to the character persona defined in the
system prompt.

## 2\. Integration Strategy

The integration is built around three components working together: `OllamaManager`,
`GuardNPC`, and the prompt architecture in `RoomData` ScriptableObjects.

`OllamaManager` is a singleton that manages all communication with Ollama and
maintains per-NPC conversation histories. This means each guard remembers the
full conversation, allowing the player's earlier statements to influence later
responses. It also exposes an `InjectContext` method that allows game state —
specifically which clues the player has read — to be silently inserted into the
conversation history as system messages.

`GuardNPC` handles the game logic layer of the integration. It passes player
input to `OllamaManager`, then parses the response for two control tokens:
`\\\[PASS]` and `\\\[STRIKE]`. These tokens are instructed in the system prompt and
detected with a simple `Contains` check. When `\\\[PASS]` is detected the guard
delivers a farewell response and the door unlocks. When `\\\[STRIKE]` is detected
the strike counter increments and the room resets at three strikes. This token
approach keeps game logic cleanly separated from natural language output.

Each guard's personality, convince condition, and behavioural rules are defined
entirely in a `RoomData` ScriptableObject. This means adding or modifying a guard
requires no code changes — only a data asset update. The system prompt follows a
consistent structure: persona, vulnerability, pass condition, strike condition,
and a STRICT RULES block that enforces spoken-dialogue-only output.

## 3\. Performance Considerations

Local inference introduced latency that required deliberate design responses.
A `ThinkingIndicator` component displays animated dots so the player is never left
with a frozen UI. This also fits the dungeon atmosphere — a guard pausing to
consider a player's words feels natural rather than like a technical limitation.

Context window management was considered throughout development. Conversation
histories are stored per-NPC and cleared on room reset, preventing unbounded
growth. Injected context is kept brief — single sentence clue summaries — to
avoid consuming tokens that would be better used for response generation.

The local deployment model means inference speed is entirely dependent on the
player's hardware. This is acknowledged as a limitation and documented in
`setup.md` with guidance for players on expected response times.

## 4\. Gameplay Impact

The LLM integration is not supplementary to the game — it is the game. Replacing
combat with conversation means the model's output quality directly determines
the quality of the play experience. This created a strong incentive to invest
in prompt engineering rather than treating it as a secondary concern.

The clue-to-context injection pipeline produces emergent behaviour that would
be impossible with scripted dialogue. A player who reads all three clues before
speaking to the guard will have that context silently available in the model's
history, producing responses that feel more reactive and personalised without
any additional scripting.

## 5\. Ethical Considerations

All LLM responses are generated locally — no player input is transmitted to
any external server. This is noted explicitly in the game's readme.

The use of AI-generated dialogue raises questions about player awareness. A player
interacting with what feels like a responsive character should understand they are
engaging with a language model, not authored dialogue. The game addresses this
through its framing — the AI-driven nature of NPCs is not hidden — and through
the inherent unpredictability of responses, which makes the system's generative
nature apparent during play.

Claude (Anthropic) was used as a coding assistant during development for code
generation, debugging, and architectural decisions. All code was reviewed and
integrated manually. This use is documented in `readme.md`.

