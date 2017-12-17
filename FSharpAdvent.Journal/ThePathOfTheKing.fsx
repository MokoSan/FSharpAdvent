(*** hide ***)
#load "packages/FsLab/FsLab.fsx"
#load @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\FSharpAdvent.DataAcquistion\BookData.fsx"
(**

Lord of the Rings: An F# Approach - The Path of the King 
===========================================================

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

open Newtonsoft.Json

[<Literal>]
let lotrBookFilePath = __SOURCE_DIRECTORY__+  @"..\Data\LordOfTheRingsBook.json"

[<Literal>]
let hobbitBookFilePath = __SOURCE_DIRECTORY__ + @"..\Data\HobbitBook.json"

let allLOTRText     = File.ReadAllText lotrBookFilePath
let allLOTRBookData = JsonConvert.DeserializeObject< BookData[] >( allLOTRText ) 

type CharacterMentions = { CharacterName : string; CharacterMentions : int; }
type ChapterWordCount  = { BookName : Book; Chapter : string; Count : int;  }

(* Seggregate Books *)

let fellowshipOfTheRingBookData : BookData[] =
    allLOTRBookData 
    |> Array.filter( fun a -> a.BookName = TheFellowshipOfTheRing )

let twoTowersBookData : BookData[]  =
    allLOTRBookData 
    |> Array.filter( fun a -> a.BookName = TheTwoTowers )

let returnOfTheKingBookData : BookData[] =
    allLOTRBookData 
    |> Array.filter( fun a -> a.BookName = TheReturnOfTheKing )

(* Word Counts *) 

let splitValues = [| ' '; '.'; '-'; ','; '!' |]
let splitWords( data : BookData[] ) : string [] =

    let cleanString ( input : string ) : string = 
        input.Replace("`", "")
             .Replace("'", "")
             .Replace("-", "")
             .Replace(";", "")
             .Replace("?", "")
             .Replace("'", "")
             .Replace("\"", "")
        
    let splitAndCleaned = 
        data 
        |> Array.collect( fun c -> 
            c.ChapterData.Trim().Split( splitValues ))
        |> Array.map( fun c -> cleanString( c )) 
        |> Array.filter( fun c -> c <> "'" && c <> "" && c <> "-" && c.Length <> 0 )
    splitAndCleaned

let totalWordCount = 
    splitWords( allLOTRBookData )
    |> Array.length

let forWordCount = 
    splitWords( fellowshipOfTheRingBookData )
    |> Array.length

let ttWordCount = 
    splitWords( twoTowersBookData )
    |> Array.length

let rokWordCount = 
    splitWords( returnOfTheKingBookData )
    |> Array.length

(* Word Counts Per Chapter *)

let getChapterCounts ( book : BookData[] ) : ChapterWordCount[] = 
    book
    |> Array.map( fun c -> 
        { BookName = c.BookName; 
          Chapter  = c.ChapterName; 
          Count    = wordCount( c.ChapterData )})

let allChapterCounts = getChapterCounts( allLOTRBookData )
let forChapterCounts = getChapterCounts( fellowshipOfTheRingBookData )
let ttChapterCounts  = getChapterCounts( twoTowersBookData )
let rokChapterCounts = getChapterCounts( returnOfTheKingBookData )

(* Character Mentions *)

let lotrImportantCharacters = 
    [
        "Frodo";
        "Sam";
        "Pippin";
        "Merry";
        "Aragorn";
        "Legolas";
        "Gimli";
        "Gandalf";
        "Boromir"
        "Gollum";
        "Saruman";
        "Faramir";
    ]

let charactersToGetMentionsFor = [] // TODO: Fix the character list above.
let characterMentions ( character : string ) ( book : BookData[] ) = 
    let characterMentions = 
        splitWords( book ) 
        |> Array.filter( fun c -> c = character )
        |> Array.length
    { CharacterName = character; CharacterMentions = characterMentions }

let characterMentionsAllBooks = 
    charactersToGetMentionsFor
    |> List.map ( fun c -> characterMentions c allLOTRBookData )

let characterMentionsForFOR =
    charactersToGetMentionsFor
    |> List.map ( fun c -> characterMentions c fellowshipOfTheRingBookData )

let characterMentionsForTT =
    charactersToGetMentionsFor
    |> List.map ( fun c -> characterMentions c twoTowersBookData )

let characterMentionsForROK =
    charactersToGetMentionsFor
    |> List.map ( fun c -> characterMentions c returnOfTheKingBookData )

(* Sentiment Analysis *)


(*** include-it:chart ***)

(**
Summary
-------
An image is worth a thousand words:

![](http://imgs.xkcd.com/comics/hofstadter.png)
*)
