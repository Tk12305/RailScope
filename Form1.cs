using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Text; // Ensure this is included
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RailScope
{
    public partial class Form1 : Form
    {

       
        private readonly RailwayDataService _railwayDataService = new();

        public Form1()
        {
            InitializeComponent();
        }

        // --- Station Search (Now uses the specialized structured wrapper) ---
        private async void btnSearchStations_Click(object? sender, EventArgs e)
        {
            // --- Input Validation Check ---
            string stationQuery = txtStation.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(stationQuery))
            {
                txtResults.Text = "Please enter a station name or CRS code to search.";
                return; // Exit the method immediately if the input is empty
            }

            // NEW: Disable input while searching is running
            txtStation.Enabled = false;
            // --- End Input Validation Check ---

            // Use the new wrapper and pass a lambda function (Action) that defines what to do with the results.
            await RunStationSearchAsync(stations =>
            {
                // Clear any previous station selection when a new search starts
                cbStations.SelectedIndex = -1;

                if (stations != null && stations.Count > 0)
                {
                    // Use a ListAdapter to display Name and store CRS code
                    var stationList = new List<StationInfo>(stations);
                    cbStations.DataSource = stationList;

                    // Set the DisplayMember (what the user sees) and ValueMember (what the code uses)
                    cbStations.DisplayMember = "Name";
                    cbStations.ValueMember = "Crs";
                }
                else
                {
                    // If no stations are found, clear the ComboBox
                    cbStations.Items.Clear();
                    txtResults.Text = "No station matches were returned. Try a different name or CRS code.";
                }
            });
        }
        private void ColorCodeServiceStatus()
        {
            // Ensure the grid has data before trying to process rows
            if (dvgDepartures.DataSource is not IList departures)
            {
                return;
            }

            // Iterate over every row in the DataGridView
            for (int i = 0; i < dvgDepartures.Rows.Count; i++)
            {
                // Skip the header row, if it exists
                if (dvgDepartures.Rows[i].IsNewRow) continue;

                // The 'Status' column is the last column we created (index 4, if zero-based)
                DataGridViewCell statusCell = dvgDepartures.Rows[i].Cells["Status"];
                string statusText = statusCell.Value?.ToString() ?? string.Empty;

                // Check the status text and set the foreground color (text color)
                switch (statusText.ToLower())
                {
                    case "cancelled":
                        // Urgent: Red text for cancelled services
                        statusCell.Style.ForeColor = System.Drawing.Color.Red;
                        break;
                    case "no live estimate":
                    case "delayed — no estimate":
                        // Warning: Orange/Yellow text for uncertain/delayed services
                        statusCell.Style.ForeColor = System.Drawing.Color.OrangeRed;
                        break;
                    default:
                        // Default status (Expected, Actual, etc.) stays black
                        statusCell.Style.ForeColor = System.Drawing.Color.Black;
                        break;
                }
            }
        }


        private async Task LoadDeparturesAsync()
        {
            // State managment (Loading start)
            btnSearchStations.Enabled = false;
            btnRefreshDepartures.Enabled = false;
            lblStatus.Text = "Contacting Trainiac...";
            txtResults.Text = string.Empty;

            DepartureResponse response = null;

            try
            {
                // --- START: Safety Check ---
                if (cbStations.SelectedValue == null)
                {
                    txtResults.Text = "Error: Please select a station before refreshing departures.";
                    return; // Exit the method immediately if no station is selected
                }
                // --- END: Safety Check ---

                // 1. Call the service and receive the structured container object
                response = await _railwayDataService.GetDeparturesAsync(cbStations.SelectedValue.ToString(), 20);

                // 2. Process and display the structured data
                if (response != null && response.Departures != null && response.Departures.Count > 0)
                {
                    // DataGridView population code: THIS IS WHERE THE DATA APPEARS
                    dvgDepartures.DataSource = response.Departures;
                    dvgDepartures.AutoGenerateColumns = false;
                    dvgDepartures.AllowUserToAddRows = false;

                    // Clear existing columns before defining new ones
                    dvgDepartures.Columns.Clear();

                    // 1. Define the Destination column
                    DataGridViewTextBoxColumn destinationCol = new DataGridViewTextBoxColumn();
                    destinationCol.Name = "Destination";
                    destinationCol.HeaderText = "Destination";
                    destinationCol.DataPropertyName = "Destination"; // Maps to DepartureItem.Destination
                    dvgDepartures.Columns.Add(destinationCol);
                    destinationCol.Width = 180;

                    // 2. Define the platformcolumn
                    DataGridViewTextBoxColumn platformCol = new DataGridViewTextBoxColumn();
                    platformCol.Name = "Platform";
                    platformCol.HeaderText = "Platform";
                    platformCol.DataPropertyName = "Platform"; // Maps to DepartureItem.Platform
                    dvgDepartures.Columns.Add(platformCol);

                    // 3. Define the Scheduled Time column
                    DataGridViewTextBoxColumn scheduledTimeCol = new DataGridViewTextBoxColumn();
                    scheduledTimeCol.Name = "ScheduledTime";
                    scheduledTimeCol.HeaderText = "Scheduled";
                    scheduledTimeCol.DataPropertyName = "ScheduledTime"; // Maps to DepartureItem.ScheduledTime
                    dvgDepartures.Columns.Add(scheduledTimeCol);

                    // 4. Define the train Headcode column
                    DataGridViewTextBoxColumn headcodeCol = new DataGridViewTextBoxColumn();
                    headcodeCol.Name = "TrainHeadcode";
                    headcodeCol.HeaderText = "Train";
                    headcodeCol.DataPropertyName = "TrainHeadcode"; // Maps to DepartureItem.TrainHeadcode
                    dvgDepartures.Columns.Add(headcodeCol);
                    headcodeCol.Width = 80;

                    // 5. Define the TrainOperator column
                    DataGridViewTextBoxColumn operatorCol = new DataGridViewTextBoxColumn();
                    operatorCol.Name = "TrainOperator";
                    operatorCol.HeaderText = "Operator";
                    operatorCol.DataPropertyName = "TrainOperator"; // Maps to DepartureItem.TrainOperator
                    dvgDepartures.Columns.Add(operatorCol);
                    operatorCol.Width = 180;

                    // 6. Define the Status column
                    DataGridViewTextBoxColumn statusCol = new DataGridViewTextBoxColumn();
                    statusCol.Name = "Status";
                    statusCol.HeaderText = "Status";
                    statusCol.DataPropertyName = "Status"; // Maps to DepartureItem.Status
                    dvgDepartures.Columns.Add(statusCol);
                    ColorCodeServiceStatus();

                    // 7. Define the Estimated Time column
                    DataGridViewTextBoxColumn estimatedTimeCol = new DataGridViewTextBoxColumn();
                    estimatedTimeCol.Name = "EstimatedTime";
                    estimatedTimeCol.HeaderText = "Expected";
                    estimatedTimeCol.DataPropertyName = "EstimatedTime"; // Maps to DepartureItem.EstimatedTime
                    dvgDepartures.Columns.Add(estimatedTimeCol);

                    // --- START: Apply Row Striping for Readability ---
                    if (dvgDepartures.DataSource is IList departures)
                    {
                        for (int i = 0; i < dvgDepartures.Rows.Count; i++)
                        {
                            // We check if the row index (i) is odd.
                            // The modulo operator (%) gives the reminder after division.
                            // If i divided by 2 has a remainder of 1, the row is odd (1, 3, 5, etc.)
                            if (i % 2 != 0)
                            {
                                dvgDepartures.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                            }
                        }
                    }

                    // Timestamp display
                    lblLastUpdated.Text = $"Data retreived at: {response.GeneratedAt}";
                }
                else
                {
                    // NEW: Display a dedicated message in the DataGridView if no data is found
                    dvgDepartures.DataSource = null; // clear previous data
                    dvgDepartures.DataSource = new List<object> { "--- No departures found for this time slot ---" };
                    dvgDepartures.AutoGenerateColumns = true;
                    dvgDepartures.AllowUserToAddRows = false;
                    txtResults.Text = "No departures were returned for this station/time.";
                }
            }
            catch (HttpRequestException ex)
            {
                txtResults.Text = "NETWORK ERROR: Could not reach the API. Please check your internet connection.";
                // Optionally show the technical error to the developer/user for debugging
                // Console.WriteLine(ex.Message); 
            }
            catch (Exception ex)
            {
                // Catch all other exceptions (API errors, parsing errors, etc.)
                txtResults.Text = $"APPLICATION ERROR: {ex.Message}";
            }
            finally
            {
                // Remaining UI restoration logic
                txtStation.Enabled = true;
                btnSearchStations.Enabled = true;
                btnRefreshDepartures.Enabled = true;
                lblStatus.Text = string.Empty;
            }
        }


        // --- Departure Search (Handles structured data and loading state manually) ---
        private async void btnRefreshDepartures_Click(object? sender, EventArgs e)
        {
            await LoadDeparturesAsync();
        }

        // --- Helper Method for Station Search ---
        private async Task RunStationSearchAsync(Action<List<StationInfo>> resultsHandler)
        {
            // Declaration MUST be at the very top of the method
            string oldSearchText = btnSearchStations.Text;

            btnSearchStations.Enabled = false;
            btnRefreshDepartures.Enabled = false;
            btnSearchStations.Text = "Searching...";
            txtResults.Text = "Contacting Trainiac…";
            txtStation.Enabled = false;

            try
            {
                // FIX: Call the service and capture the actual list of stations
                List<StationInfo> stations = await _railwayDataService.SearchStationsAsync(txtStation.Text);

                // FIX: Pass the captured list to the handler (the lambda function)
                resultsHandler(stations);
            }
            catch (Exception ex)
            {
                // Handle search errors
                txtResults.Text = $"Unexpected search error:\r\n{ex.Message}";
            }
            finally
            {
                // Restore UI state, using the variable declared at the start
                txtStation.Enabled = true;
                btnSearchStations.Text = oldSearchText;
                btnSearchStations.Enabled = true;
                btnRefreshDepartures.Enabled = true;
            }
        }

        private async void cbStations_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // Check if a station object was actually selected
            if (cbStations.SelectedItem is StationInfo selectedStation)
            {
                // 1. Update the visual display of the current station
                lblCurrentStationInfo.Text = $"Current Station: {selectedStation.Name} (CRS: {selectedStation.Crs})";
            }
            else
            {
                // 2. clear the visual display if nothing is selected
                lblCurrentStationInfo.Text = "Current Station: Not Selected";
            }

            await LoadDeparturesAsync();
        }

    }
}
