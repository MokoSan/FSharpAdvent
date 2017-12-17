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
let lotrBookFilePath = __SOURCE_DIRECTORY__+  @"\..\Data\LordOfTheRingsBook.json"

[<Literal>]
let hobbitBookFilePath = __SOURCE_DIRECTORY__ + @"\..\Data\HobbitBook.json"

let allLOTRText       = File.ReadAllText lotrBookFilePath
let allLOTRBookData   = JsonConvert.DeserializeObject< BookData[] >( allLOTRText ) 

let allHobbitText     = File.ReadAllText hobbitBookFilePath
let allHobbitBookData = JsonConvert.DeserializeObject< BookData[] >( allHobbitText ) 

(* Book Based Data *)

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

let splitValues = [| ' '; '.'; '-'; ','; '!'; '\''; '"'; |]
let splitWords( data : BookData[] ) : string [] =

    let cleanString ( input : string ) : string = 
        input.Replace("`", "")
             .Replace("'", "")
             .Replace("-", "")
             .Replace(";", "")
             .Replace("?", "")
             .Replace("\"", "")
        
    data 
    |> Array.collect( fun c -> c.ChapterData.Trim().Split( splitValues ))
    |> Array.map( fun c -> cleanString( c )) 
    |> Array.filter( fun c -> c <> "'" && 
                              c <> "" && 
                              c <> "-" && 
                              c <> "\"" && 
                              c <> "-" && 
                              c.Length <> 0 )

let totalLOTRWordCount : int = 
    splitWords( allLOTRBookData )
    |> Array.length

let forWordCount : int = 
    splitWords( fellowshipOfTheRingBookData )
    |> Array.length

let ttWordCount : int = 
    splitWords( twoTowersBookData )
    |> Array.length

let rokWordCount : int = 
    splitWords( returnOfTheKingBookData )
    |> Array.length

let hobbitWordCount : int = 
    splitWords( allHobbitBookData )
    |> Array.length

(* Chapter Data *)

let wordsPerChapter ( wordsFromAChapter : string ) : string[] = 
    let cleanedLines = 
        wordsFromAChapter.Split( splitValues )
        |> Array.filter( fun c -> 
            c <> "'" && c <> "" && c <> "-" && c <> " " && c.Length <> 0 )
    cleanedLines

let wordCount ( chapter : string ) : int =
    Array.length ( wordsPerChapter( chapter )) 

type ChapterWordCount  = { BookName : Book; 
                           Chapter : string; 
                           WordCount: int;  }

let getChapterWordCounts ( book : BookData[] ) : ChapterWordCount[] = 
    book
    |> Array.map( fun c -> 
        { BookName  = c.BookName; 
          Chapter   = c.ChapterName; 
          WordCount = wordCount( c.ChapterData )})

let allLOTRChapterCounts = getChapterWordCounts( allLOTRBookData )
let forChapterCounts     = getChapterWordCounts( fellowshipOfTheRingBookData )
let ttChapterCounts      = getChapterWordCounts( twoTowersBookData )
let rokChapterCounts     = getChapterWordCounts( returnOfTheKingBookData )

let hobbitChapterCounts  = getChapterWordCounts ( allHobbitBookData )

(* Character Mentions *)

type CharacterMentions = { CharacterName : string; CharacterMentions : int; }

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

let characterMentions ( character : string ) ( book : BookData[] ) = 
    let characterMentions : int = 
        splitWords( book ) 
        |> Array.filter( fun c -> c = character )
        |> Array.length
    { CharacterName = character; CharacterMentions = characterMentions }

let characterMentionsAllLOTRBooks = 
    lotrImportantCharacters
    |> List.map ( fun c -> characterMentions c allLOTRBookData )

let characterMentionsForFOR =
    lotrImportantCharacters
    |> List.map ( fun c -> characterMentions c fellowshipOfTheRingBookData )

let characterMentionsForTT =
    lotrImportantCharacters
    |> List.map ( fun c -> characterMentions c twoTowersBookData )

let characterMentionsForROK =
    lotrImportantCharacters
    |> List.map ( fun c -> characterMentions c returnOfTheKingBookData )

let hobbitImportantCharacters = 
    [
        "Bilbo";
        "Gandalf";
        "Thorin";
        "Dwalin"; 
        "Balin" 
        "Kili";
        "Fili"; 
        "Dori"; 
        "Nori"; 
        "Ori"; 
        "Oin"; 
        "Gloin"; 
        "Bifur"; 
        "Bofur"; 
        "Bombur"
    ]

let characterMentionsForHobbit = 
    hobbitImportantCharacters 
    |> List.map ( fun c -> characterMentions c allHobbitBookData )

(* Chapter Based Character Interaction *)

(* Sentiment Analysis *)


(*** include-it:chart ***)

(**
Summary
-------
An image is worth a thousand words:

![](http://imgs.xkcd.com/comics/hofstadter.png)
*)
