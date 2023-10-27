# Lab 06 - Designing and Implementing Message based & Event Driven Apps

In this lab we will take a look at the message flow between the services and design the message data structures. Most of the practical work will be done in the next lab as we will use Dapr to implement the message flow.

## Task: Examine the Domain Message Flow Model and Design the Message Data Structure

- Examine the `Order Service Domain Message Flow Model`. 

    ![domain-message-flow-model](_images/domain-message-flow.png)

- Design the data structures for the messages that will be exchanged between the services.

- You can use the [Miro - Entity Relationship Diagram Template](https://miro.com/templates/entity-relationship-diagram/) or some other tool or even a piece of paper.

- If you don't want to draw charts just implement the classes and the interfaces for the message flow in `C#` or `TypeScript` and document the message flow in the code. You can do this in a separate project or in plain markdown or a diagram tool of your choice.

## Task: Connect Order Service to the Payment Service and implement a simple Saga