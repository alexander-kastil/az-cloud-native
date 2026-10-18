import hashlib
import os
import time

from azure.cosmos import CosmosClient, PartitionKey
from azure.identity import DefaultAzureCredential, get_bearer_token_provider
from openai import AzureOpenAI

DATABASE = "logistics"
CONTAINER = "delivery-notes"
POLL_SECONDS = 5

VECTOR_EMBEDDING_POLICY = {
    "vectorEmbeddings": [
        {
            "path": "/embedding",
            "dataType": "float32",
            "dimensions": 1536,
            "distanceFunction": "cosine",
        }
    ]
}

INDEXING_POLICY = {
    "indexingMode": "consistent",
    "automatic": True,
    "includedPaths": [{"path": "/*"}],
    "excludedPaths": [{"path": '/"_etag"/?'}, {"path": "/embedding/*"}],
    "vectorIndexes": [{"path": "/embedding", "type": "diskANN"}],
}


def text_hash(text):
    return hashlib.sha256(text.encode("utf-8")).hexdigest()


def open_container():
    settings = dict(part.split("=", 1) for part in os.environ["ConnectionStrings__logistics"].split(";") if part)
    client = CosmosClient(
        settings["AccountEndpoint"],
        credential=settings["AccountKey"],
        connection_verify="localhost" not in settings["AccountEndpoint"],
    )
    database = client.create_database_if_not_exists(id=settings.get("Database", DATABASE))
    return database.create_container_if_not_exists(
        id=CONTAINER,
        partition_key=PartitionKey(path="/parcelId"),
        indexing_policy=INDEXING_POLICY,
        vector_embedding_policy=VECTOR_EMBEDDING_POLICY,
    )


def open_embedder():
    token_provider = get_bearer_token_provider(
        DefaultAzureCredential(), "https://cognitiveservices.azure.com/.default"
    )
    return AzureOpenAI(
        azure_endpoint=os.environ["FOUNDRY_ENDPOINT"],
        azure_ad_token_provider=token_provider,
        api_version="2024-10-21",
    )


def embed(embedder, text):
    response = embedder.embeddings.create(model=os.environ["EMBEDDING_DEPLOYMENT"], input=text)
    return response.data[0].embedding


def similar_notes(container, vector):
    return list(
        container.query_items(
            query="SELECT TOP 5 c.id, c.parcelId, c.text, VectorDistance(c.embedding, @vector) AS score "
            "FROM c ORDER BY VectorDistance(c.embedding, @vector)",
            parameters=[{"name": "@vector", "value": vector}],
            enable_cross_partition_query=True,
        )
    )


def process(container, embedder, note):
    text = note.get("text")
    if not text:
        return
    digest = text_hash(text)
    if note.get("textHash") == digest:
        return
    note["embedding"] = embed(embedder, text)
    note["textHash"] = digest
    container.upsert_item(note)


def main():
    container = open_container()
    embedder = open_embedder()
    response = container.query_items_change_feed(start_time="Beginning")
    while True:
        for note in response:
            process(container, embedder, note)
        token = container.client_connection.last_response_headers["etag"]
        time.sleep(POLL_SECONDS)
        response = container.query_items_change_feed(continuation=token)


if __name__ == "__main__":
    main()
