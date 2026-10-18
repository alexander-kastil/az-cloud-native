---
slide: "06-01"
topic: 06-testing
title: "Testing the real AppHost"
subtitle: "Aspire.Hosting.Testing starts every resource inside a test"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/02-aspire/pptx/images/06-01-test-hosts.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, 'Segoe UI', Inter, Arial,
  sans-serif as in the module's diagrams, text #0F172A, secondary #64748B, arrows #334155, minimum 26px
  type. No heading inside the image; the slide title carries it. No arrow crosses a box or a label. Left
  frame "Test process with WebApplicationFactory": a grey Test box, an arrow "HTTP through TestServer"
  down to a blue "shipping-api, in memory" box, a purple "Stub, fake clock or fake client" box below with
  an arrow up "replaces one registration", footer "starts quickly and needs no container runtime". Right
  frame "Test process with DistributedApplicationTestingBuilder": a grey Test box with an arrow
  "StartAsync" to a white AppHost box. Below the frame a connector from the AppHost fans out to four
  boxes: blue "shipping-api, separate process", blue "tracking-service, separate process", amber
  "Service Bus, emulator container", green "Cosmos DB, emulator container"; a second arrow "real HTTP"
  runs from the Test box down to shipping-api. Footer "starts every AppHost resource, including
  emulators" and "needs a container runtime and starts more slowly".
order: 14
source: demos/02-aspire/06-testing/readme.md
gamma_prompt: >
  Slide titled "Testing the real AppHost", subtitle "Aspire.Hosting.Testing starts every resource inside
  a test". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Testing the real AppHost

## Content

- `DistributedApplicationTestingBuilder.CreateAsync` takes the AppHost type and starts it in the test
- `CreateHttpClient("shipping-api")` sends **real HTTP** to a real process
- `await using` stops the containers; a cancellation token caps the whole test
- `WebApplicationFactory` hosts one app in memory for logic inside a single service

## Speaker Notes

A parcel booking crosses the shipping API, the message bus and the tracking projection, and a unit test of any one of them misses the handoffs. `Aspire.Hosting.Testing` runs the real AppHost inside a test.

CreateAsync: the test project references the AppHost project, and `CreateAsync` takes the AppHost type as its generic argument. The test then builds and starts the application with the same wiring that `aspire run` uses.

CreateHttpClient: it returns an `HttpClient` whose base address points at the named resource. The test sends its first request after `WaitForResourceHealthyAsync` reports the resource healthy.

Cleanup: the `await using` declaration disposes the application and stops the containers the test started. The cancellation token makes a stuck resource fail the test instead of hanging the build agent.

WebApplicationFactory: it starts quickly, needs no container runtime and can replace one registration with a stub such as a fake clock. The testing builder fits flows that cross services and brokers, and the two tools combine well.
