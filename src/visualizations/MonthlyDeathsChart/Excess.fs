module MonthlyDeathsChart.Excess

open Browser
open Highcharts
open Fable.Core.JsInterop

open Types

let colors = {|
    ExcessDeaths = "#ff3333"
    CovidDeaths = "#a483c7"
|}

let renderChartOptions (data : MonthlyDeathsData) (statsData : StatsData) =

    let baselineStartYear, baselineEndYear = 2015, 2019

    let deceasedBaseline =
        data
        |> List.filter (fun dp -> dp.year >= baselineStartYear && dp.year <= baselineEndYear)
        |> List.groupBy (fun dp -> dp.month)
        |> List.map (fun (month, dps) ->
            (month, (List.sumBy (fun (dp : Data.MonthlyDeaths.DataPoint) -> float dp.deceased) dps) / float (baselineEndYear - baselineStartYear + 1)) )

    let deceasedCurrentYear =
        data
        |> List.filter (fun dp -> dp.year = System.DateTime.Today.Year)
        |> List.map (fun dp -> (dp.month, dp.deceased))

    let deceasedCurrentYearRelativeToBaseline =
        let deceasedBaselineMap =
            deceasedBaseline
            |> FSharp.Collections.Map

        deceasedCurrentYear
        |> List.map (fun (month, deceased) ->
            match deceasedBaselineMap.TryFind(month) with
            | None -> None
            | Some baseline ->
                Some (month, (float deceased - baseline) / baseline * 100.) )
        |> List.choose id

    let deceasedCovidCurrentYear =
        statsData
        // Filter the data to the current year
        |> List.filter (fun dp -> dp.Date.Year = System.DateTime.Today.Year)
        // Select only the non-empty deceased data points
        |> List.map (fun dp ->
            match dp.StatePerTreatment.Deceased with
            | None -> None
            | Some deceased -> Some {| Date = dp.Date ; Deceased = deceased |} )
        |> List.choose id
        |> List.groupBy (fun dp -> dp.Date.Month)
        |> List.map (fun (month, deceased) ->
            let deceasedSum =
                deceased
                |> List.map (fun dp -> dp.Deceased)
                |> List.sum
            (month, deceasedSum) )

    let deceasedCovidCurrentYearPercent =
        let deceasedCurrentYearMap =
            deceasedCurrentYear
            |> FSharp.Collections.Map

        deceasedCovidCurrentYear
        |> List.map (fun (month, deceasedCovid) ->
            match deceasedCurrentYearMap.TryFind(month) with
            | None -> None
            | Some deceasedTotal ->
                Some (month, float deceasedCovid / float deceasedTotal * 100.) )
        |> List.choose id

    let series =
        [|
            {| ``type`` = "line"
               name = (I18N.t "charts.monthlyDeaths.excess.excessDeaths")
               marker = {| enabled = false |} |> pojo
               color = colors.ExcessDeaths
               data =
                   deceasedCurrentYearRelativeToBaseline
                   |> List.map (fun (month, percent) ->
                       {| x = month
                          y = percent
                          name = Utils.monthNameOfIndex month
                       |} |> pojo)
                   |> List.toArray
            |} |> pojo
            {| ``type`` = "area"
               name = (I18N.t "charts.monthlyDeaths.excess.covidDeaths")
               marker = {| enabled = false |} |> pojo
               color = colors.CovidDeaths
               data =
                   deceasedCovidCurrentYearPercent
                   |> List.map (fun (month, percent) ->
                       {| x = month
                          y = percent
                          name = Utils.monthNameOfIndex month
                       |} |> pojo)
                   |> List.toArray
            |} |> pojo
        |]

    {| baseOptions with
        yAxis =
            {| title = {| text = "" |}
               labels = {| formatter = fun (x) -> x?value + " %" |} |> pojo
            |}
        tooltip = {| formatter = fun () -> sprintf "%s<br>%.2f %%" (Utils.monthNameOfIndex jsThis?x) jsThis?y |} |> pojo
        series = series |} |> pojo
