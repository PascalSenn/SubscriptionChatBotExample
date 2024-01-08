# GraphQL Subscriptions with HotChocolate: A Chat Bot Example Project

Welcome to this sample project that demonstrates how to use GraphQL subscriptions, particularly in the
context of a chat bot, (that may use OpenAI's chat completions under the hood). This project is a practical guide to
understand and implement GraphQL subscriptions using HotChocolate.

## Getting Started

To get started with this project, follow these simple steps:

1. **Run the Project**: Use the command `dotnet run` to start the project.
2. **Access GraphQL Editor**: Navigate to `http://localhost:5000/graphql` on your browser to open the GraphQL editor.

## Project Workflow

Here's a step-by-step guide to test the GraphQL subscriptions:

### Step 1: Create a New Chat

Initiate a new chat using the following GraphQL mutation:

```graphql
mutation CreateChat {
    createChat {
        chat {
            id
            status
        }
        errors {
            ... on UnauthorizedError {
                message
            }
        }
    }
}
```

### Step 2: Subscribe to Chat Messages

In a new tab within the editor, subscribe to chat messages updates using the following subscription:

```graphql
subscription onChatMessagesUpdated($chatId: ID!) {
    onChatMessagesUpdated(chatId: $chatId) {
        ... on ChatMessageCreated {
            message {
                ...ChatMessage
            }
        }
        ... on ChatMessageUpdated {
            message {
                ...ChatMessage
            }
        }
    }
}

fragment ChatMessage on ChatMessage {
    id
    content
    role
    sentAt
}
```

### Step 3: Send a Message

Send a new message to the chat using the mutation below:

```graphql
mutation SendMessage($chatId: ID!, $content: String!) {
    sendMessage(
        input: {
            chatId: $chatId
            content: $content
        }
    ) {
        message {
            id
            content
            role
            sentAt
        }
        errors {
            ... on ChatClosedError {
                message
            }
            ... on ChatNotFoundError {
                message
            }
            ... on ChatNotReadyForMessageError {
                message
            }
            ... on UnauthorizedError {
                message
            }
        }
    }
}
```

### Step 4: Monitor Subscription Updates

Watch the subscription tab for incoming messages. Notice how the subscription also notifies you of partial message
updates from the assistant.

### Step 5: Blocking the Message During Reply

Attempt to execute the 'send message' mutation again and observe that it gets blocked while the assistant is replying.

### Step 6: Closing the Chat

Close the chat using the following mutation:

```graphql
mutation closeChat($chatId: ID!) {
    closeChat(input: {
        chatId: $chatId
    }){
        chat {
            id
            status
        }
        errors {
            ... on UnauthorizedError {
                message
            }
            ... on ChatNotFoundError {
                message
            }
        }
    }
}
```

Watch the subscription tab to see the completion of the subscription.

### Step 7: Handling Closed Chat

Try executing the 'send message' mutation again and observe that it is blocked due to the chat being closed.

### Links

- **HotChocolate**: https://chillicream.com/docs/hotchocolate
- **Slack**: https://slack.chillicream.com/
- **ChilliCream**: https://chillicream.com/
