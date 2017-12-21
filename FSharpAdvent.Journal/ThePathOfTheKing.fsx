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

type ChapterWordCount  = { BookName : Book; 
                           Chapter : string; 
                           WordCount: int;  }

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

let cleanString ( input : string ) : string = 
    input.Replace("`", "")
         .Replace("'", "")
         .Replace("-", "")
         .Replace(";", "")
         .Replace("?", "")
         .Replace(":", "")
         .Replace("\"", "")
        
let splitValues = [| ' '; '.'; '-'; ','; '!'; '"'; |]
let splitDataPerChapter ( chapter : string ) : string[] = 
    chapter.Split( splitValues )
    |> Array.map( fun c -> cleanString( c )) 
    |> Array.filter( fun c -> c <> "'" && 
                              c <> "" && 
                              c <> "-" && 
                              c <> "\"" && 
                              c <> "-" && 
                              c <> "`" && 
                              c <> ";" && 
                              c <> "?" && 
                              c.Length <> 0 )


File.WriteAllLines( "C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\Hi.txt", splitWords( fellowshipOfTheRingBookData ))

let getWordCountForBook ( book : BookData[] ) : int = 
    book
    |> Array.collect( fun c -> splitDataPerChapter( c.ChapterData ))
    |> Array.length

let totalLOTRWordCount : int = getWordCountForBook allLOTRBookData 
let forWordCount       : int = getWordCountForBook fellowshipOfTheRingBookData
let ttWordCount        : int = getWordCountForBook twoTowersBookData
let rokWordCount       : int = getWordCountForBook returnOfTheKingBookData
let hobbitWordCount    : int = getWordCountForBook allHobbitBookData

let LOTRWordCountData   : string * int = "The Lord of the Rings", totalLOTRWordCount 
let FORWordCountData    : string * int = "The Fellowship of the Ring", forWordCount 
let TTWordCountData     : string * int = "The Two Towers", ttWordCount 
let ROKWordCountData    : string * int = "The Return of the King", rokWordCount 
let HobbitWordCountData : string * int = "The Hobbit", hobbitWordCount

let wordCounts : ( string * int ) list = 
    [
        LOTRWordCountData;
        FORWordCountData;
        TTWordCountData;
        ROKWordCountData;
        HobbitWordCountData;
    ]

let bookWordCountDataFrame : Frame< string, string > =
    wordCounts 
    |> List.map( fun ( book, count ) -> book, "Word Count", count )
    |> Frame.ofValues

bookWordCountDataFrame
|> Chart.Column
|> Chart.WithLegend true
|> Chart.WithTitle "The Lord of the Rings and Hobbit Books and Word Counts"
|> Chart.WithYTitle "Word Counts"

// Mean and Standard Deviation of 3 of the LOTR Books

let lotrBooksWordCounts : Frame< string, string > =
    bookWordCountDataFrame
    |> Frame.sliceRows [ "The Fellowship of the Ring"; 
                          "The Two Towers"; 
                          "The Return of the King" ]

let meanLOTRBookWordCounts : Series< string, float > =
    lotrBooksWordCounts 
    |> Stats.mean

let stdDeviationLOTRBookWordCounts : Series< string, float > =
    lotrBooksWordCounts 
    |> Stats.stdDev

let maxLOTRBookWordCounts : Series< string, float > = 
    lotrBooksWordCounts 
    |> Stats.max

let minLOTRBookWordCounts : Series< string, float > = 
    lotrBooksWordCounts 
    |> Stats.min

(* Chapter Data *)

let wordCountPerChapter ( chapter : string ) : int =
    splitDataPerChapter chapter
    |> Array.length

let getChapterWordCounts ( book : BookData[] ) : ChapterWordCount[] = 
    book
    |> Array.map( fun c -> 
        { BookName  = c.BookName; 
          Chapter   = c.ChapterName; 
          WordCount = wordCountPerChapter( c.ChapterData )})

let allLOTRChapterCounts : ChapterWordCount[] = getChapterWordCounts( allLOTRBookData )
let forChapterCounts     : ChapterWordCount[] = getChapterWordCounts( fellowshipOfTheRingBookData )
let ttChapterCounts      : ChapterWordCount[] = getChapterWordCounts( twoTowersBookData )
let rokChapterCounts     : ChapterWordCount[] = getChapterWordCounts( returnOfTheKingBookData )
let hobbitChapterCounts  : ChapterWordCount[] = getChapterWordCounts ( allHobbitBookData )

(* Character Mentions *)

type CharacterMentions = { CharacterName : string; 
                           CharacterMentions : int; }

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

let mostPopularCharacterInBook ( characterMentionsOfBook : CharacterMentions list ) 
                               : CharacterMentions =
    characterMentionsOfBook 
    |> List.maxBy ( fun c -> c.CharacterMentions )

let characterMentionsForFOR =
    lotrImportantCharacters
    |> List.map ( fun c -> characterMentions c fellowshipOfTheRingBookData )

let mostImportantCharacterInFOR : CharacterMentions

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

open RDotNet
open RProvider
open RProvider.``base``
open RProvider.utils

R.install_packages("RSentiment")

open RProvider.caret



(*** include-it:chart ***)

(**
Summary
-------
An image is worth a thousand words:

![](http://imgs.xkcd.com/comics/hofstadter.png)
*)
