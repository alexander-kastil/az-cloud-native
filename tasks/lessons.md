# Lessons

## The root CLAUDE.md describes another class

**Pattern:** The untracked root `CLAUDE.md` describes "Agentic Software Engineering with Claude Code" with 13 modules, while `readme.md`, `demos/` and `labs/` are the Azure cloud-native class.
**Rule:** Scope work in this repo from `readme.md` and a listing of `demos/`, never from `CLAUDE.md`, until that file is replaced.

## The demos index is not the module list

**Pattern:** `demos/readme.md` lists nine modules, including a .NET Aspire module and no containers module, while disk holds `00-app` to `09-micro-frontends` with no Aspire folder; many topic readmes also link files and folders that no longer exist.
**Rule:** Derive module and topic lists from the folders on disk, and check every link a readme gives before relying on it.
**Status:** the old tree now lives in `demos-old/` and `labs-old/`; since 2026-10-18 `demos/readme.md` is written from the agreed layout and matches disk (`toc-sync.py` passes). The rule still holds for the old trees.

## The class is about event-driven distributed applications

**Pattern:** A market-demand study rated the class against the AI-200 exam outline and marked Dapr and Aspire optional. The owner corrected it: the class teaches event-driven distributed applications, and Dapr and Aspire are its backbone, not side products. The market-demand leaf already says a certification outline is not the yardstick.
**Rule:** Rate and refresh this class against demand for event-driven distributed systems (messaging, event sourcing, CQRS, sagas, workflows, actors, Dapr, Aspire). Never propose Dapr, Aspire or the messaging modules as optional or cuttable; a certification outline is supporting evidence only.
**Status:** addressed in the global market-demand leaf (subject from the course description, backbone never optional, investment as a fourth evidence kind, reading a sparse Trends anchor).

## AZ-204's successor is AI-200, not AI-103

**Pattern:** The owner believed AI-103 replaced AZ-204. Microsoft Learn shows AZ-204 retired on 31 Jul 2026 and AI-200 (Azure AI Cloud Developer Associate) carries its developer content; AI-103 replaced AI-102. The owner then asked to take AI-200 into account in planning.
**Rule:** State the prerequisite as "AI-200 (formerly AZ-204) level" and position the class as the step above AI-200; map new topics against the AI-200 skills outline, but keep the event-driven backbone as the yardstick.

## The authoring contract lives in tasks/todo.md

**Pattern:** Nine writers built the teaching from one contract (shapes, rules, the Contoso Logistics domain, verified facts, slugs and titles). Writers found facts where Learn disagreed with the contract or with itself.
**Rule:** Every demo, lab or teaching writer reads `tasks/todo.md` first, and a fact settled on Learn is fixed in the contract in the same pass, so the next writer inherits it.

## Never echo the owner's own decisions back

**Pattern:** After the owner corrected the run rules, the reply restated three rules the owner had set an hour earlier as if they were news.
**Rule:** A reply names only what changed in that turn; a decision the owner already made is applied silently, never listed back.

## Never add a verification mechanism the owner did not ask for

**Pattern:** The lab plan made xUnit gate tests the proof for every pattern. The owner never asked for them; they doubled the run time of every lab (agents ran them, then the coordinator reran them) and the owner had them removed from all labs.
**Rule:** A lab proves itself through its Expected Outcomes. Add tests, gates or checkers to a class only when the owner asks for them in their own words.

## Nothing goes above the H1 of a teaching

**Pattern:** Every topic readme opened with `minutes:` front matter, so the duration rendered above the title on GitHub. The owner had it moved.
**Rule:** A topic readme starts with its H1; the duration is `> **_NOTE:_** Duration: about N minutes.` on the line after the H1.

## A topic duration is measured, never a default

**Pattern:** All 56 topic readmes said 10 minutes. The rule (prose words / 80, rounded to 5, at least 10) cannot produce anything else for 600 to 900 words, and the numbers were never counted. The owner called the timing useless.
**Rule:** Count each topic's prose words (no code, no tables, no Key Topics), divide by 80 and round to the whole minute.

## Settle content contradictions from the repo, never hand them back

**Pattern:** After the diagram run, the reply listed nine readme contradictions and two stale sentences as decisions for the owner. The owner had already said everything needed, and the lab solution code, the labs' docs (events.md, architecture.md) and the product docs answer each one.
**Rule:** When readmes contradict each other or a sentence no longer matches a diagram, settle it from the lab solution code and its docs, then Microsoft Learn or the product docs, fix the prose to match, finish the blocked work, and report what changed.

## The executed lab code is the truth for the teachings

**Pattern:** A teaching sync treated the teachings as settled and changed them only where the lab code proved a line wrong; every "design difference" (a status code, a filter type, a retry policy, an event on another broker, an SVG showing the old design) went back to the owner as a question. The owner: "the code that executes correctly defines the truth and what should be in the teachings".
**Rule:** Teachings (prose, code samples, tables, Mermaid and SVGs) say what the executed lab solution does; where labs differ, the latest executed lab that practices the capability wins. Align every difference in the same pass and report what changed, never a list of differences to decide.
