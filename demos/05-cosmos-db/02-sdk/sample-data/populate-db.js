const Cosmos = require("@azure/cosmos");
const yargs = require("yargs");
const food = require("./food.json");
const orders = require("./orders.json");
const customers = require("./customers.json");

// destructure command line arguments
let { endpoint, key, databaseName } = yargs.argv;

// create the cosmos client
const client = new Cosmos.CosmosClient({ endpoint, key });
const database = client.database(databaseName);
const foodContainer = database.container("food");
const ordersContainer = database.container("orders");

// insert the items into Cosmos DB
food.forEach(async food => {
  try {
    await foodContainer.items.create(food);
  } catch (err) {
    console.log(err.message);
  }
});

orders.forEach(async order => {
  try {
    await ordersContainer.items.create(order);
  } catch (err) {
    console.log(err.message);
  }
});

customers.forEach(async customer => {
  try {
    await ordersContainer.items.create(customer);
  } catch (err) {
    console.log(err.message);
  }
});
