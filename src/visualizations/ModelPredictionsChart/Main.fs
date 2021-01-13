module ModelPredictionsChart.Main

open Feliz
open Feliz.UseDeferred
open Browser

open Types

let init statsData =
    { StatsData = statsData
      PredictionsMetadata = NotAsked
      SelectedDisplayOptions = SelectedDisplayOptions.Empty }

let toggleSelected (set : Set<'item>) (item  : 'item) selected =
    match selected with
    | true -> set.Add item
    | false -> set.Remove item

let update state msg =
    match msg with
    | PredictionsMetadataLoaded predictionsMetadataResults ->
        let predictionsMetadata =
            match predictionsMetadataResults with
            | Ok data -> Success data
            | Error error -> Failure error
        { state with PredictionsMetadata = predictionsMetadata }
    | DisplayOptionChanged (displayOption, selected) ->
        let oldOptions = state.SelectedDisplayOptions
        let newOptions =
            match displayOption, selected with
            | Model item, selected ->
                { oldOptions with Models = toggleSelected oldOptions.Models item selected }
            | Scenario item, selected ->
                { oldOptions with Scenarios = toggleSelected oldOptions.Scenarios item selected }
            | IntervalKind item, selected ->
                { oldOptions with IntervalKinds = toggleSelected oldOptions.IntervalKinds item selected }

        { state with SelectedDisplayOptions = newOptions }

// let renderDisplayTypeSelectors state dispatch =
//     let selectors =
//         DisplayType.available
//         |> List.map (fun dt ->
//             Html.div [
//                 prop.onClick (fun _ -> DisplayTypeChanged dt |> dispatch)
//                 Utils.classes
//                     [(true, "chart-display-property-selector__item")
//                      (state.DisplayType = dt, "selected")]
//                 prop.text (DisplayType.getName dt) ] )

//     Html.div [
//         prop.className "chart-display-property-selector"
//         prop.children selectors
//     ]


let chart = React.functionComponent("ModelPredictionsChart", fun (props : {| statsData : Types.StatsData |}) ->
    let (state, dispatch) = React.useReducer(update, init props.statsData)

    let loadPredictionsMetadata () = async {
        let! data = Data.ModelPredictions.loadPredictionsMetadata ()
        dispatch (PredictionsMetadataLoaded data)
    }

    React.useEffect(loadPredictionsMetadata >> Async.StartImmediate, [| |])

    match state.PredictionsMetadata with
    | NotAsked
    | Loading -> React.fragment []
    | Failure error -> Utils.renderErrorLoading error
    | Success predictionsMetadata ->
        Html.div [
            Utils.renderChartTopControls [
                // renderDisplayTypeSelectors state dispatch
            ]
            Html.div [
                prop.style [ style.height 450 ]
                prop.className "highcharts-wrapper"
                prop.children [
                    Html.text (sprintf "%A" predictionsMetadata.Models)
                ]
            ]
        ]
)
