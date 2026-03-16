# Hortifia Backend API

A RESTful API built with C# ASP.NET Core 10 to support a mobile application helping users care for houseplants. It allows users to manage rooms, plants, and environmental data, receive watering reminders, and access care recommendations. The system also provides a forum for users to share knowledge and experiences.

The backend is part of the full project - the **frontend mobile application is available [here](https://github.com/crinsay/hortifia-client-mobile).**


## Table of Contents

- [Features](#features)
  - [User Management](#user-management)
  - [Plant Management](#plant-management)
  - [Room Management](#room-management)
  - [Weather Integration](#weather-integration)
  - [Forum](#forum)
  - [Notifications](#notifications)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Database Model](#database-model)
  - [Key Entities](#key-entities)
- [Screenshots](#screenshots)


## Features

### User Management
- **Registration & Login** - Users can create accounts and sign in using email and password.
- **Authentication**
- **Notifications** - Store multiple devices per user for push notifications.
- **Custom Notification Schedule** - Users can choose preferred notification times.
- **Profile Management** - Update account information, preferences, or delete the account.

### Plant Management
- **CRUD Operations** - Add, update, and delete plants.
- **Care Recommendations** - Retrieve plant care data from Permapeople API.
- **Watering Scheduler** - Automatic calculation of next watering date based on environmental conditions.
- **Favorites & Quick Actions** - Mark plants as favorites or water them individually / in bulk.

### Room Management
- **Create & Edit Rooms** - Define rooms with name, type, temperature, and humidity.
- **Room-Plant Association** - Assign plants to rooms to track environmental conditions.
- **Dashboard View** - See all plants in a room and manage them efficiently.

### Weather Integration
- **Location-Based Forecast** - Display current weather for the user’s location.
- **Impact on Plants** - Weather data influences watering reminders.

### Forum
- **Posts** - Create, edit, and delete forum posts.
- **Featured Post** - View the most popular post from recent activity.
- **Reactions** - Like posts.
- **Hashtags & Filtering** - Tag posts and filter by hashtags or categories.
- **Search & Sorting** - Quickly find posts by keywords or popularity.


## Architecture

The Hortifia backend is structured as a **RESTful API** built in C# 14 ASP.NET Core 10, It follows **Clean Architecture**, **SOLID principles**, and **Domain-Driven Design (DDD)**, ensuring maintainability, scalability, and testability. The system is divided into four main layers:

**1. Presentation Layer**
- Handles incoming HTTP requests via **controllers** and **endpoints**.  
- Serves as the entry point of the API.  
- Uses **DTOs (Data Transfer Objects)** to send only necessary data to the client.  
- Implements **Mediator pattern** to decouple controllers from application logic.  

**2. Application Layer**
- Coordinates business logic and workflows across the system.  
- Implements **CQRS (Command-Query Responsibility Segregation)** to separate write operations (Commands) from read operations (Queries).  
- Orchestrates interactions between domain entities.  
- Returns **Result<T> objects** to wrap success or failure of operations, avoiding nulls and unhandled exceptions.  

**3. Domain Layer**
- Contains **entities**, rich domain models, and business rules.  
- Implements **Domain-Driven Design (DDD)** principles for accurate modeling of business concepts.  
- Ensures consistency and enforces business rules independently of UI or infrastructure.  

**4. Infrastructure Layer**
- Handles all external integrations, such as:  
  - Database access via **Entity Framework Core**  
  - Push notifications using **Firebase Cloud Messaging**  
  - External APIs (Permapeople, Open-Meteo, OpenStreetMap)  
- Uses **Repository pattern** to abstract data access and improve testability.  
- Atomic operations.
- Manages component lifetimes and dependencies via **Dependency Injection (DI)**.  

**Key Design Patterns & Principles**
- Clean Architecture
- SOLID Principles 
- Mediator & CQRS
- DTOs
- Result Pattern


## Technology Stack

**Backend:**
- C# 14 - language used
- ASP.NET Core 10 - main framework for REST API  
- Entity Framework Core & SQL Server - database and ORM 
- Azure Blob Storage - file storage
- MediatR - mediator pattern implementation  
- FluentValidation - request validation  
- Swagger (Swashbuckle) - API documentation and testing  
- Quartz.NET - background jobs for notifications  
- FirebaseAdmin & Google.Apis.Auth - push notifications and auth  

**Frontend:**

- The mobile client is implemented in React Native with Expo and TypeScript and([frontend repository](https://github.com/crinsay/hortifia-client-mobile))  

**External APIs:**

- Plant data: [Permapeople API](https://permapeople.org/knowledgebase/api-docs.html?)
- Weather: [Open-Meteo](https://open-meteo.com)
- Geolocation: [Nominatim OpenStreetMap](https://nominatim.openstreetmap.org/ui/search.html)


## Database Model
Hortifia uses a single relational database Microsoft SQL Server. The backend leverages Entity Framework Core (EF Core) for ORM and schema management.

### Key Entities

- **Users** - central entity based on Microsoft Identity, containing identity data along with additional custom fields. Each user can have:
  - Multiple device tokens (`UserDeviceTokens`) for push notifications
  - Multiple rooms (`Rooms`)
  - Multiple plants (`Plants`)
  - Forum posts (`Posts`) and reactions (`PostLikes`)
- **UserDeviceTokens** - stores unique device tokens per user for sending notifications.  
- **Rooms** - represents rooms with custom environmental conditions. Each room belongs to one user and can contain many plants.  
- **Plants** - each plant belongs to a single user and room. Plant species and care requirements are retrieved from the external Permapeople API (`PlantApiId`). Environmental conditions of the room influence watering reminders.  
- **Posts** - forum posts with title, content, creation date, optional image, and author. Posts can have:
  - Multiple likes (`PostLikes`)
  - Multiple hashtags (`Hashtags`)  
- **PostLikes** - a many-to-one relationship linking users to posts, with a composite key (`UserId`, `PostId`) and a timestamp (`LikedAt`) for tracking popularity over time.  
- **Hashtags** - contains the hashtag text and links to a single post.

The database is designed following normalization principles, ensuring each entity represents a single logical concept, with primary keys and proper foreign keys to enforce relationships.

<img width="2515" height="2034" alt="Entity diagram" src="https://github.com/user-attachments/assets/7e2930c3-11bb-4ec2-ad55-d524995d8266" />

## Screenshots

<img width="752" height="760" alt="IdentitySwagger" src="https://github.com/user-attachments/assets/add99706-04dc-4c6b-a09e-3ff4c6fefbde" />

<img width="753" height="760" alt="LocationSwagger" src="https://github.com/user-attachments/assets/8242152f-950a-4af8-b301-652ff542237c" />

<img width="754" height="760" alt="PostsSwagger" src="https://github.com/user-attachments/assets/b394c77e-e8c5-4fe3-80a6-95b89003e743" />

<img width="1006" height="760" alt="WeatherSwagger" src="https://github.com/user-attachments/assets/96f709eb-1777-457f-8b61-aafe80416488" />

