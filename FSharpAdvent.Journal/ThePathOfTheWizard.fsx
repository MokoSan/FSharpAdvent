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

let firstName = 
    nameSeries
    |> Series.mapValues ( fun r -> r.Split ' ' |> Array.head )

lotrCharacterDf.AddColumn ( "FirstName", firstName )
lotrCharacterDf.DropColumn "Url"
lotrCharacterDf.DropColumn "Name"

let filteredCharacterDf : Frame < int, string > = 
    lotrCharacterDf
    |> Frame.filterRowValues ( fun r -> r.GetAs< string >( "Race" ) <> "Maiar" )

let inputs : string[] =
    filteredCharacterDf
    |> Frame.getCol "FirstName"
    |> Series.values
    |> Seq.toArray

let outputs : int[] = 
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

let inputsAndOutputsCombined = 
    Array.zip inputs outputs

let raceCount : int = races.Length 

#r @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\packages\Accord.3.8.0\lib\net45\Accord.dll"
#r @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\packages\Accord.MachineLearning.3.8.0\lib\net45\Accord.MachineLearning.dll"
#r @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\packages\Accord.Math.3.8.0\lib\net45\Accord.Math.dll"
#r @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\packages\Accord.Math.3.8.0\lib\net45\Accord.Math.Core.dll"
#r @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\packages\Accord.Statistics.3.8.0\lib\net45\Accord.Statistics.dll"

open Accord.Math.Distances
open Accord.MachineLearning
open Accord.Statistics.Analysis

open System
open MathNet.Numerics.Random

// Data Partitioning

[<Literal>]
let trainingDataSplit = 0.8

// Splitting Training and Testing data set
let totalNumberOfDataPoints = inputs.Length 

let combinedDataIdx =  
    let random = Random.doublesSeed 100 totalNumberOfDataPoints 
    let training =
        random
        |> Array.mapi( fun i r -> i, r < trainingDataSplit )

    let testing = 
        random
        |> Array.mapi( fun i r -> i, r > trainingDataSplit )

    training, testing 

let trainingData =
    let getAllFilteredTrainingData =
        ( fst combinedDataIdx )
        |> Array.filter( fun ( r,i ) -> ( i ))
        |> Array.map( fun ( r, i ) -> r ) 

    getAllFilteredTrainingData
    |> Array.map( fun r -> inputsAndOutputsCombined.[ r ])

let testingData = 
    let getAllFilteredTestingData =
        ( snd combinedDataIdx )
        |> Array.filter( fun ( r,i ) -> ( i ))
        |> Array.map( fun ( r, i ) -> r ) 

    getAllFilteredTestingData 
    |> Array.map( fun r -> inputsAndOutputsCombined.[ r ])

let trainingDataInput =
    trainingData
    |> Array.map( fun d -> fst d )

let trainingDataOutput =
    trainingData
    |> Array.map( fun d -> snd d )

let testingDataInput =
    trainingData
    |> Array.map( fun d -> fst d )

let testingDataOutput =  
    trainingData
    |> Array.map( fun d -> snd d )
    
// Learning phase should be based on the Training Data 
let knn           = KNearestNeighbors< string >( k = raceCount, distance = Levenshtein() )
let knnLearnt     = knn.Learn( trainingDataInput, trainingDataOutput )

let classificationResultWrapper ( name : string ) 
                                ( knn : KNearestNeighbors< string > ) : string = 
    let decidedClass = knn.Decide( name )
    match decidedClass with
        | 0 -> "Human" 
        | 1 -> "Hobbit"
        | 2 -> "Elf" 
        | 3 -> "Dwarf"
        | _ -> failwith "Class not found"

classificationResultWrapper "Ardamir" knnLearnt

// Confusion Matrix should be based on the Testing Data 
let confusionMatrix = GeneralConfusionMatrix.Estimate( knnLearnt, testingDataInput, testingDataOutput )
let accuracy        = confusionMatrix.Accuracy
let error           = confusionMatrix.Error
let kappa           = confusionMatrix.Kappa

(**
Summary
-------
An image is worth a thousand words:

![](http://imgs.xkcd.com/comics/hofstadter.png)
*)
