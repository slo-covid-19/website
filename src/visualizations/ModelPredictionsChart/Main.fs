module ModelPredictionsChart.Main

open Feliz
open Browser

open Types

let init statsData =
    { StatsData = statsData
      Predictions = Map.empty
      DisplayOptions = NotAsked
      SelectedDisplayOptions = SelectedDisplayOptions.empty }

let toggleSelected (set : Set<'item>) (item  : 'item) selected =
    match selected with
    | true -> set.Add item
    | false -> set.Remove item

let update state msg =
    match msg with
    | DisplayOptionChanged (displayOption, selected) ->
        let oldOptions = state.SelectedDisplayOptions
        let newOptions =
            match displayOption, selected with
            | Model item, selected ->
                { oldOptions with Models = toggleSelected oldOptions.Models item selected }
            | PredictionIntervalKind item, selected ->
                { oldOptions with PredictionIntervalKinds = toggleSelected oldOptions.PredictionIntervalKinds item selected }
            | PredictionIntervalWidth item, selected ->
                { oldOptions with PredictionIntervalWidths = toggleSelected oldOptions.PredictionIntervalWidths item selected }

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

    // let loadData () = async {
    //     let! data = Data.DailyDeaths.loadData ()
    //     dispatch (DailyDeathsDataReceived data)
    // }

    // React.useEffect(loadData >> Async.StartImmediate, [| |])

    // match state.WeeklyDeathsData with
    // | NotAsked
    // | Loading -> React.fragment [ ]
    // | Failure error -> Html.div [ Html.text error ]
    // | Success data ->
    Html.div [
        Utils.renderChartTopControls [
            // renderDisplayTypeSelectors state dispatch
        ]
        Html.div [
            prop.style [ style.height 450 ]
            prop.className "highcharts-wrapper"
            prop.children [
            ]
        ]
    ]
)
