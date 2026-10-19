---
slide: "05-01"
topic: 05-polyglot
title: "Hosting calls for JavaScript and Python"
subtitle: "The call follows how the app starts"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/02-aspire/pptx/images/05-01-hosting-calls.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, 'Segoe UI', Inter, Arial,
  sans-serif as in the module's diagrams, text #0F172A, secondary #64748B, minimum 26px type. No heading
  inside the image; the slide title carries it. Two framed columns. Left "Aspire.Hosting.JavaScript",
  "runs the app's package script", three blue #DBEAFE rows: AddJavaScriptApp(name, appDirectory) "a Node
  app that runs its dev script by default"; AddViteApp(name, appDirectory) "a Vite app, including
  Angular 17 and later" and "registers an http endpoint and sets PORT", with a thicker border and a pink
  pill "ops-console"; AddNodeApp(name, appDirectory, scriptPath) "a Node server started from a script
  file"; footer "every call takes the app directory as its second argument". Right
  "Aspire.Hosting.Python", "uses the .venv folder in the app directory", four green #DCFCE7 rows:
  AddPythonApp(name, appDirectory, scriptPath) "a Python script"; AddUvicornApp(name, appDirectory, app)
  "an ASGI app in module:variable form, such as main:app", thicker border and pink pill
  "exception-agent"; AddPythonModule(name, appDirectory, moduleName) "a module, like python -m";
  AddPythonExecutable(name, appDirectory, executableName) "a CLI from the virtual environment"; footer
  "WithVirtualEnvironment points at another location".
order: 12
source: demos/02-aspire/05-polyglot/readme.md
gamma_prompt: >
  Slide titled "Hosting calls for JavaScript and Python", subtitle "The call follows how the app
  starts". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Hosting calls for JavaScript and Python

## Content

- `AddJavaScriptApp` runs a Node app's `dev` script, `AddNodeApp` a script file
- `AddViteApp` hosts Vite apps, including **Angular** 17 and later, and sets `PORT`
- `AddPythonApp` runs a worker script, `AddUvicornApp` an ASGI app such as a FastAPI agent
- Aspire uses the `.venv` in the app directory; `WithVirtualEnvironment` points elsewhere

## Speaker Notes

Grey's operations console is an Angular app and its exception agent is a Python service, so the AppHost needs hosting calls for apps that are not .NET.

JavaScript apps: `Aspire.Hosting.JavaScript` creates a resource that runs the app's package script when the AppHost starts. Every call takes the app directory as its second argument, relative to the AppHost project.

Vite and Angular: `AddViteApp` registers an `http` endpoint and sets `PORT`, so a Vite resource needs no `WithHttpEndpoint` call. Older samples use `AddNpmApp`, while the current JavaScript page documents the three calls in the diagram.

Python apps: `Aspire.Hosting.Python` adds `AddPythonApp` for a script and `AddUvicornApp` for an ASGI app in `module:variable` form. `AddPythonModule` runs a module like `python -m`, and `AddPythonExecutable` runs a CLI from the virtual environment.

Virtual environment: Aspire detects the `.venv` folder from `requirements.txt` or `pyproject.toml`, so the agent's packages never mix with other Python tools on the machine. `WithVirtualEnvironment` points at another location.
