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

**Pattern:** Nine writers built the teaching from one contract (shapes, rules, the Grey Logistics domain, verified facts, slugs and titles). Writers found facts where Learn disagreed with the contract or with itself.
**Rule:** Every demo, lab or teaching writer reads `tasks/todo.md` first, and a fact settled on Learn is fixed in the contract in the same pass, so the next writer inherits it.

## Never echo the owner's own decisions back

**Pattern:** After the owner corrected the run rules, the reply restated three rules the owner had set an hour earlier as if they were news.
**Rule:** A reply names only what changed in that turn; a decision the owner already made is applied silently, never listed back.

## Never add a verification mechanism the owner did not ask for

**Pattern:** The lab plan made xUnit gate tests the proof for every pattern. The owner never asked for them; they doubled the run time of every lab (agents ran them, then the coordinator reran them) and the owner had them removed from all labs.
**Rule:** A lab proves itself through its Expected Outcomes. Add tests, gates or checkers to a class only when the owner asks for them in their own words.
**Status:** each lab step a gate could prove ends with a `> **_NOTE:_**` inside that step naming what the gate would check; lab 2 step 5 carries the one gate a participant writes and runs, at the owner's request.

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
**Status:** the procedure (rule set, per-module agents, SVG second wave, embedding alt texts) is the global create-class `teaching-from-lab-code` leaf.

## Git is the owner's during an execution run

**Pattern:** The owner committed in-progress lab work mid-run; the run then spent turns reading `git log` and `git show` to explain the `M` status of the solution files. The owner: "is git your business or to run all labs?"
**Rule:** During a lab run, never inspect history, commits or the index. Files the owner committed show as modified afterwards; that is expected and needs no explanation.

## Name the lab and the step in every status line

**Pattern:** Over long runs the owner asked "what are you doing?" four times and once thought the run was elsewhere ("i thought you are on lab 8") while a guide fix and a folder reset ran without a status line.
**Rule:** Every status line starts with "Lab N step M:" and the action in progress; a mid-turn question gets that line first.

## A lab's guide is its readme.md

**Pattern:** Each lab had one guide in `lab-01-*.md` beside a readme that only linked it. The owner had the guides moved into the readmes.
**Rule:** A lab's guide is `labs/NN-<module>/readme.md`; never create a separate `lab-NN-*.md`, and point `run-guide-recipes.py --guide` at the readme.
**Status:** since 2026-10-19 `class-time-budget.py` reads a lab folder's `readme.md` when it has no `lab-*.md`, so the budget counts this class's labs.

## A lab guide assumes code no earlier lab built

**Pattern:** Labs 7 to 9 assumed a notification-service, a routing call to replace, "the publisher lab 7 defines", a `parcelId` on `DeliveryDelayed`, Durable Task packages "from the previous lab", and a free `exception-agent` resource name. None existed; each cost a failed run.
**Rule:** Before running a lab, grep every noun its recipes assume in the starter and fix the guide first. In this class the tracking number is the parcel id, and each Durable Task host has its own task hub (`bookings`, `exception-tasks`).
**Status:** the generic rule is in the global create-class `verify-by-execution` leaf; the Aspire, Dapr and Durable Task runtime traps found in labs 7 to 9 are in the global `aspire-dapr-durable` skill.

## A platform limit the lab never deployed stays as Learn states it

**Pattern:** A docs agent settled "routing-service on AKS" against the lab AppHost, which adds it to the Container Apps environment, and rewrote the module 3 overview to "all services run on Container Apps". That contradicted Learn (Container Apps managed Dapr does not support the actor SDK), the AKS topic and the managed-Dapr decision table; the edit was reverted.
**Rule:** The executed lab settles what its code does when it runs. A hosting or platform capability the lab never deployed stays as Learn states it. Accept a delegate's fix to a cross-topic claim only after grepping every topic that states the same fact.

## Hold a module whose lab is running

**Pattern:** Mid-run the owner stopped the module 9 teaching fixes: "09 is currently executing the lab ... so wait with that one". Edits made while the lab runs are checked against code that is still changing.
**Rule:** Before editing a module's teachings, ask whether the owner or a peer session is executing that module's lab, and hold the module until the run is reported complete; then align it with the finished code.

## Teaching diagrams: quote only flowcharts, verify animations by seeking

**Pattern:** The authoring contract said "Mermaid labels use quoted labels", so a 25-agent diagram run quoted sequence diagram messages, which Mermaid prints with visible quote marks, and nine agents needed a correction. Every agent also found that headless Chrome `--virtual-time-budget` captures frame 0 of an animated SVG, and parallel agents moved each other's pages in the shared chrome-devtools browser.
**Rule:** Quote flowchart labels only. Check animated SVG frames on a scratch page that pauses and seeks the animations, and give parallel image agents command-line headless Chrome with their own profile.
**Status:** the contract in `tasks/todo.md` is fixed; the Mermaid rule is in the global mermaid-diagram playbook (pitfall 3) and the repo brand-voice-code rule 2; the SVG rules are in the global image-producer agent (opus-svg drawing rules); widening a running fan-out by file is in the claude-orchestration `fan-out` leaf.

## Split a shared tree with a busy peer session before fanning out

**Pattern:** A diagram session and a teaching-sync session edited the same `demos/` readmes at once. Agents had to work around live edits, and a docs agent kept editing `demos/09-ai-agents` after the peer claimed it, because the stop message reached it only after it had written.
**Rule:** Before fanning out into a tree, list the agents, message the session that edits it, agree an owner per folder, and launch agents only on released folders. After a late stop, ask the agent what it already wrote and send that list to the folder's owner.
**Status:** in the global create-class `teaching-from-lab-code` leaf ("Running it").

## A step's explanation describes that step's build

**Pattern:** The lab 9 step 3 "How it works" paragraph described the agent-state store that step 4 adds, because the docs agent read the finished solution folder, which holds every later step.
**Rule:** Ground each step's explanation in its own recipe and Expected Outcome; use the solution only for exact names, and describe a feature in the step that adds it.
**Status:** in the global create-class `explain-lab-guide` leaf, with the What You Build and one-image-per-step pattern this class's lab guides use.

## Slide specs reuse the class's own _images SVGs in place

**Pattern:** The owner asked for the deck metadata with "each teaching and lab has an _images folder. try to re use the svgs in there", no photos rendered and no Gamma run. The nine module decks got 163 specs: 92 cards point at topic or lab `_images` SVGs as `existing`, 51 new diagrams sit in `pptx/images`, lab slides are `90-NN` specs, and the module-slide portrait prompts are in `demos/pptx/portraits.json` for `batch_spec_photos.py --portraits`. Two gaps surfaced: `gamma_deck.py prompt` only reaches images under `pptx/images`, and the CSS-animated readme SVGs with a hidden start render near-empty as a still.
**Rule:** Keep the in-place reuse. Before the Gamma run, the owner decides how the reused SVGs reach `pptx/images` and how the hidden-start ones get a full still; do neither unasked.
**Status:** settled on 2026-10-19 with the owner's go: each reused SVG got a PNG with the same basename in its module's `pptx/images` (the `svg-frames` no-animation still for the 28 animated ones), committed with the photos, and `prep` printed `bad_urls=0` for all nine modules. The procedure is in the global create-pptx `gamma-deck` leaf (Step 3), and `fix_cropped_diagrams.py` now also repairs the one `layout: mixed` lab card (07 `90-01`) that came back cropped.

## A spec fan-out ran without the peer check and per-agent scratch folders

**Pattern:** Nine image-producer spec agents were launched without listing peer sessions. A peer was editing the `_images` SVGs, the readmes and the create-pptx skill during the run, so two agents rewrote finished specs, and all nine picked the scratch folder `existing` and overwrote each other's copies. Both rules already existed (the claude-orchestration `fan-out` and `cross-session-messaging` leaves, and "Split a shared tree with a busy peer session" above), but the create-pptx path never pointed there. The report ticks also over-counted image calls by grepping tool names that the transcript holds as tool definitions.
**Rule:** Before any fan-out, list the peer sessions and give each brief its own `<scratchpad>/<label>/` folder. Count subagent tool calls from parsed `tool_use` blocks.
**Status:** the global create-pptx `gamma-deck` Step 1 now points at both orchestration leaves; `report-plugin` `scripts/agent-probe.py` counts the calls and is registered in the harvest registry.

## The protected master takes the deck images only through a pull request

**Pattern:** The deck run had the owner's permission to push the 121 new images, but master rejected the push with `GH006: Changes must be made through a pull request` (0 approvals required, admins enforced).
**Rule:** Push the commits to a branch, open a pull request, merge it with `gh pr merge --merge --delete-branch` so the local SHAs survive, then `git merge --ff-only origin/master`. The raw image base keeps `master`.
**Status:** in the global create-pptx `gamma-deck` leaf (Step 4) and the `/create-repo-pptx` step 8 row.
**Update 2026-10-19:** a direct `git push origin master` of the Grey Logistics rename went through (e224f73). When the owner says push to master, push directly first and take the pull request route only on `GH006`.

## Photo prompts: vans need no badge, screens need a still state

**Pattern:** At step 7, three of 29 photos showed a Mercedes star or a Fiat emblem and number plates on the delivery vans outside the window, and the takeaways photo of module 03 rendered two screen bars floating in mid-air twice, because its prompt described the bar changing ("she makes the mint bar a little longer").
**Rule:** Write vans as "seen side-on, far away and softly out of focus, with no badge, emblem or number plate" with `vehicle badges, number plates` in `Avoid:`, and describe a screen as a still blur of colours, never as an action on it.
**Status:** in the global create-pptx `pptx-own-images` leaf (Prompt rules); the five rewritten prompts are in the module specs and `demos/pptx/portraits.json`.

## The deck fan-out shares one portraits file, and finish need not wait for every part

**Pattern:** Five fit-gate agents each covered two modules, but all nine portrait scenes live in one `demos/pptx/portraits.json`; the briefs had each agent return its rewritten scenes as text and the main thread wrote the file once, so no agent overwrote another. At step 10 the poller was asked for an interim list, and seven decks were finished while the last four Gamma parts were still pending. A synchronous photo redo at step 7 had no install command and was written by hand through `batch_spec_photos.save()`.
**Rule:** In a deck fan-out, an agent edits only its modules' spec files and returns shared-file content as text. Finish each module as soon as its parts complete. Install a redo through `save()` with the spec's `media-file` as target.
**Status:** in the global `/create-repo-pptx` command (steps 2 and 11) and the create-pptx `gamma-deck` leaf (Step 2).

## A title review shortens, never lengthens

**Pattern:** Asked whether the class title was the best name, the reply proposed adding ".NET" and swapping a word, making it longer. The owner wanted it less complicated.
**Rule:** When the owner questions a title, propose shorter variants first; detail belongs in the description, not the title.

## Answer the owner's mid-turn question before the next step

**Pattern:** During the Contoso to Grey rename the owner asked "do we still need this folder? app". The status line said the folder was being checked, then the run went on to the commit checks; the owner had to write "before you commit answer my question about the folder".
**Rule:** A question the owner sends mid-turn is answered in the next visible message, before any further step of the queued work and always before a commit.

## The class domain is never a vendor's sample brand

**Pattern:** The class ran on Contoso Logistics. The owner: "contoso is a microsoft brand ... find something new", and picked Grey Logistics (from greyhound), renamed everywhere, texts first.
**Rule:** The business case is Grey Logistics. Never name a scenario company after a Microsoft sample brand (Contoso, Fabrikam, Northwind, Tailspin, Adventure Works, Woodgrove, Litware, Wingtip, Wide World Importers), and check a new name against the stack's product names (Kestrel, Relay, Pinpoint).

## A text sweep does not reach the decks

**Pattern:** The repo-wide rename edited 698 text files and the final grep found no Contoso, yet 16 of the 27 committed module decks still carried it in their slide XML, and so did their PDFs. The closing reply then said the decks needed a Gamma rebuild.
**Rule:** A rename "everywhere" includes the zipped Office files: grep the XML inside every .pptx, .docx and .xlsx, fix hits run by run and re-export the PDFs, as the create-pptx `finished-deck-edits` leaf describes. Text and redrawn diagrams in a finished deck never need Gamma.
**Status:** the check is `rename-sweep.py --mode office` in the global windows-project-rename skill. The az-native decks and PDFs still say Contoso.

## Prove a rename against HEAD, not with git diff -M

**Pattern:** Every lab keeps a starter and a solution copy of the same tree, so `git diff -M` paired renamed files with their twins in other folders, and a diff-based check reported 2,473 edits that never happened. A raw byte compare flagged every file, because autocrlf returns LF blobs against a CRLF tree.
**Rule:** Verify a sweep file by file: map the path back, apply the substitution to the HEAD blob and compare CRLF-normalized. Zero mismatches proves the change is the rename.
**Status:** in the global windows-project-rename skill (section 3b, `scripts/rename-sweep.py --mode verify`).
