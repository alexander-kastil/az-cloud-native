# CLAUDE.md

Event-Driven Apps on Azure: a 4-day hands-on class for Azure developers and software architects who design and build event-driven distributed applications on .NET 10, taught on one domain, Grey Logistics, a last-mile parcel delivery company.

Author with the global `create-class` skill, build slides with the global `create-pptx` skill, and run the `brand-voice-code` skill after writing or editing a readme or guide.

## Layout

```text
demos/readme.md                      class readme: Duration, Audience, Prerequisites, Modules, Schedule
demos/_images/                       grey-system-map.svg, the Grey Logistics system map
demos/NN-<module>/readme.md          module readme, 9 modules from 01-distributed-design to 09-ai-agents
demos/NN-<module>/NN-<topic>/        readme.md (teaching) and _images/
demos/NN-<module>/pptx/              slide specs NN-MM-<slug>.md, images/, topics.json, NN-<module>.pptx and .pdf
demos/pptx/                          portraits.json
labs/NN-<module>/                    readme.md (lab guide), grey-logistics/ starter, grey-logistics-solution/, _images/,
                                     provision.azcli and deprovision.azcli where the lab needs Azure resources
labs/solution/                       the complete Grey Logistics solution
setup/                               lab VM, Docker, WSL, Dapr CLI and tooling install
tooling/                             GitHub, VS Code, Markdown, Azure CLI, Azure Pass, REST client, Docker and WSL
tasks/                               todo.md, lessons.md
```

Module and topic numbers are two-digit, gapless, in teaching order; do not reorganize them. A topic title is identical in the `demos/readme.md` bullet, the topic H1 and the module Topics table.

## Brand Voice

Every `readme.md` and guide under `demos/` and `labs/` follows `.claude/skills/brand-voice-code/references/rules.md`. Check a file against it after writing or significantly editing it.

## Harness

Orchestration runs in the integrations-marketplace mods (`D:\git-projects\integrations-marketplace`), and the house rules in `~/.claude/CLAUDE.md` apply as they stand; this file repeats neither.

- `subagent-delegation-plugin`: routes work to the owning agent (owners in `.claude/rules/`), sends fact gathering to the Haiku fact-gatherer, bounds every brief, keeps the agent ledger.
- `learn-loop-plugin`: runs `claude-learn` once a session goes idle.
- `report-plugin`: starts a status cron when an agent or workflow runs long.
- `intent-guard-plugin`: holds destructive commands nobody asked for.
- `ask-check-plugin`: stops questions the conversation already settled and unrequested findings.
- `write-gate-plugin`: holds a writer's finish until the humanize and marketing-psychology passes ran.
- `harness-plugin`: guards this file's shape, halts a run on `AGENT_STOP`, redirects it with `STEER.md`; `fix-constitution` realigns the repo.

## Hard Rules

- Internal links are relative; anchors use `#heading-name`.
- Code fences declare a language (`bash`, `python`, `json`, ...).
- If a quality check fails, fix the underlying issue; never bypass with `--no-verify`.
- Write clean code with no noise: no inline comments, no explanatory remarks, no placeholder notes.
- No error handling in scripts unless explicitly requested.

## Working Method

- **Plan first** for anything non-trivial (3+ steps or an architectural decision): write it to `tasks/todo.md`, check in, then track progress there.
- **Verify before reporting done.** Run the check, show the command and its output. A claim is not a receipt.
- **Review `tasks/lessons.md`** at session start; `claude-learn`'s `instruction-audit` gate prunes it and this file.
- **Simplicity first**: minimal diff, root causes over patches, no rewrite to fix a typo.
- **Tutorial over library**: clear explanations and copy-paste examples over reusable abstractions.
