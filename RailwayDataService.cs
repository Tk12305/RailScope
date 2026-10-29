using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RailScope
{
    /// <summary>
    /// Small client for Trainiac's public v1 API.
    /// The API is read-only and does not require an API key.
    /// </summary>
    public class DepartureResponse
    {
        public List<DepartureItem> Departures { get; set; }
        public string GeneratedAt { get; set; }
    }
    public class StationInfo
    {
        public string Name { get; set; }
        public string Crs { get; set; }
    }
    public sealed class RailwayDataService
    {
        private static readonly HttpClient Http = CreateHttpClient();
        private const string BaseUrl = "https://api.traini.ac/v1";

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(20)
            };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("RailScope/1.0");
            return client;
        }

        public async Task<List<StationInfo>> SearchStationsAsync(string stationQuery)
        {
            if (string.IsNullOrWhiteSpace(stationQuery))
                return new List<StationInfo>();

            string url = $"{BaseUrl}/stations?q={Uri.EscapeDataString(stationQuery.Trim())}";
            using JsonDocument doc = await GetJsonAsync(url);

            JsonElement root = doc.RootElement;
            ThrowIfApiError(root);

            JsonElement searchData = GetProperty(root, "data");
            if (searchData.ValueKind != JsonValueKind.Object)
                searchData = root;

            var results = new List<StationInfo>();

            // 1. Check if the API response is a JSON ARRAY (Our Resilience Fallback)
            if (root.ValueKind == JsonValueKind.Array)
            {
                // If it's an array, treat every item as a potential station match
                foreach (JsonElement station in root.EnumerateArray())
                {
                    string name = GetString(station, "name");
                    string crs = GetString(station, "crs");

                    // We only care about Name and CRS for the dropdown
                    if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(crs))
                    {
                        results.Add(new StationInfo { Name = name, Crs = crs });
                    }
                }
                // Return immediately if we successfully processed the array structure
                return results;
            }

            // --- 2. Handle the standard JSON OBJECT response (The original expected format) ---

            // Handle the "Best Match" case
            if (searchData.TryGetProperty("resolves_to", out JsonElement resolvesTo) &&
                resolvesTo.ValueKind == JsonValueKind.Object)
            {
                string resolvedName = GetString(resolvesTo, "name");
                string resolvedCrs = GetString(resolvesTo, "crs");

                if (!string.IsNullOrWhiteSpace(resolvedName))
                {
                    results.Add(new StationInfo { Name = resolvedName, Crs = resolvedCrs });
                }
            }

            // Handle the "Matches" array case
            if (searchData.TryGetProperty("matches", out JsonElement matchesElement) &&
               matchesElement.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement station in matchesElement.EnumerateArray())
                {
                    string name = GetString(station, "name");
                    string crs = GetString(station, "crs");

                    if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(crs))
                    {
                        results.Add(new StationInfo { Name = name, Crs = crs });
                    }
                }
            }

            return results;
        }
        // Update GetDeparturesAsync in RailwayDataService.cs
        public async Task<DepartureResponse> GetDeparturesAsync(string station, int limit = 15)
        {
            if (string.IsNullOrWhiteSpace(station))
                throw new ArgumentException("Station query cannot be empty.");

            limit = Math.Clamp(limit, 1, 100);
            string url = $"{BaseUrl}/departures/{Uri.EscapeDataString(station.Trim())}?limit={limit}";

            using JsonDocument doc = await GetJsonAsync(url);
            JsonElement root = doc.RootElement;
            ThrowIfApiError(root);

            var response = new DepartureResponse();
            response.GeneratedAt = GetString(root, "generated_at");
            List<DepartureItem> departures = new List<DepartureItem>();

            string resolvedName = "";
            string resolvedCrs = "";
            if (root.TryGetProperty("request", out JsonElement request) &&
                request.TryGetProperty("station", out JsonElement stationRequest) &&
                stationRequest.TryGetProperty("resolution", out JsonElement resolution))
            {
                resolvedName = GetString(resolution, "name");
                resolvedCrs = GetString(resolution, "crs");
            }

            if (!root.TryGetProperty("data", out JsonElement data) ||
                data.ValueKind != JsonValueKind.Array || data.GetArrayLength() == 0)
            {
                response.Departures = departures;
                return response;
            }

            foreach (JsonElement departure in data.EnumerateArray())
            {
                JsonElement train = GetProperty(departure, "train");
                JsonElement destination = GetProperty(departure, "destination");
                JsonElement departs = GetProperty(departure, "departs");
                JsonElement estimate = GetProperty(departs, "estimate");
                JsonElement platform = GetProperty(departure, "platform");
                JsonElement operatorElement = GetProperty(train, "operator");
                string trainOperatorName = GetString(operatorElement, "name");

                // *** STEP 1: Extract the raw estimated time ***
                string estimatedTimeValue = string.Empty;
                // Check if the 'estimate' element has an 'at' property (the time)
                if (estimate.TryGetProperty("at", out JsonElement atElement) && atElement.ValueKind == JsonValueKind.String)
                {
                    // If it has a time, we format it now and store it separately
                    estimatedTimeValue = FormatTime(atElement.GetString());
                }

                // --- MODIFIED BLOCK: Data Cleaning/Normalization ---
                // Helper function to ensure we don't display empty strings
                string SanitizeValue(string value)
                {
                    return string.IsNullOrWhiteSpace(value) ? "N/A" : value;
                }

                var item = new DepartureItem
                {
                    // Clean up Destination: Use SanitizeValue on the retrieved name
                    Destination = SanitizeValue(GetString(destination, "name")),

                    // Platform already has a formatting function, but we wrap it for safety
                    Platform = SanitizeValue(FormatPlatform(platform)),

                    // Clean up ScheduledTime
                    ScheduledTime = SanitizeValue(FormatTime(GetString(departs, "scheduled"))),

                    // New Column to display train operator
                    TrainOperator = trainOperatorName,
                    
                    // Clean up Train Headcode
                    TrainHeadcode = SanitizeValue(GetString(train, "headcode")),

                    // Status formatting handles its own missing info, but we ensure it's safe
                    Status = SanitizeValue(FormatEstimate(estimate)),
                    
                    EstimatedTime = estimatedTimeValue
                };
                departures.Add(item);
            }

            response.Departures = departures;
            return response;
        }

        private static async Task<JsonDocument> GetJsonAsync(string url)
        {
            try
            {
                using HttpResponseMessage response = await Http.GetAsync(url);
                string body = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"--- RAW API RESPONSE for {url} ---\n{body}\n------------------------------------");

                JsonDocument document;
                try
                {
                    document = JsonDocument.Parse(body);
                }
                catch (JsonException)
                {
                    throw new InvalidOperationException(
                        $"Trainiac returned an unexpected response (HTTP {(int)response.StatusCode}).");
                }

                if (!response.IsSuccessStatusCode)
                {
                    string message = $"HTTP {(int)response.StatusCode} ({response.ReasonPhrase})";
                    document.Dispose();
                    throw new HttpRequestException(message);
                }

                return document;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"!!! CRITICAL NETWORK ERROR during API call to {url} !!!");
                Console.WriteLine($"Error Message: {ex.Message}");
                throw;
            }
        }

        private static void ThrowIfApiError(JsonElement root)
        {
            if (root.ValueKind == JsonValueKind.Object &&
                root.TryGetProperty("type", out JsonElement type) &&
                type.ValueKind == JsonValueKind.String &&
                type.GetString() == "error")
            {
                string message = "Trainiac returned an API error.";
                if (root.TryGetProperty("error", out JsonElement error) &&
                    error.ValueKind == JsonValueKind.Object)
                {
                    string detail = GetString(error, "message");
                    string errorType = GetString(error, "type");
                    if (!string.IsNullOrWhiteSpace(detail))
                        message = detail;
                    else if (!string.IsNullOrWhiteSpace(errorType))
                        message = $"Trainiac API error: {errorType}";
                }
                throw new InvalidOperationException(message);
            }
        }

        private static JsonElement GetProperty(JsonElement element, string propertyName)
        {
            return element.ValueKind == JsonValueKind.Object &&
                   element.TryGetProperty(propertyName, out JsonElement value)
                ? value
                : default;
        }

        private static string GetString(JsonElement element, string propertyName)
        {
            JsonElement value = GetProperty(element, propertyName);
            return value.ValueKind == JsonValueKind.String ? value.GetString() ?? "" : "";
        }

        private static string GetNullableNumber(JsonElement element, string propertyName)
        {
            JsonElement value = GetProperty(element, propertyName);
            if (value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out double number))
                return number.ToString("0.#", CultureInfo.InvariantCulture);
            return "";
        }

        private static string FormatTime(string isoTime)
        {
            if (DateTimeOffset.TryParse(isoTime, CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind, out DateTimeOffset parsed))
                return parsed.ToLocalTime().ToString("HH:mm", CultureInfo.CurrentCulture);
            return string.IsNullOrWhiteSpace(isoTime) ? "Time unknown" : isoTime;
        }

        private static string FormatEstimate(JsonElement estimate)
        {
            string type = GetString(estimate, "type");
            string at = GetString(estimate, "at");
            return type switch
            {
                "actual" => $"Departed {FormatTime(at)}",
                "forecast" => $"Expected {FormatTime(at)}",
                "delayed_no_estimate" => "Delayed — no estimate",
                "cancelled" => "Cancelled",
                "no_information" => "No live estimate",
                _ => "Status unknown"
            };
        }

        private static string FormatPlatform(JsonElement platform)
        {
            string type = GetString(platform, "type");
            return type switch
            {
                "known" => GetString(platform, "number"),
                "withheld" => "Not yet displayed",
                "unknown" => "Unknown",
                _ => "Unknown"
            };
        }
    }
}

