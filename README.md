# Sila Technologies Product Server

## Getting started

### Starting the database

Pull the mongo docker container from the docker hub by running the following:

```bash
docker pull mongo
```

Start the docker container in the background by running the following: (Omit the `-d` flag to run in the foreground)

```bash
docker run -d --name Sila -p 12345:27017 mongo
```

### Running the ASP.NET Server

## Interacting with the web server

### Creating a part

**NOTE:** Both the server and database must be running in order to run any commands in this section.

curl -v -X POST localhost:5000/api/part -H "Content-Type: application/json" -d '{"Name": "Cap"}'

## Future Improvements

### 1. Authentication

### 2. Unit Tests

### 3. Logging

### 4. Optimize Db Schema for most typical use cases
