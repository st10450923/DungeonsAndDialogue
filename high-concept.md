# High Concept Document

## *Dungeons \& Dialogue*

## Concept

A first-person dungeon crawler game where the player has no weapons or combat abilities.
The only way to progress is to talk to the guards blocking each door and convince them to
let you pass. Each guard has a hidden emotional vulnerability that the player must discover
through environmental clues and conversation. 

## The Role of LLM

The LLM plays the role of interpreting the players response and including the context of the clues the player has uncovered to reply in a logical manner. 

Every guard
interaction is driven entirely by a locally running Ollama model. The model generates
guard responses in real time based on the player's input, the guard's personality defined
in the system prompt, and context injected dynamically \[clues read, 'strikes' (times the player has offended the guard)
accumulated].

The game would not function without the LLM. There is no scripted dialogue tree — every
conversation is unique, and whether the player succeeds depends entirely on how they
engage with the model's output. While a game like this could theoretically exist without the use of an LLM, it would require reams of written dialogue with pre-written dialogue options instead of dynamically responding to whatever the player says to the guard. This also makes each interaction with the guard unique, each time the game is played there will be different responses and they might engage differently to different appeals. 

## Why a Local Model

A locally hosted model via Ollama was chosen over a cloud API for several reasons:

* **No internet dependency** — the game runs entirely offline, which is appropriate for
a dungeon atmosphere and ensures consistent performance regardless of connectivity.
* **No API cost or token limiting** — cloud APIs introduce per-token costs and limits
that would be inappropriate for a game with open-ended conversation.
* **Privacy** — player inputs are never sent to an external server or seen by anyone that the player doesn't choose to share them with.

## Model Choice

The final model used was llama3.2:3b. This was chosen because it is relatively small compared to many models in terms of its storage size (2 GB) while being significantly more coherent and following instructions much better than other small models that I tried like smollm2 (1.8 GB). It also gives much more in-context responses, whereas smollm2 would give responses that spilled outside of what was needed or that would break the immersion of the player. 

## Target Experience

The player should feel like a careful observer and a thoughtful communicator. Reading the
room carefully and choosing words deliberately should be rewarding. The LLM should make
each guard feel like an emotionally complex character rather than a puzzle with a
fixed solution.

