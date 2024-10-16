# Web API для мессенджера

Данный проект реализует Web API для мессенджера с поддержкой real-time обмена сообщениями через SignalR.


# Технологии

- ASP.NET Core Web API
- SignalR для real-time взаимодействия
- Entity Framework Core для работы с базой данных
- JWT для аутентификации

# Структура проекта

- **Controllers**
  - `MessageController` — CRUD операции для сообщений.
  - `ChatController` — CRUD операции для чатов.
  - `UserController` — CRUD операции для пользователей, регистрация и аутентификация.
- **ChatHun**
  - Реализует механизм обмена сообщениями в реальном времени.
