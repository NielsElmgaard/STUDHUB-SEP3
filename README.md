## StudHub Data Server

## Spinup Database (local database)
> [!Note]  
> By default, the database runs on Amazon RDS (cloud database). If you prefer not to use RDS, follow the guide to spin up a local database
1. Download [docker](https://www.docker.com/products/docker-desktop/)
2. Run command `docker compose up -d` to run postgres in background.
3. Connect to database with tool at your choice, e.g. DataGrip. Database is
   running at `localhost:4444`, with postgres
   as both username and password. Database name is studhub_db.

> [!Note]  
> If you have an existed db that has outdated tables, run
`docker compose down --volumes` to
> remove existing db first.
---


### Setting Environment Variables in IntelliJ

#### For Running the `DataServerApplication`:
For the encryption and database connection you need to set the Environment Variables.
You can either:
- Manually set the variables from the provided .env file (see the instructions for [Running Integration Tests](#for-running-integration-tests)
  below), or
- Use the EnvFile plugin.
[!IMPORTANT]

**NOTE**: If you are using the local database (Docker), ensure your DB_URL is set to `jdbc:postgresql://localhost:4444/studhub_db`
###### Using EnvFile-plugin:
1. Install the EnvFile plugin via IntelliJ Settings → Plugins → Marketplace → EnvFile.
2. Open Run → Edit Configurations….
3. Select your DataServerApplication run configuration.
4. Enable EnvFile in the configuration.
5. Click + and choose the .env file from your project.
   6. You need to place the .env inside the `data-server` folder (the same place where e.g. `pom.xml` is located)
6. Apply the changes.
All environment variables from .env will now be available when running the application.
#### For Running Integration Tests
If you run integration tests (e.g., `StudServiceIntegrationTest`), you need to set the test APP_SECRET_KEY as an environment variable:
1. Open **Run/Debug Configurations**.
2. Click **Modify options...**.
3. Select **Environment variables **.
4. Add `APP_SECRET_KEY=1234567890123456` in the **Environment variables** field.
5. Click **Apply**.
   *Now all new tests you run will automatically have the secret key available.*
---
### Local HTTPS Setup (For Blazor Client)

By default, The Blazor Client is set to use HTTPS, therefore you must trust the .NET Core development
certificate to prevent SSL connection errors:

1. **Trust Certificate:** Open a terminal as an **Administrator** and run the
   following command once:
    ```bash
    dotnet dev-certs https --trust
    ```
2. **Restart:** Restart all running ASP.NET Core projects after running the
   trust command.

#### Switching from HTTPS to HTTP
To switch from HTTPS to HTTP you need to change the code inside `Client/Program.cs` in Rider to this:
```csharp
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5299/"), // Use HTTP
    Timeout = TimeSpan.FromMinutes(15)
});
```
---

### Seed csv file (not necessary if using the Amazon RDS)

1. **Set API Key:** Get your Rebrickable API key
   from [https://rebrickable.com/api/](https://rebrickable.com/api/) and set it
   in your terminal session before running the script:
   ```bash
   $env:REBRICKABLE_API_KEY='YOUR_ACTUAL_REBRICKABLE_API_KEY_HERE'
   ```

2. **Generate Data:** Run the script to fetch data and generate the initial CSV
   file (requires the `requests` and `pandas` Python packages).
   ```bash
   python scripts/create-mapping.py
   ```

3. **Clean Data:** Remove duplicate records from the generated CSV file.
   ```bash
   python scripts/remove-duplicates.py
   ```

4. **Import to Database:** Copy the clean CSV data into the `parts_map` table in
   the running `studhub_db` container.
   ```bash
   docker exec -it studhub_db psql -U postgres -d studhub_db -c "\COPY parts_map FROM './data/parts_mapping_clean.csv' CSV HEADER"
   ```

---
