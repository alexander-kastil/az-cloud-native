const Cosmos = require("@azure/cosmos");
const food = require("./food.json");
const yargs = require("yargs");

// destructure command line arguments
let { endpoint, key, databaseName, containerName } = yargs.argv;

// create the cosmos client
const client = new Cosmos.CosmosClient({ endpoint, key });
const database = client.database(databaseName);
const container = database.container(containerName);

// insert the items into Cosmos DB
food.forEach(async product => {
  try {
    await container.items.create(product);
  } catch (err) {
    console.log(err.message);
  }
});
