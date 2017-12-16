(*** hide ***)
#load "packages/FsLab/FsLab.fsx"
(**

Lord of the Rings: An F# Approach - The Path of the Hobbits
===========================================================

This path involves answering the two questions:

Which movie series, The Lord of the Rings or The Hobbit, is the better movie series?

Our Analysis involves comparing the following features of the two movies series:
1. Rotten Tomatoes
2. Return on Investment
3. Oscar Nominations

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

// Feature 1: Rotten Tomatoes 
let rottenTomatoesScoreDf = 
    movieSeriesDf
    |> Frame.sliceCols [ "RottenTomatoesScore";  ]

Chart.Table rottenTomatoesScoreDf

// Feature 2: Return on Investment.
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

// Adding the Return on Investment
let roiSeries : Series< string, float > =
    (( profitDf?BoxOfficeRevenueInMillions - profitDf?BudgetInMillions )
        / profitDf?BudgetInMillions ) * 100.0

let profitSeries : Series< string, float > = 
    profitDf?BoxOfficeRevenueInMillions - profitDf?BudgetInMillions

profitDf.AddColumn( "Profit [Mill. $]", profitSeries )

profitDf.RenameColumn( "BudgetInMillions", "Budget [Mill. $]")
profitDf.RenameColumn( "BoxOfficeRevenueInMillions", "Box Office Revenue  [Mill. $]")

(*** define-output:chart ***)

Chart.Column( profitDf )
|> Chart.WithTitle "Budget, Revenue and Profit"
|> Chart.WithXTitle "Series"
|> Chart.WithYTitle "$ [ In Millions ]"
|> Chart.WithLegend true
|> Chart.Show


profitDf.AddColumn( "ROI in %", roiSeries )

let roiDf : Frame< string, string > =
    profitDf
    |> Frame.sliceCols [ "ROI in %" ]

Chart.Column( roiDf )
|> Chart.WithTitle "Return on Investment"  
|> Chart.WithXTitle "Series"
|> Chart.WithYTitle "ROI [ % ]"
|> Chart.WithLegend true
 
profitDf
|> Chart.Table

// Feature 3: Academy Award - Wins / Nominations
let academyAwardsDf : Frame< string, string > = 
    movieSeriesDf
    |> Frame.sliceCols [ "AcademyAwardNominations";  "AcademyAwardWins" ]

let academyAwardLosses : Series< string, float > = 
    academyAwardsDf?AcademyAwardNominations - academyAwardsDf?AcademyAwardWins

academyAwardsDf.AddColumn( "AcademyAwardLoses", academyAwardLosses )

let lotrAcademyAwardsDf : Frame< string, string > =
    academyAwardsDf
    |> Frame.sliceRows [ "The Lord of the Rings Series" ]

let lotrAcademyAwardWins : float = 
    lotrAcademyAwardsDf?AcademyAwardWins.Values
    |> Seq.head

let lotrAcademyAwardLosses : float = 
    lotrAcademyAwardsDf?AcademyAwardLoses.Values
    |> Seq.head

let lotrAcademyAwardPieChart : GoogleChart = 
    [ "Wins", lotrAcademyAwardWins; "Loses", lotrAcademyAwardLosses ]
    |> Chart.Pie
    |> Chart.WithTitle "The Lord of the Rings - Academy Awards Wins vs. Losses"
    |> Chart.WithLegend true
lotrAcademyAwardPieChart

let hobbitAcademyAwardsDf : Frame< string, string > =
    academyAwardsDf
    |> Frame.sliceRows [ "The Hobbit Series" ]

let hobbitAcademyAwardWins : float = 
    hobbitAcademyAwardsDf?AcademyAwardWins.Values
    |> Seq.head

let hobbitAcademyAwardLosses : float = 
    hobbitAcademyAwardsDf?AcademyAwardLoses.Values
    |> Seq.head

let hobbitAcademyAwardPieChart : GoogleChart = 
    [ "Wins", hobbitAcademyAwardWins; "Loses", hobbitAcademyAwardLosses ]
    |> Chart.Pie
    |> Chart.WithTitle "The Hobbit - Academy Awards Wins vs. Losses"
    |> Chart.WithLegend true
hobbitAcademyAwardPieChart

let academyAwardWinPercentageSeries : Series< string, float > = 
    ( academyAwardsDf?AcademyAwardWins 
        / academyAwardsDf?AcademyAwardNominations ) * 100.0 

academyAwardsDf.AddColumn( "AcademyAwardWinPercentage", academyAwardWinPercentageSeries )

let academyAwardWinsDf : Frame< string, string > = 
    let academyAwardWinsDf = 
        academyAwardsDf 
        |> Frame.sliceCols [ "AcademyAwardWinPercentage" ]

    academyAwardWinsDf.RenameColumn( "AcademyAwardWinPercentage", "Wins %")
    academyAwardWinsDf
    

Chart.Column( academyAwardWinsDf )
|> Chart.WithTitle "Academy Awards Wins %"  
|> Chart.WithXTitle "Series"
|> Chart.WithYTitle "Wins [ % ]"
|> Chart.WithLegend true

// Result Df.

(*** include-it:chart ***)

(**
Summary
-------
An image is worth a thousand words:

![](http://imgs.xkcd.com/comics/hofstadter.png)
*)
