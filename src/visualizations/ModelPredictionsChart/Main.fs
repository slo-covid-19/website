module ModelPredictionsChart.Main

open Feliz
open Browser
open Fable.Core.JsInterop

open Types

let init statsData =
    { StatsData = statsData
      DisplayType = Default }


let update state msg =
    match msg with
    | DisplayTypeChanged displayType ->
        { state with DisplayType = displayType }


let renderDisplayTypeSelectors state dispatch =
    let selectors =
        DisplayType.available
        |> List.map (fun dt ->
            Html.div [
                prop.onClick (fun _ -> DisplayTypeChanged dt |> dispatch)
                Utils.classes
                    [(true, "chart-display-property-selector__item")
                     (state.DisplayType = dt, "selected")]
                prop.text (DisplayType.getName dt) ] )

    Html.div [
        prop.className "chart-display-property-selector"
        prop.children selectors
    ]


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
            renderDisplayTypeSelectors state dispatch ]
        Html.div [
            prop.style [ style.height 450 ]
            prop.className "highcharts-wrapper"
            prop.children [
            ]
        ]
    ]
)
