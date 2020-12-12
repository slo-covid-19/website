module ModelPredictionsChart.Types

open Types

type DisplayType =
    | Default
with
    static member available = [ Default ]

    static member getName = function
        | Default -> I18N.chartText "modelPredictions" "default.title"

type State = {
    StatsData : StatsData
    DisplayType : DisplayType
}

type Msg =
    | DisplayTypeChanged of DisplayType
