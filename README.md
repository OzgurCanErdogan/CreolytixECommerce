# Creolytix E-Commerce API - Design Decisions

This document outlines the core architectural and design decisions made in developing the Creolytix E-Commerce API. Each decision aims to enhance modularity, scalability, and maintainability in a microservices-oriented environment.

---

## 1. Architecture Pattern: Clean Architecture

**Objective**: Achieve separation of concerns by organizing code into layers with explicit responsibilities and dependencies.

### Layers

- **Domain Layer**: Contains core business logic, entities, and service interfaces. It’s independent of any external frameworks or technologies.
- **Application Layer**: Implements the business use cases and application services, handling commands and queries while coordinating data flow across the system.
- **Infrastructure Layer**: Manages integrations with external services, data persistence, and messaging. This layer implements repository interfaces, messaging services, and interacts with external dependencies like MongoDB and RabbitMQ.
- **Presentation Layer (API)**: Exposes endpoints to clients, receiving requests and responding accordingly. Controllers handle requests related to commands and queries.

---

## 2. Design Patterns and Principles

### CQRS (Command Query Responsibility Segregation)

**Objective**: Separate write (command) and read (query) operations, making scaling and optimization easier.

- **Implementation**:
  - **Commands**: Used for actions, e.g., `CreateReservationCommandHandler`.
  - **Queries**: Used to fetch data, e.g., `GetStoreProductsQueryHandler`.

### Mediator Pattern

- **Purpose**: Decouples controllers from the application layer by sending commands and queries to their respective handlers.

### Repository Pattern

**Objective**: Abstracts the data access layer, enabling easy data storage mechanisms and unit testability.

- **Implementation**: MongoDB is accessed through repository classes, providing methods for CRUD operations, e.g., `IProductRepository`, `IReservationRepository`.

### Event-Driven Communication

**Objective**: Decouples services, promotes asynchronous communication, and improves scalability.

- **Implementation**:
  - **RabbitMQ**: Used as a message broker, allowing microservices to communicate asynchronously. For instance, consumers listen to specific queues, e.g., `CreateReservationConsumer`.
  - **Message Wrappers**: To ensure reliable message tracking, messages include a `CorrelationId` and are wrapped in `MessageWrapper<T>` objects, facilitating response handling based on `CorrelationId`.

### Microservices Pattern

**Objective**: Decompose services into self-contained units based on business capabilities, improving deployment independence and scalability.

- **Implementation**:
  - Each service (e.g., Reservation, Product) communicates through a message broker, sharing only essential data and functionality.
  - **Docker and Docker Compose**: Services are containerized for isolated, scalable deployment.

---

## 3. Error Handling and Result Wrapping

### Success/Error Handling

- **Standardized Response Structure**: Commands and queries return responses with `Success` and `Message` fields for uniform error handling.
- **Result Wrapping**: Commands implement a `ResultWrapper<T>` wrapper for consistent handling of success, failure, and associated messages.

### Asynchronous Processing

- **Non-Blocking Communication**: `IMessageListener` and `IMessagePublisher` process messages asynchronously to efficiently handle requests without blocking.

---

## 4. Dependency Injection and Inversion of Control (IoC)

**Objective**: Inject dependencies to promote modularity, improve testability, and efficiently manage the lifecycle of services.

- **Implementation**:
  - **Service Lifecycles**: Services are registered as Scoped, Singleton, or Transient based on their lifecycle requirements.
  - **Centralized Configuration**: Key settings, such as database connections, message broker settings, and environment configurations, are managed in `appsettings.json`.

---

## 5. Containerization and Port Management

**Objective**: Enable isolated and reproducible environments, ensuring easy setup and minimal conflicts for other developers.

- **Implementation**:
  - **Docker Compose**: Each service (API, RabbitMQ, MongoDB) is defined in `docker-compose.yml`, with custom port mappings to avoid conflicts.
  - **Dockerfile**: The .NET API project uses multi-stage builds to optimize image size and build time.

---
