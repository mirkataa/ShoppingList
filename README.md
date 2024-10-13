# Shopping List Application

## Overview

The Shopping List Application is a web-based platform that allows users to manage their shopping lists efficiently. Users can create, view, edit, and delete their individual shopping lists, while categories and products are shared among all users. The application is built using ASP.NET Core MVC, Entity Framework Core, and PostgreSQL, and is containerized using Docker.

## Features

- **User Registration and Authentication**: Users can register for an account and log in to access their shopping lists.
- **Role Management**: Admin users can create and manage categories and products, while regular users can only view these items.
- **CRUD Operations**:
  - Create, read (+ see the products contained in the category), update (+ move products between categories), and delete categories.
  - Create, read, update, and delete products.
  - Manage personal shopping lists, including marking items as purchased.
- **Data Persistence**: Utilizes PostgreSQL for data storage with Entity Framework Core for ORM.
- **Docker Support**: Fully containerized application with Docker and Docker Compose.

## Technologies Used

- **ASP.NET Core 8**: Framework for building the web application.
- **Entity Framework Core**: ORM for interacting with the PostgreSQL database.
- **PostgreSQL**: Relational database management system for data storage.
- **Docker**: Containerization platform to package and run the application.

## Setup Instructions

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 8 or later)
- [Docker](https://www.docker.com/get-started)

### Run

- Clone the repository.
- Open the project in a .NET IDE.
- Run Docker Compose to start the web application.
  
### Usage

- User Registration: Click Register to create a new account.

- Log In: Use your registered credentials to log in.
- Admin Credentials (There is only one admin):
```bash
    Username: admina
    Password: Test1234_
```
- Manage Categories and Products: Only Admins can create, edit or delete categories and products. Regular users can only view them.
- Shopping Lists: Create and manage your personal shopping lists. You can add items, mark them as purchased, and delete lists as needed.
