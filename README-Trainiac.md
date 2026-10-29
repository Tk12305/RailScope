# RailScope — Trainiac API update

This version replaces the previous incorrect National Rail/Realtime Trains-style request with Trainiac's current v1 REST API.

## What's included
- **Search Stations** searches `GET https://api.traini.ac/v1/stations?q=...`
- **Live Departures** requests `GET https://api.traini.ac/v1/departures/{station}?limit=10`
- Station name or CRS code input (pre-filled with `PAD`)
- JSON parsing, empty results, HTTP/API errors, timeout handling, and no API key or bearer token

## Open in Visual Studio
1. Extract the ZIP.
2. Open `RailScope.slnx` (or `RailScope.csproj`) in Visual Studio.
3. Build the project, then run it.
4. Leave `PAD` in the station box, click **Search Stations**, then click **Live Departures**.
5. Try `OXF`, `RDG`, or a station name such as `London Paddington`.

The Trainiac API's current documented endpoint is under `/v1`. Older `/api` examples are legacy. API usage is for personal, non-commercial projects; check the provider's current terms before publishing or using it commercially.

## Test the exact GET request before running the app

### In a browser
Open:
https://api.traini.ac/v1/departures/PAD?limit=5

You should see JSON containing `type`, `request`, `data`, and `generated_at`. If `data` is empty, the request may still be valid; it means no departures were returned for that query/time.

Station search:
https://api.traini.ac/v1/stations?q=Paddington

### In PowerShell
Run this in PowerShell (not in the C# editor):

```powershell
$response = Invoke-RestMethod -Uri "https://api.traini.ac/v1/departures/PAD?limit=5" -Method Get
$response | ConvertTo-Json -Depth 10
```

To print a compact departure list:

```powershell
$response.data | ForEach-Object {
    [PSCustomObject]@{
        Scheduled  = $_.departs.scheduled
        Train      = $_.train.headcode
        Destination = $_.destination.name
        Platform   = $_.platform.number
        Estimate   = $_.departs.estimate.type
        ExpectedAt = $_.departs.estimate.at
    }
} | Format-Table -AutoSize
```

For station lookup:

```powershell
Invoke-RestMethod -Uri "https://api.traini.ac/v1/stations?q=Paddington" |
    ConvertTo-Json -Depth 10
```

If PowerShell returns an error, check the URL and internet connection. HTTP 400 usually means an invalid/ambiguous parameter; HTTP 429 means the fair-use rate limit was reached, so wait before retrying.

## Security note
The original `Form1.cs` included a hard-coded bearer token for a different API. It has been removed from this version. If it was a real credential, revoke/rotate it with its provider. Never paste API credentials into source code or public repositories.


## Fix for "No station matches were returned"
Test these endpoints directly:
- https://api.traini.ac/v1/stations?q=PAD
- https://api.traini.ac/v1/stations?q=Paddington

The documented response uses `matches` and `resolves_to`. This update checks for Trainiac's documented `{ "type": "error", "error": { ... } }` response and shows its message instead of incorrectly reporting no matches. Rebuild the solution after opening the updated project.


## Editing the interface in Visual Studio

The form's visual controls and layout are now defined in `Form1.Designer.cs`, with request/event logic in `Form1.cs`. Open `Form1.cs` in Solution Explorer and choose **View Designer** (or right-click the file and choose **View Designer**). Select a control to edit its properties, drag controls to reposition them, or use the Toolbox to add controls. Double-click a button in Designer to create its click handler.

Do not manually add controls in the `Form1` constructor; visual layout changes belong in the Windows Forms Designer. The Favourites button is currently a visual placeholder and is not wired to a feature yet.
