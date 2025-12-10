## StudHub Data Server

## Spinup Database

1. Download [docker](https://www.docker.com/products/docker-desktop/)
2. Run command `docker compose up -d` to run postgres in background.
3. Connect to database with tool at your choice, e.g. DataGrip. Database is running at `localhost:4444`, with postgres
   as both username and password. Database name is studhub_db.
---
> [!Note]  
> If you have an existed db that has outdated tables, run `docker compose down --volumes` to
> remove existing db first.
---
> [!Note]  
> The API keys stored in the database are protected using AES encryption.
>
> To enable decryption, you need the **Secret Key** used during encryption. Store this key in your IntelliJ Run Configuration as an environment variable:
>
> **`APP_SECRET_KEY=YOUR_16_CHAR_SECRET_KEY`**
>
> *Replace* `YOUR_16_CHAR_SECRET_KEY` with the actual 16-character secret key value.
---
### Local HTTPS Setup (For Blazor Client)

If you are running the Blazor Client project and the API Server locally using HTTPS (e.g., in Development mode), you must trust the .NET Core development certificate to prevent SSL connection errors:

1. **Trust Certificate:** Open a terminal as an **Administrator** and run the following command once:
    ```bash
    dotnet dev-certs https --trust
    ```
2. **Restart:** Restart all running ASP.NET Core projects after running the trust command.

---

### Seed csv file

1.  **Set API Key:** Get your Rebrickable API key from [https://rebrickable.com/api/](https://rebrickable.com/api/) and set it in your terminal session before running the script:
    ```bash
    $env:REBRICKABLE_API_KEY='YOUR_ACTUAL_REBRICKABLE_API_KEY_HERE'
    ```

2.  **Generate Data:** Run the script to fetch data and generate the initial CSV file (requires the `requests` and `pandas` Python packages).
    ```bash
    python scripts/create-mapping.py
    ```

3.  **Clean Data:** Remove duplicate records from the generated CSV file.
    ```bash
    python scripts/remove-duplicates.py
    ```

4.  **Import to Database:** Copy the clean CSV data into the `parts_map` table in the running `studhub_db` container.
    ```bash
    docker exec -it studhub_db psql -U postgres -d studhub_db -c "\COPY parts_map FROM './data/parts_mapping_clean.csv' CSV HEADER"
    ```
---
