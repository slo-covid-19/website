module ModelPredictionsChart.Types

open Types

let translate = I18N.chartText "modelPredictions"

type Metric =
    | Icu
    | Hospitalized
    | Deceased
    | DeceasedToDate

    static member all = [Icu ; Hospitalized ; Deceased ; DeceasedToDate]

    member this.name =
        match this with
        | Icu -> translate "metrics.icu"
        | Hospitalized -> translate "metrics.hospitalized"
        | Deceased -> translate "metrics.deceased"
        | DeceasedToDate -> translate "metrics.deceasedToDate"

type Model = {
    Id : System.Guid
    Name : string
}

type Scenario =
    Scenario of string

type IntervalKind =
    IntervalKind of string

type Prediction = {
    Date : System.DateTime
    Model : Model
    Scenario : Scenario
    IntervalKind : IntervalKind
}

type PredictionDataPoint =
    { Date : System.DateTime
      Icu : int
      IcuLowerBound : int
      IcuUpperBound : int
      Hospitalized : int
      HospitalizedLowerBound : int
      HospitalizedUpperBound : int
      Deceased : int
      DeceasedLowerBound : int
      DeceasedUpperBound : int
      DeceasedToDate : int
      DeceasedToDateLowerBound : int
      DeceasedToDateUpperBound : int }

// type PredictionData = PredictionDataPoint list

// type PredictionsByKind = Map<PredictionKind, PredictionData>

// type PredictionsByModel = Map<Model, PredictionsByKind>

type PredictionsByDate = Map<System.DateTime, RemoteData<PredictionsByModel, string>>

type DisplayOption =
    | Model of Model
    | Scenario of Scenario
    | IntervalKind of IntervalKind

type DisplayOptions =
    { Models : Model list
      Scenarios : Scenario list
      IntervalKinds : IntervalKind list }

type SelectedDisplayOptions =
    { Models : Set<Model>
      Scenarios : Set<Scenario>
      IntervalKinds : Set<IntervalKind> }

    static member Empty =
        { Models = Set.empty
          Scenarios = Set.empty
          IntervalKinds = Set.empty }

type State =
    { StatsData : StatsData
      PredictionsData : PredictionsByDate
      DisplayOptions : RemoteData<DisplayOptions, string>
      SelectedDisplayOptions : SelectedDisplayOptions }

type Msg =
    | DisplayOptionChanged of DisplayOption * bool
