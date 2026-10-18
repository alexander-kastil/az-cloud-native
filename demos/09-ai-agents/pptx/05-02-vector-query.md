---
slide: "05-02"
topic: 05-vector-search
title: "Vector index and similarity query"
subtitle: "DiskANN on /embedding, TOP 5 by VectorDistance"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/09-ai-agents/05-vector-search/_images/embedding-space.svg"
media-file: demos/09-ai-agents/05-vector-search/_images/embedding-space.svg
visual-prompt: >
  Existing diagram of a 2D sketch of the 1536-dimension embedding space, where a TOP 5 circle around the query "left with neighbour, gate code wrong" holds five delivery notes that share no word with it, no generation. Referenced at its repo path.
order: 14
source: demos/09-ai-agents/05-vector-search/readme.md
gamma_prompt: >
  Slide titled "Vector index and similarity query", subtitle "DiskANN on /embedding, TOP 5 by
  VectorDistance". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Vector index and similarity query

## Content

- `vectorEmbeddings`: /embedding, float32, 1536 dimensions, cosine distance
- A **diskANN** index on the same path; both policies are immutable after creation
- `flat` accepts at most 505 dimensions, so 1536 needs quantizedFlat for a brute-force scan
- `TOP 5 ... ORDER BY VectorDistance`; raise searchListSizeMultiplier when recall matters
- A model change means a new container and a full re-embedding run

## Speaker Notes

The vectors become useful through two container policies and one query, and the sketch shows what that query finds. The five nearest notes share no word with the query "left with neighbour, gate code wrong", yet they describe the same situation.

Policies: the vector embedding policy describes /embedding as float32 vectors of 1536 dimensions, the default size of text-embedding-3-small, compared with cosine distance. The indexing policy adds a diskANN index on that path, the index type for large collections, and excludes the path from the range index.

Index types: DiskANN is approximate, so the same query can return slightly different results across runs. The flat type is exact but accepts at most 505 dimensions, so 1536-dimension embeddings need quantizedFlat for a brute-force scan with a small accuracy loss.

The query: VectorDistance orders by distance with TOP, because an unbounded vector query costs more request units and adds latency. Raise searchListSizeMultiplier for one query when recall matters more than latency, and allow cross-partition execution when there is no partition key filter.

Model changes: Microsoft documents the vector policy and the vector index as immutable after creation. A new model or dimension count means a new container, a full re-embedding run, and switching the reads once the backfill finishes.
