# Refinements \& Changes Log

## Model Selection

* **Initial model:** smollm2 (1.8GB)

  * Response quality was poor — incoherent, ignored prompt instructions, broke character frequently
* **Switched to:** llama3.2:3b (2.0GB)

  * Significantly improved coherence and instruction following
  * Better at maintaining character persona across conversation turns

## Performance Optimisation

* Added `num\_predict: 60` to cap response token length

  * Reduced average response time from 16s to 5s
* Added `temperature: 0.3` to reduce hallucination and improve on-topic responses

  * Lower temperature made guard personalities more consistent across playtests

## Prompt Engineering Iterations

### Stage Direction Problem

* **Issue:** Model kept producing stage directions in asterisks e.g. `\*looks over shoulder\*`
* **Fix:** Added explicit STRICT RULES block to all prompts:

```
  STRICT RULES:
  - Respond with spoken dialogue only.
  - Never write stage directions or actions in asterisks.
  - Only words \[NAME] would say out loud.
  ```

* **Additional fix:** Added regex strip in `GuardNPC.CleanResponse()` to remove any
remaining asterisk content as a code-level safety net

### Token Reliability

* **Issue:** Model occasionally produced \[PASS] or \[STRIKE] mid-response rather than at the end
* **Fix:** Added `end your response with exactly:` wording to prompt instructions
* **Additional fix:** Used `response.Contains()` rather than endsWith check so tokens
are caught regardless of position

### Convince Condition Leaking

* **Issue:** Guard occasionally hinted too directly at their convince condition
* **Fix:** Added `Never reveal your condition directly` instruction to all prompts

## Technical Fixes

* **ContactFilter2D:** Replaced `Physics2D.OverlapCircle` with `ContactFilter2D` version
to fix trigger detection not working outside of runtime layer overrides
* **Coroutine host:** Moved ThinkingIndicator coroutine to run on DialogueUI host rather
than ThinkingIndicator itself to fix inactive GameObject coroutine error
* **Input System:** Removed legacy `Input.GetKeyDown` usage, replaced with
`Keyboard.current.eKey.wasPressedThisFrame` for compatibility with new Input System
* **Dialogue reopen bug:** Added `\&\& !isDialogueOpen` guard to E key check to prevent
dialogue restarting while player is typing

## Scope Changes

* Removed procedural room generation — static hand-built rooms used instead (time constraint)
* Removed save system — not required for a short dungeon experience
* Removed music and SFX
* Removed AI voice acting (was originally a stretch goal)

## Playtesting Notes

<!-- TODO: Fill in after playtesting your rooms -->

* Room 1 (\[Guard name]): \[Notes on difficulty, prompt tweaks made]
* Room 2 (\[Guard name]): \[Notes on difficulty, prompt tweaks made]
* Room 3 (\[Guard name]): \[Notes on difficulty, prompt tweaks made]

