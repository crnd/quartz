# Quartz

Quartz is a simple serverless API for duration measurement built with HTTP triggered .NET Core Azure Functions and Azure Table storage.

## Usage

There are three endpoints to use:

* ```POST /api/start```
Starts a new measurement and returns the ID of the measurement that was created.
* ```POST /api/{ID}/stop```
Stops the specified measurement and returns the duration in seconds. Returns HTTP error code 400 if the specified measurement is not running. Returns HTTP error code 404 if no measurement can be found with the given ID.
* ```GET /api/{ID}```
Returns the duration of the measurement, start timestamp in ISO 8601 format and if the measurement is running or not. If the specified measurement can not be found with the given ID then returns HTTP error code 404.

### Running locally

Quartz can be run locally by using Azure Functions CLI and Azure Storage Emulator. The emulator default connection string is defined in ```local.settings.json```.

## License

This project is licensed under the terms of the MIT license.
