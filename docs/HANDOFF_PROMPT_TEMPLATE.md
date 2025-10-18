# Beat-Blaster2 Codex CLI Onboarding Prompt (Template)

Paste the following into Codex CLI at the start of a session to bring the assistant up to speed:

```
You are joining the Beat-Blaster2 project (Phaser + TypeScript). Bring yourself up to speed by following these steps:

## Repository Overview
- Check `docs/dev-journal.md` (read the most recent entries) for current progress and open tasks.
- Review the applicable design files in `docs/design/` (e.g., phase plans) to understand current sprint objectives.
- Be aware that gameplay, UI, and systems code live under `src/`; HUD components under `src/ui/`; scenes under `src/scenes/`; services under `src/systems/`.

## Working Practices
- Always log significant progress in `docs/dev-journal.md` with timestamp, role, tasks, notes, next steps.
- Update the relevant `LLM_AGENT_*` playbook when you modify responsibilities/next steps for that role.
- Before starting new work, run `git status` to ensure a clean working tree; ensure branch matches the current sprint plan.
- If adding new features, update or create appropriate design docs in `docs/design/`.
- Build with `npm run build` (or `pnpm run build`) and run relevant tests whenever applicable.
- Respect `.gitignore` (DS_Store already blocked); do not commit generated or binary assets unless necessary.
- For PRs, ensure all changes are committed, pushed to a feature branch, and documented.

## Phase/Bullet Discussions (update per sprint)
- Current phase plan: (fill in — e.g., Phase B – Player Feel & HUD Feedback, see `docs/design/phaseB_player_feel.md`).
- Key tasks in flight: (list top priorities).
- Notable open issues or TODOs: (list).

## Development Checklist
1. Review latest dev-journal entry and update it as you work.
2. Cross-check responsibilities in `LLM_AGENT_*` files; adjust role-specific next steps.
3. Implement code changes following current sprint plan.
4. Keep tests/build passing (`npm run build`, tests if available).
5. Update documentation/design and journal entries.
6. Commit changes with meaningful messages, push your branch, and create/refresh PR.

## Additional Notes
- AbilityService currently feeds HUD ability overlay; replace mock data when gameplay abilities are available.
- LatencyCalibrationScene exists with basic visuals; polish and integrate save feedback when time allows.
- ProfileService & SaveService manage persistence; ensure any profile/state changes go through them.
```

Fill in the phase-specific bullet points (current sprint, tasks, TODOs) before sharing the prompt in each new session.
