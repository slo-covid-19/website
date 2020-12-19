module PhaseDiagram.RegionsData

open Types

let sumMunicipalites (regions : Region list) =
    regions
    |> List.map (fun dp ->
        {| Name = dp.Name
           ConfirmedToDate =
            dp.Municipalities |> List.sumBy (fun mun -> Option.defaultValue 0 mun.ConfirmedToDate)
           DeceasedToDate =
            dp.Municipalities |> List.sumBy (fun mun -> Option.defaultValue 0 mun.DeceasedToDate)
        |} )

type RegionDataPoint =
    { Date : System.DateTime
      ConfirmedToDate : int
      DeceasedToDate : int }

type RegionData =
    { Region : string
      Data : RegionDataPoint list }

let dataByRegion (data : RegionsData) =
    data
    |> List.collect (fun dp ->
        dp.Regions
        |> sumMunicipalites
        |> List.map (fun region -> {| Date = dp.Date ; Region = region |} ) )
    |> List.groupBy (fun dp -> dp.Region.Name)
    |> List.map (fun (region, dps) ->
        { Region = region
          Data = dps |> List.map (fun dp ->
            { Date = dp.Date
              ConfirmedToDate = dp.Region.ConfirmedToDate
              DeceasedToDate = dp.Region.DeceasedToDate } ) } )

let totalVsWeekData (metric : Metric) data =
    let windowSize = 7

    data
    |> List.map (fun dp ->
        let region = dp.Region
        let data =
            dp.Data
            |> List.toArray
            |> Array.windowed windowSize
            |> Array.map (fun days ->
                let first = Array.head days
                let last = Array.last days
                match metric with
                | Cases ->
                    { date = last.Date
                      x = last.ConfirmedToDate
                      y = last.ConfirmedToDate - first.ConfirmedToDate } |> Some
                | Deceased ->
                    { date = last.Date
                      x = last.DeceasedToDate
                      y = last.DeceasedToDate - first.DeceasedToDate } |> Some
                | Hospitalized -> None )

        {| Region = region ; Data = Array.choose id data |} )
