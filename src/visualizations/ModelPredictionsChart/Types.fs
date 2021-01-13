module ModelPredictionsChart.Types

open Types
open Data.ModelPredictions

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

// type PredictionsByDate = Map<System.DateTime, RemoteData<PredictionsByModel, string>>

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
      PredictionsMetadata : RemoteData<PredictionsMetadata, string>
      SelectedDisplayOptions : SelectedDisplayOptions }

type DisplayOption =
    | Model of Model
    | Scenario of Scenario
    | IntervalKind of IntervalKind

type Msg =
    | PredictionsMetadataLoaded of Result<PredictionsMetadata, string>
    | DisplayOptionChanged of DisplayOption * bool
