# Matchmaking Service

This project is a matchmaking service designed to handle user event processing and match creation. It leverages **Kafka** for message streaming and **Docker Compose** for managing dependencies. The service includes multiple workers for processing events and forming user matches.

## Project Structure

- **Kafka**: Used for handling event streaming and data exchange between workers.
- **Redis**: Used for storing the state of users and the match queue.
- **.NET Worker**: Workers that process user events and create matches.

## Requirements

Before running the project, ensure you have the following installed:

- **Docker** and **Docker Compose**: For building and managing containers.
- **Kafka** and **Redis**: For message exchange between the services.

## Running the Project

Use **Docker Compose** to run the project. It will automatically spin up all necessary services, including Kafka and Redis, and start two instances of the worker to process events.

## Running the Project

To start the project, follow these steps:

1. **Clone the repository**  
   Open a terminal and run the following command to clone the repository:
   
   ```bash
   git clone https://github.com/KanstantsinTrush/MatchMaking.git
   cd MatchMaking
2. **Build and start services using Docker Compose**  
   To build and start the services, use Docker Compose with the following command:

   ```bash
   docker-compose up --scale matchmaking-worker=2