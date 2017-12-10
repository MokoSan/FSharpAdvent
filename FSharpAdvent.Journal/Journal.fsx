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

(** Load the Character File from the Data Folder in the CSV Type Provider. **)
type LotrCsvProvider = CsvProvider< lotrCharacterDataFilePath >
let lotrCsvProvider  = LotrCsvProvider.Load( lotrCharacterDataFilePath ) 

(*** include-output:loading ***)
(** Additionally, let's add the Csv data into a Deedle Data Frame **) 
let lotrCsvDf = Frame.ReadCsv( lotrCharacterDataFilePath )

(*** define-output:chart ***)
let options = Options( page = "enable", pageSize = 20 )
lotrCsvDf
|> Chart.Table
|> Chart.WithOptions options
|> Chart.Show

(*** include-it:chart ***)

let characterToRaceSeries = 
    lotrCsvProvider.Rows
    |> Seq.map( fun r -> r.Name, r.Race )
    |> Seq.groupBy ( fun (_, r) -> r )
    |> series
printfn "%A" characterToRaceSeries

(** Now, let's build the chart. *)
(*** define-output:chart ***)
(*
words
|> Series.sort
|> Series.rev
|> Series.take 6
|> Chart.Column
*)
(*** include-it:chart ***)

(**
Summary
-------
An image is worth a thousand words:

![](http://imgs.xkcd.com/comics/hofstadter.png)
*)
