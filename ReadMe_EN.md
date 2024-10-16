[Русская версия](https://github.com/Chesno4ok/MessengerAPI-WIP/blob/master/README.md)

# Web API for Messenger

This project implements a Web API for a messenger with support for real-time messaging through SignalR. You can view the desktop application [here](https://github.com/Chesno4ok/Avalonia-Messenger-WIP).

## Technologies

- ASP.NET Core Web API
- SignalR for real-time interaction
- Entity Framework Core for database access
- JWT for authentication
- PostgreSQL

## Project Structure

- **Controllers**
  - `MessageController` — CRUD operations for messages.
  - `ChatController` — CRUD operations for chats.
  - `UserController` — CRUD operations for users, registration, and authentication.
- **ChatHub**
  - Implements the mechanism for real-time messaging.

## Setup

1. Set up the database using the backup MessengerDatabase.backup.
2. Specify the connection string in the `appsettings.json` file.
