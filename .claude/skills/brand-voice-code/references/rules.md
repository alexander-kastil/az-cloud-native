# Brand Voice Rules

This file is the single source of truth for quality rules enforced by the `brand-voice` skill. Add or update rules here without touching `skill.md`.

---

## Rule 1 — No Em Dashes in Prose

Em dashes (`—`, `—`, or `--` used as one) are banned in body text. Replace them with the closest correct punctuation based on their role in the sentence.

| Em dash role | Replace with |
|---|---|
| Parenthetical aside enclosed by two dashes | `(...)` |
| Clause continuation ("x — so y") | `,` or `;` |
| Explanation / amplification ("x — y") | `:` |
| Simple pause between clauses | `;` |

**Not affected:** em dashes inside module link text (`[01 — Intro]`) or inside code fences — these are intentional and must not be changed.

**Examples:**

```
Before: "They run in your OS shell — outside the LLM entirely — so they are deterministic."
After:  "They run in your OS shell, outside the LLM entirely, so they are deterministic."

Before: "Clients discover tools by querying the schema—giving you full control."
After:  "Clients discover tools by querying the schema, giving you full control."

Before: "A MEMORY.md index acts as a table of contents — it is always loaded into context."
After:  "A MEMORY.md index acts as a table of contents: it is always loaded into context."
```

---

## Rule 2 — Mermaid Node Labels

Inside Mermaid diagrams, `\n` does not render as a newline in node labels. Use `<br/>` instead, and wrap the label in double quotes.

**Pattern to find:** any Mermaid code fence containing `\n` inside `[...]` or `(...)` node syntax.

**Fix:**
```
Before: O[Orchestrator] --> R[Research Agent\nfinds facts]
After:  O[Orchestrator] --> R["Research Agent<br/>finds facts"]
```

Apply to all node styles: `[]`, `()`, `([])`, `[/\]`, `{{}}`.

Do not modify edge labels (`-->|"label"|`) — those do not use `<br/>`.

---

## Rule 3 — Paragraph Length

No paragraph body may exceed 4 sentences. At the 5th sentence, start a new paragraph (blank line before it).

Count only declarative and imperative sentences ending with `.`, `!`, or `?`. Do not count:
- Sentences inside code fences
- Sentences inside blockquotes
- Sentences inside table cells
- List items

When splitting, find the natural break: prefer splitting after a topic shift, not mid-argument. Do not rewrite sentences — only insert the paragraph break.

---

## Rule 4 — Slash Command Tables

Every module README contains a "Helpful Claude Slash Commands" table. Commands in that table must be specific to the module's topic. The same generic set (`/update-config`, `/fewer-permission-prompts`, `/find-skills`, `/init`) must not appear unchanged across every module.

**Do not include `/init` unless the module is explicitly about starting a new project or initializing a `CLAUDE.md`.** Modules where `/init` is appropriate: `01-intro`, `07-memory`.

### Approved command sets by module topic

| Module topic | Preferred commands |
|---|---|
| Harness intro / settings | `/init`, `/update-config`, `/memory`, `/config` |
| Slash commands | `/review`, `/security-review`, `/cost`, `/help` |
| Hooks | `/update-config`, `/fewer-permission-prompts`, `/doctor` |
| MCP | `/update-config`, `/doctor`, `/find-skills` |
| Skills | `/find-skills`, `/update-config`, `/model` |
| Plugins | `/update-config`, `/doctor`, `/find-skills` |
| Memory | `/memory`, `/init`, `/update-config` |
| Checkpoints | `/undo`, `/update-config`, `/clear` |
| Subagents | `/model`, `/cost`, `/find-skills` |

When a module topic is not listed, flag it in the output rather than guessing a command set.

### Built-in vs custom commands

| Command | Type | Description |
|---|---|---|
| `/help` | built-in | Show available commands and keyboard shortcuts |
| `/clear` | built-in | Clear conversation context |
| `/config` | built-in | Open interactive settings panel |
| `/cost` | built-in | Display token usage and session cost |
| `/doctor` | built-in | Health check on installation, MCP servers, hooks |
| `/memory` | built-in | Open global `CLAUDE.md` for editing |
| `/model` | built-in | Switch active model mid-session |
| `/undo` | built-in | Revert most recent file changes |
| `/vim` | built-in | Toggle vim keybindings |
| `/init` | built-in | Generate `CLAUDE.md` for current project |
| `/review` | custom (global) | Structured code review of the current branch |
| `/security-review` | custom (global) | Security audit of branch changes |
| `/update-config` | custom (project) | Edit `settings.json` via guided prompts |
| `/fewer-permission-prompts` | custom (project) | Scan transcripts and build a tool allowlist |
| `/find-skills` | custom (project) | Discover and install agent skills |

---

## Adding New Rules

When a new quality pattern is discovered during editing:
1. Add a numbered rule section here following the same format.
2. Include a "Before / After" example.
3. Update the skill summary comment in `skill.md` description if the rule type is new.

Do not modify `skill.md` instructions for new rules — the instructions deliberately stay generic ("apply the rules from references/rules.md").
