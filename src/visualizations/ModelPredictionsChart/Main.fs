module ModelPredictionsChart.Main

open Browser
open Feliz
open Feliz.UseDeferred

open Types

let init statsData =
    { StatsData = statsData
      SelectedDisplayOptions = SelectedDisplayOptions.Default }

let toggleSelected (selected : Set<'item>) (item  : 'item) =
    match selected.Contains item with
    | true -> selected.Remove item
    | false -> selected.Add item

let update state msg =
    match msg with
    | DisplayOptionChanged displayOption ->
        let oldOptions = state.SelectedDisplayOptions
        let newOptions =
            match displayOption with
            | Model item ->
                { oldOptions with Models = toggleSelected oldOptions.Models item }
            | Scenario item ->
                { oldOptions with Scenarios = toggleSelected oldOptions.Scenarios item }
            | IntervalKind item ->
                { oldOptions with IntervalKinds = toggleSelected oldOptions.IntervalKinds item }

        { state with SelectedDisplayOptions = newOptions }

let renderDisplaySelector (name : string) (selected : bool) onClick =
    let style =
        match selected with
        | true -> [ style.backgroundColor "gray" ; style.borderColor "gray" ]
        | false -> [ ]

    Html.div [
        prop.onClick onClick
        Utils.classes
            [(true, "btn btn-sm metric-selector")
             (selected, "metric-selector--selected")]
        prop.style style
        prop.text name ]

let renderDisplayTypeSelectors (predictionsMetadata : PredictionsMetadata) (state : State) dispatch =
    let models =
        predictionsMetadata.Models
        |> List.map (fun model ->
            renderDisplaySelector
                model.Name
                (state.SelectedDisplayOptions.Models.Contains model)
                (fun _ -> DisplayOptionChanged (Model model) |> dispatch))

    let scenarios =
        predictionsMetadata.Scenarios
        |> List.map (fun scenario ->
            renderDisplaySelector
                scenario.Name
                (state.SelectedDisplayOptions.Scenarios.Contains scenario)
                (fun _ -> DisplayOptionChanged (Scenario scenario) |> dispatch))

    let intervalKinds =
        predictionsMetadata.IntervalKinds
        |> List.map (fun intervalKind ->
            renderDisplaySelector
                intervalKind.Name
                (state.SelectedDisplayOptions.IntervalKinds.Contains intervalKind)
                (fun _ -> DisplayOptionChanged (IntervalKind intervalKind) |> dispatch))

    Html.div [
        prop.children [
            Html.div ((Html.text "Modeli: ") :: models)
            Html.div ((Html.text "Scenariji: ") :: scenarios)
            Html.div ((Html.text "Vrste intervalov: ") :: intervalKinds)
        ]
    ]

let chart = React.functionComponent("ModelPredictionsChart", fun (props : {| statsData : Types.StatsData |}) ->
    let (state, dispatch) = React.useReducer(update, init props.statsData)

    let deferredPredictionsMetadata = React.useDeferred(Data.ModelPredictions.loadPredictionsMetadata, [||])

    match deferredPredictionsMetadata with
    | Deferred.HasNotStartedYet
    | Deferred.InProgress -> React.fragment []
    | Deferred.Failed error -> Utils.renderErrorLoading error.Message
    | Deferred.Resolved predictionsMetadataResult ->
        match predictionsMetadataResult with
        | Error error -> Utils.renderErrorLoading error
        | Ok predictionsMetadata ->
            Html.div [
                Utils.renderChartTopControls [
                    renderDisplayTypeSelectors predictionsMetadata state dispatch
                ]
                Html.div [
                    prop.style [ style.height 450 ]
                    prop.className "highcharts-wrapper"
                    prop.children [
                        // Html.text (sprintf "%A" predictionsMetadata.Models)
                    ]
                ]
            ]
)
