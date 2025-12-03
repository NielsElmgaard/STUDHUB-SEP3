## StudHub Data Server

## Spinup Database
1. Download [docker](https://www.docker.com/products/docker-desktop/)
2. Run command `docker compose up -d` to run postgres in background.
3. Connect to database with tool at your choice, e.g. DataGrip. Database is running at `localhost:4444`, with postgres 
as both username and password. Database name is studhub_db.

> [!Note]  
> If you have an existed db that has outdated tables, run `docker compose down --volumes` to
> remove existing db first.

## Run project

```bash
mvn clean install
```
In maven tool, reload maven project if error shows for `pom.xml`.

```bash
mvn clean compile
```