# Sila Technologies Product Server

Demo for Sila by Montana Gau done using ASP.NET and MongoDb. Note: I have opted to separate the parts from the assemblies completely as that organization made more sense to me. There is no adding "parts" to each other as requested. Instead, an assembly must be created first and then parts can be added to that assembly. This seemed a more logical structure in my opinion. Changing this however, would be reasonably easy by automatically creating a parent assembly when adding parts together. Additionally, a real database has been added and state is preserved throughout restarts. Much of the code used here (particularly around DB interaction) is borrowed from my website, https://yabberon.com (still WIP please ignore the embarrassing test content) which is already running ASP.NET in the backend.

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

First, ensure that you have [.Net 5](https://dotnet.microsoft.com/download/dotnet/5.0) installed. Then, from the command line, run `dotnet run` from the root of the repo. This will run the server in _development mode_ which should be sufficient for our purposes. I have excluded the `appsettings.Development.json` file from the `.gitignore` to make for easier configuration.

## Interacting with the web server

**NOTE:** Both the server and database must be running in order to run any commands in this section.

### Creating a part

```bash
curl -v -X POST localhost:5000/api/part -H "Content-Type: application/json" -d '
{
    "Name": "Barrel Bottom"
}'
```

Additionally, we can optionally add parameters for `Material` and `Color` shown as follows:

```bash
curl -v -X POST localhost:5000/api/part -H "Content-Type: application/json" -d '
{
    "Name": "Barrel Bottom",
    "Color": "Red",
    "Material": "Plastic"
}'
```

### Getting a part

```bash
curl -v -X GET localhost:5000/api/part/<PART_ID>
```

Get all parts by running

```bash
curl -v -X GET localhost:5000/api/part
```

### Creating an assembly

```bash
curl -v -X POST localhost:5000/api/assembly -H "Content-Type: application/json" -d '
{
    "Name": "Barrel Assembly"
}'
```

### Getting an assembly

```bash
curl -v -X GET localhost:5000/api/assembly/<ASSEMBLY_ID>
```

Get all assemblies by running

```bash
curl -v -X GET localhost:5000/api/assembly
```

### Add an item to an assembly

Add a **part** to the assembly by running:

```bash
curl -v -X PUT localhost:5000/api/part/<ID_OF_CHILD_PART> -H "Content-Type: application/json" -d '
{
    "ParentAssemblyId": "<ID_OF_PARENT_ASSEMBLY>"
}'
```

Add an **assembly** to another assembly by running:

```bash
curl -v -X PUT localhost:5000/api/assembly/<ID_OF_CHILD_ASSEMBLY> -H "Content-Type: application/json" -d '
{
    "ParentAssemblyId": "<ID_OF_PARENT_ASSEMBLY>"
}'
```

Remove an item from an assembly by running either of the above, but with the value of `ParentAssemblyId` set to null.

### Delete a part or assembly

Delete a part by running:

```bash
curl -v -X DELETE localhost:5000/api/part/<ID_OF_ASSEMBLY>
```

Delete an assembly by running:

```bash
curl -v -X DELETE localhost:5000/api/assembly/<ID_OF_ASSEMBLY>
```

### List all top-level assemblies

```bash
curl -v -X GET localhost:5000/api/assembly?topLevelOnly=true
```

### Pen Demo

All 4 examples follow the same process:

1. Create the basic component parts
2. Organize these component parts into sub-assemblies
3. Combine the sub-assemblies to form the top-level assembly

Create a red metal pen:

```bash

```

Create a blue metal pen:

```bash

```

Create a red plastic pen:

```bash

```

Create a blue plastic pen:

```bash

```

## Future Improvements

### 1. Authentication

### 2. Unit Tests

### 3. Logging

### 4. Optimize Db Schema for most typical use cases
