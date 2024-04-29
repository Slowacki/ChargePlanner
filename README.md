# ChargePlanner

## Description
The solution allows to generate a plan how a car battery should be charged, taking into account the desired
charge levels, but also the energy prices during specific times of the day.

It dividies the time we have to charge the battery into periods by the available tariffs. Subsequently it applies
charge time to those periods based on the business requirements. The result is mapped for the comfort of the API user
and to comply with the response requirements.

## Prerequisites
- .NET 8
- WSL 2 for Windows (https://learn.microsoft.com/en-us/windows/wsl/install)
- Docker Desktop (https://www.docker.com/products/docker-desktop/)

## How to run
- Clone the repository from github
- Make sure port 8080 is available
- Run `docker compose up` in the main solution folder
- Navigate to `http://localhost:8080/swagger/index.html`

## Notes
- Way more tests could be added. The ones present are meant to just demonstrate how the test coverage could look like.
- The API expects the tariffs to start at 00:00:00 and end at 23:59:59 and validates if they cover the whole day with specific accuracy.
  I don't feel like it's a good approach however. It adds a lot of unnecessary complication for the users' of the API. Don't want to spend too
  much time on the project, but if I were to, I'd rewrite the endpoint to only accept tariff start times, the tariff periods should be calculated
  in the backend then.
- Swagger is enabled for non-development environments on purpose to make testing the solution easier after running the solution with Docker

## Sample JSON to test out the API
```
{
  "batterySettings": {
    "chargePower": 10,
    "capacity": 100,
    "currentLevel": 0
  },
  "chargeSettings": {
    "desiredChargePercentage": 80,
    "startTime": "2034-04-28T22:30:00Z",
    "endTime": "2034-04-29T08:30:00Z",
    "directChargePercentage": 20,
    "tariffs": [
      {
        "startTime": "00:00:00",
        "endTime": "07:29:59",
        "pricePerKwh": 1
      },
      {
        "startTime": "7:30:00",
        "endTime": "23:59:59",
        "pricePerKwh": 2
      }
    ]
  }
}
```

