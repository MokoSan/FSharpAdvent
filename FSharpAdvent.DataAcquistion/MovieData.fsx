#r @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\packages\FSharp.Data.2.4.3\lib\net45\FSharp.Data.dll"

open System
open FSharp.Data

[<Literal>]
let lotrFilmSeries           = @"https://en.wikipedia.org/wiki/The_Lord_of_the_Rings_(film_series)"

[<Literal>]
let fellowshipOfTheRingWiki  = @"https://en.wikipedia.org/wiki/The_Lord_of_the_Rings:_The_Fellowship_of_the_Ring"
[<Literal>]
let twoTowersWiki            = @"https://en.wikipedia.org/wiki/The_Lord_of_the_Rings:_The_Two_Towers"
[<Literal>]
let returnOfTheKingWiki      = @"https://en.wikipedia.org/wiki/The_Lord_of_the_Rings:_The_Return_of_the_King"

type LotrFilmSeriesProvider  =  HtmlProvider< lotrFilmSeries >
let lotrFilmSeriesProvider   = LotrFilmSeriesProvider.Load( lotrFilmSeries )

(* Entire Series - Overall Numbers *)

(** Table of Budget, Box Office and Running Time **)
let overallFirstTablesRows            = lotrFilmSeriesProvider.Tables.Table1.Rows

// Running Time
let overallRunningTime                = overallFirstTablesRows.[ 12 ].``The Lord of the Rings 2``  
let overallRunningTimeInMinutes       = int( overallRunningTime.Split(' ').[ 0 ] )

// Budget
let overallBudget                     = overallFirstTablesRows.[ 15 ].``The Lord of the Rings 2``
let overallBudgetInMillions           = int ( overallBudget.Split(' ').[ 0 ].Replace( "$", "" ))

// Box Office Revenue
let overallBoxOfficeRevenue           = overallFirstTablesRows.[ 16 ].``The Lord of the Rings 2``
let overallBoxOfficeRevenueInMillions = float( overallBoxOfficeRevenue.Split(' ').[ 0 ].Replace( "$", "" )) * 1000.

// List of Nominations vs. Wins 
lotrFilmSeriesProvider.Lists.``Academy Awards``.Values