---
slide: "05-01"
topic: 05-vector-search
title: "Embeddings from the change feed"
subtitle: "A text hash check ends the write-back loop"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/09-ai-agents/_images/change-feed-embedding.svg"
media-file: labs/09-ai-agents/_images/change-feed-embedding.svg
visual-prompt: >
  Existing diagram of the embedding-worker reading the delivery-notes change feed, skipping notes whose textHash is unchanged, calling grey-embed and upserting the embedding, with the vector policy and the similar_notes query, no generation. Referenced at its repo path.
order: 13
source: demos/09-ai-agents/05-vector-search/readme.md
gamma_prompt: >
  Slide titled "Embeddings from the change feed", subtitle "A text hash check ends the write-back loop".
  Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided diagram
  as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped,
  never used as a background or side accent. Use exactly the one image given for this card; do not
  generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Embeddings from the change feed

## Content

- Every insert or update of a delivery note appends a **change feed** entry
- The embedding-worker embeds each changed note; scanner and driver apps never call the model
- Writing the vector back is another change, so the worker skips notes whose **textHash** is unchanged
- A separate embeddings container removes the loop by construction
- Change feed consumers run at least once: a replay costs a model call and nothing else

## Speaker Notes

Topic 05 makes delivery notes searchable by meaning, because drivers phrase the same problem in many ways and keyword search misses most of them. The diagram is the lab's step 7, the Python embedding-worker on the delivery-notes change feed.

The trigger: every insert or update in the delivery-notes container appends a change feed entry, and the worker computes an embedding for each changed note. The write path stays simple, because the scanner and driver apps never call the embedding model.

The loop: writing the vector back into the container triggers another change feed entry, so the worker skips any note whose text hash has not changed. A separate embeddings container removes the loop by construction, and then the worker must also see deletes, which the AllVersionsAndDeletes mode returns.

At least once: the worker is a projection like the tracking read model from Module 6, so it must be idempotent. A replay overwrites the vector with one for the same text, which costs a model call and nothing else.

Updates: an edit produces a new entry, and the worker replaces the vector in the same upsert. A reader in between sees the old vector, which matches the eventual consistency the design already accepts, and the worker should log the lag.
