module Data.ModelPredictions

open FsToolkit.ErrorHandling

let apiUrl = "http://127.0.0.1:8000/api/modeling/models/"

// https://api-models.sledilnik.org/api/models

type Contact = {
    Name : string
    Email : string
}

type Model = {
    Id : string
    Name : string
    Www : string
    Description : string option
    Contacts : Contact list
}

type Scenario = {
    Id : string
    Slug : string
    Name : string
    Description : string
}

type IntervalKind = {
    Id : string
    Slug : string
    Name : string
    Description : string
}

type Prediction = {
    Date : System.DateTime
    Model : Model
    Scenario : Scenario
    IntervalKind : IntervalKind
}

type PredictionsMetadata = {
    Models : Model list
    Scenarios : Scenario list
    IntervalKinds : IntervalKind list
    Predictions : Prediction list
}

// type PredictionDataPoint =
//     { Date : System.DateTime
//       Icu : int
//       IcuLowerBound : int
//       IcuUpperBound : int
//       Hospitalized : int
//       HospitalizedLowerBound : int
//       HospitalizedUpperBound : int
//       Deceased : int
//       DeceasedLowerBound : int
//       DeceasedUpperBound : int
//       DeceasedToDate : int
//       DeceasedToDateLowerBound : int
//       DeceasedToDateUpperBound : int }


module Transfer =

    type Prediction = {
        Date : System.DateTime
        ModelId : string
        ScenarioId : string
        IntervalKindId : string
    }

    type PredictionsMetadata = {
        Models : Model list
        Scenarios : Scenario list
        IntervalKinds : IntervalKind list
        Predictions : Prediction list
    }

let loadPredictionsMetadata () =
    asyncResult {
        let! data = DataLoader.makeDataLoader<Transfer.PredictionsMetadata> apiUrl ()

        try
            let predictions =
                data.Predictions
                |> List.map (fun prediction ->
                    { Date = prediction.Date
                      Model = data.Models |> List.find (fun model -> model.Id = prediction.ModelId)
                      Scenario = data.Scenarios |> List.find (fun scenario -> scenario.Id = prediction.ScenarioId)
                      IntervalKind = data.IntervalKinds |> List.find (fun intervalKind -> intervalKind.Id = prediction.IntervalKindId)
                    }
                )

            return {
                Models = data.Models
                Scenarios = data.Scenarios
                IntervalKinds = data.IntervalKinds
                Predictions = predictions
            }
        with
            | _ as ex ->
                return! Error (sprintf "Invalid model prediction metadata: %A" ex)
    }
