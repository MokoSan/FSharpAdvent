(*** hide ***)
#load "packages/FsLab/FsLab.fsx"
(**

Lord of the Rings: An F# Approach
========================

FsLab journal is a simple Visual Studio template that makes it easy to do
interactive data analysis using F# Interactive and produce nice HTML or PDF
to document you research.

Next steps
----------

 * To learn more about FsLab Journal, go to "Solution Explorer", right click
   on your newly created project, select "Add", "New item.." and choose
   "FsLab Walkthrough" (if you do not have R statistics environment installed)
   or "FsLab Walkthrough with R" (if you do have R).

 * To add new experiments to your project, got to "Add", "New item" and choose
   new "FsLab Experiment". You can have multiple scripts in a single project.

 * To see how things work, hit **F5** and see how FsLab Journal turns this
   Markdown document with simple F# script into a nice report!

 * To generate PDF from your experiments, you need to install `pdflatex` and
   have it accessible in the system `PATH` variable. Then you can run
   `build pdf` in the folder with this script (then check out `output` folder).

Sample experiment
-----------------

We start by referencing `Deedle` and `FSharp.Charting` libraries and then we
load the contents of *this* file:
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
Loading the Character Data into the Journal 
-------------------------------------------
*)

[<Literal>]
let lotrCharacterDataFilePath = __SOURCE_DIRECTORY__+  @"\..\Data\Characters.csv"

let lotrCharacterDf : Frame< int, string > 
    = Frame.ReadCsv( lotrCharacterDataFilePath )

let lotrCharacterDfCleaned : Frame< string, string > =  
    lotrCharacterDf
    |> Frame.indexRowsString "Name"
    |> Frame.dropCol "Url"

let options = Options( page = "enable", pageSize = 20 )
lotrCharacterDfCleaned
|> Chart.Table
|> Chart.WithOptions options
|> Chart.Show

let races = [ "Human"; "Hobbit"; "Elf"; "Dwarf"; "Maiar" ]

let getSeriesByRaceName ( race : string ) : Series< string, string > = 
    lotrCharacterDfCleaned.GetColumn "Race"
    |> Series.filterValues( (=) race )

let getSeriesByRaceCount ( race : string ) : int = 
    getSeriesByRaceName race |> Series.countKeys

let raceCountDf : Frame<string, string> = 
    let allRaceCountSeries = 
        races
        |> List.map( fun r -> "Race", r, getSeriesByRaceCount r )
        |> Seq.ofList

    allRaceCountSeries
    |> Frame.ofValues

raceCountDf
|> Chart.Column
|> Chart.WithTitle "Distribution of Characters by Race"
|> Chart.WithYTitle "Count"
|> Chart.WithLegend true
|> Chart.Show

raceCountDf
|> Chart.Table

// Fool of a Took

let hobbitSeries : Series<string, string> = getSeriesByRaceName "Hobbit"


// Race Prediction

// Get First Name

let nameSeries : Series< int, string > =  
    lotrCharacterDf
    |> Frame.getCol "Name"

// Only consider the first name
let firstName = 
    nameSeries
    |> Series.mapValues ( fun r -> r.Split ' ' |> Array.head )

lotrCharacterDf.AddColumn ( "FirstName", firstName )
lotrCharacterDf.DropColumn "Url"
lotrCharacterDf.DropColumn "Name"

let filteredCharacterDf : Frame < int, string > = 
    lotrCharacterDf
    |> Frame.filterRowValues ( fun r -> r.GetAs< string >( "Race" ) <> "Maiar" )

#r @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\packages\Accord.3.8.0\lib\net45\Accord.dll"
#r @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\packages\Accord.MachineLearning.3.8.0\lib\net45\Accord.MachineLearning.dll"
#r @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\packages\Accord.Math.3.8.0\lib\net45\Accord.Math.dll"
#r @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\packages\Accord.Math.3.8.0\lib\net45\Accord.Math.Core.dll"
#r @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\packages\Accord.Statistics.3.8.0\lib\net45\Accord.Statistics.dll"

open System

// Data Partitioning

open RDotNet
open RProvider
open RProvider.``base``
open RProvider.utils

R.install_packages("caret")

open RProvider.caret

let range = [ 1..filteredCharacterDf.Rows.KeyCount ]

let trainingIdx : int[] = R.sample( range, float( range.Length ) * 0.7 ).GetValue()
let testingIdx  : int[] = R.setdiff( range, trainingIdx ).GetValue()

let trainingData : Frame< int, string > = filteredCharacterDf.Rows.[ trainingIdx ] 
let testingData  : Frame< int, string > = filteredCharacterDf.Rows.[ testingIdx ]

let getInputs ( dataSet : Frame< int, string > ): string[] =
    filteredCharacterDf
    |> Frame.getCol "FirstName"
    |> Series.values
    |> Seq.toArray

let getOutputs ( dataSet : Frame< int, string > ) : int[] = 
    filteredCharacterDf
    |> Frame.getCol "Race"
    |> Series.values
    |> Seq.toArray
    |> Array.map( fun o -> 
        match o with
        | "Human"  -> 0
        | "Hobbit" -> 1
        | "Elf"    -> 2
        | "Dwarf"  -> 3
    )

let raceCount : int = races.Length 

open System

open Accord.Math.Distances
open Accord.MachineLearning
open Accord.Statistics.Analysis

// Learning phase should be based on the Training Data 
let trainingDataInputs  : string[] = getInputs( trainingData ) 
let trainingDataOutputs : int[]    = getOutputs( trainingData ) 

// Function that trains the Model based on the 'k', the number of nearest neighbors
// to consider to classify a new data point.
let train ( k : int ) : KNearestNeighbors< string > = 
    let knn : KNearestNeighbors< string > = 
        KNearestNeighbors< string >( k = k, distance = Levenshtein() )

    knn.Learn( trainingDataInputs, trainingDataOutputs )

//let k : int = int ( Math.Ceiling ( Math.Sqrt( float( trainingDataInputs.Length ) / 2.0 )))

let predict ( knn : KNearestNeighbors< string > ) 
            ( name : string ) : string = 
    let decidedClass = knn.Decide( name )
    match decidedClass with
        | 0 -> "Human" 
        | 1 -> "Hobbit"
        | 2 -> "Elf" 
        | 3 -> "Dwarf"
        | _ -> failwith "Class not found"

let modelWithK5 = train 5

predict modelWithK5 "Moko" 
predict modelWithK5 "Bori" 
predict modelWithK5 "Terry" 

// Cross Validation should be based on the Testing Data 
let testingDataInputs  : string[] = getInputs( testingData ) 
let testingDataOutputs : int[]    = getOutputs( testingData )

let knnWith5Ks      = train( 5 )
let confusionMatrix = 
    GeneralConfusionMatrix.Estimate( knnWith5Ks, 
                                     testingDataInputs, 
                                     testingDataOutputs )
let accuracy        = confusionMatrix.Accuracy
let error           = confusionMatrix.Error

let crossValidationBasedOnK ( k : int ) : GeneralConfusionMatrix = 
    let knnModel        : KNearestNeighbors< string > 
        = train( k )
    GeneralConfusionMatrix.Estimate( knnModel, testingDataInputs, testingDataOutputs )

let rangeOfKs : int list  = [ 1..20 ]
let crossValidationResults : GeneralConfusionMatrix list = 
    rangeOfKs
    |> List.map( crossValidationBasedOnK )

let listOfAccuracies : float list = 
    crossValidationResults
    |> List.map( fun c -> c.Accuracy * 100.0 )

let listOfErrors : float list = 
    crossValidationResults
    |> List.map( fun c -> c.Error * 100.0 )

// Let's chart.
let ksToAccuracies : Frame< string, int > = 
    List.zip rangeOfKs listOfAccuracies 
    |> List.map( fun ( k, a ) -> "K", k, a )
    |> Frame.ofValues

ksToAccuracies
|> Chart.Column
|> Chart.WithTitle "Accuracies vs. K"
|> Chart.WithYTitle "Accuracy [ % ]"
|> Chart.WithLegend true
|> Chart.Show

let ksToErrors : Frame< string, int > = 
    List.zip rangeOfKs listOfErrors 
    |> List.map( fun ( k, e) -> "K", k, e )
    |> Frame.ofValues

ksToErrors
|> Chart.Column
|> Chart.WithTitle "Errors vs. K"
|> Chart.WithYTitle "Error [ % ]"
|> Chart.WithLegend true
|> Chart.Show



(**
Summary
-------
An image is worth a thousand words:

![](http://imgs.xkcd.com/comics/hofstadter.png)
*)
