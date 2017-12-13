(*** hide ***)
#load "packages/FsLab/FsLab.fsx"
(**

Lord of the Rings: An F# Approach - The Path of the Hobbits
===========================================================

This path involves answering the two questions:

Which movie series, The Lord of the Rings or The Hobbit, is the better movie series?

Our Analysis involves comparing the following features of the two movies series:
1. Rate of Return [ Appeals to the Movie Production ] 
2. Oscar Nominations [ Appeals to the Critics ]
3. Rotten Tomatoes  

----------

-----------------
*)

(** 
Referencing the Libraries 
-------------------------
*)
(*** define-output:loading ***)
open FSharp.Data
open Deedle
open System.IO
open XPlot.GoogleCharts
open XPlot.GoogleCharts.Deedle

(** 
Movie Series Comparison
-------------------------------------------
*)

[<Literal>]
let moviesFile = __SOURCE_DIRECTORY__+  @"\..\Data\Movies.csv"

(*** include-output:loading ***)
(** Let's add the Csv data into a Deedle Data Frame **) 
let moviesDf : Frame< int, string > = Frame.ReadCsv( moviesFile )

// Let's set the index oto that of the 
let indexedMoviesDf = 
    moviesDf
    |> Frame.indexRowsString "Name"

let movieSeriesDf =
    indexedMoviesDf
    |> Frame.sliceRows[ "The Lord of the Rings Series"; "The Hobbit Series"]

// Feature 1: Rate of Return.
let profitDf = 
    movieSeriesDf
    |> Frame.sliceCols [ "BudgetInMillions";  "BoxOfficeRevenueInMillions" ]

(*** define-output:chart ***)

Chart.Column( profitDf )
|> Chart.WithTitle "Budget and Revenue Comparision"
|> Chart.WithXTitle "Series Name"
|> Chart.WithYTitle "$ [ In Millions ]"
|> Chart.WithLegend true
|> Chart.Show

(*** include-it:chart ***)

let profitSeries : Series< string, float > = 
    profitDf?BoxOfficeRevenueInMillions - profitDf?BudgetInMillions

let rateOfReturnSeries : Series< string, float > =
    (( profitDf?BoxOfficeRevenueInMillions - profitDf?BudgetInMillions ) / profitDf?BudgetInMillions ) * 100.0

profitDf.AddColumn( "Profit [Mill. $]", profitSeries )
profitDf

profitDf.RenameColumn( "BudgetInMillions", "Budget [Mill. $]")
profitDf.RenameColumn( "BoxOfficeRevenueInMillions", "Box Office Revenue [Mill. $]")

(*** define-output:chart ***)

Chart.Column( profitDf )
|> Chart.WithTitle "Budget, Revenue and Profit"
|> Chart.WithXTitle "Series"
|> Chart.WithYTitle "$ [ In Millions ]"
|> Chart.WithLegend true
|> Chart.Show

// Adding the Rate of Return
profitDf.AddColumn( "Rate of Return in %", rateOfReturnSeries )
let rateOfReturnDf =
    profitDf
    |> Frame.sliceCols [ "Rate of Return in %" ]

Chart.Column( rateOfReturnDf )
|> Chart.WithTitle "Rate of Return"
|> Chart.WithXTitle "Series"
|> Chart.WithYTitle "Rate of Return in %"
|> Chart.WithOptions( Options( colors = [| "red" |] ))
|> Chart.WithLegend true
 
profitDf
|> Chart.Table

(*** include-it:chart ***)

// Feature 2: Academy Award - Wins / Nominations
let academyAwardsDf = 
    movieSeriesDf
    |> Frame.sliceCols [ "AcademyAwardNominations";  "AcademyAwardWins" ]

let academyAwardLosses = 
    academyAwardsDf?AcademyAwardNominations - academyAwardsDf?AcademyAwardWins

academyAwardsDf.AddColumn( "AcademyAwardLoses", academyAwardLosses )

let lotrAcademyAwardsDf =
    academyAwardsDf
    |> Frame.sliceRows [ "The Lord of the Rings Series" ]

lotrAcademyAwardsDf.DropColumn( "AcademyAwardNominations" )

let lotrAcademyAwardWins = 
    lotrAcademyAwardsDf?AcademyAwardWins.Values
    |> Seq.item 0

let lotrAcademyAwardLosses = 
    lotrAcademyAwardsDf?AcademyAwardLoses.Values
    |> Seq.item 0

let lotrAcademyAwardPieChart = 
    [ "Wins", lotrAcademyAwardWins; "Loses", lotrAcademyAwardLosses ]
    |> Chart.Pie
    |> Chart.WithTitle "The Lord of the Rings - Academy Awards Wins vs. Losses"
    |> Chart.WithLegend true

let hobbitAcademyAwardsDf =
    academyAwardsDf
    |> Frame.sliceRows [ "The Hobbit Series" ]

hobbitAcademyAwardsDf.DropColumn( "AcademyAwardNominations" )

let hobbitAcademyAwardWins = 
    hobbitAcademyAwardsDf?AcademyAwardWins.Values
    |> Seq.item 0

let hobbitAcademyAwardLosses = 
    hobbitAcademyAwardsDf?AcademyAwardLoses.Values
    |> Seq.item 0

let hobbitAcademyAwardPieChart = 
    [ "Wins", hobbitAcademyAwardWins; "Loses", hobbitAcademyAwardLosses ]
    |> Chart.Pie
    |> Chart.WithTitle "The Hobbit - Academy Awards Wins vs. Losses"
    |> Chart.WithLegend true

// Feature 3: Rotten Tomatoes 
let rottenTomatoesScoreDf = 
    movieSeriesDf
    |> Frame.sliceCols [ "RottenTomatoesScore";  ]

Chart.Table rottenTomatoesScoreDf
    
(**
Summary
-------
An image is worth a thousand words:

![](http://imgs.xkcd.com/comics/hofstadter.png)
*)
