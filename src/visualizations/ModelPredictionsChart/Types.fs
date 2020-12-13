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

type Scenario = Scenario of string

type PredictionIntervalKind = PredictionIntervalKind of string

type PredictionIntervalWidth = PredictionIntervalWidth of int

type PredictionData =
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

type PredictionInterval =
    { Type : PredictionIntervalKind
      Width : PredictionIntervalWidth }

type PredictionKind =
    { Scenario : Scenario
      Interval : PredictionInterval }

type PredictionsByKind = Map<PredictionKind, PredictionData>

type PredictionsByModel = Map<Model, PredictionsByKind>

type PredictionsByDate = Map<System.DateTime, RemoteData<PredictionsByModel, string>>

type DisplayOption =
    | Model of Model
    | PredictionIntervalKind of PredictionIntervalKind
    | PredictionIntervalWidth of PredictionIntervalWidth

type DisplayOptions =
    { Models : Model list
      PredictionIntervalTypes : PredictionIntervalKind list
      PredictionIntervalWidths : PredictionIntervalWidth list }

type SelectedDisplayOptions =
    { Models : Set<Model>
      PredictionIntervalKinds : Set<PredictionIntervalKind>
      PredictionIntervalWidths : Set<PredictionIntervalWidth> }

    static member empty =
        { Models = Set.empty
          PredictionIntervalKinds = Set.empty
          PredictionIntervalWidths = Set.empty }

type State =
    { StatsData : StatsData
      Predictions : PredictionsByDate
      DisplayOptions : RemoteData<DisplayOptions, string>
      SelectedDisplayOptions : SelectedDisplayOptions }

type Msg =
    | DisplayOptionChanged of DisplayOption * bool
