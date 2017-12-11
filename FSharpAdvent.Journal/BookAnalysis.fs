module LotrMovie

open FSharp.Data

open System
open System.IO

type Book = 
    | TheFellowshipOfTheRing
    | TheTwoTowers
    | TheReturnOfTheKing

type LotrData = { BookName    : Book; 
                  ChapterName : string; 
                  ChapterData : string }

[<Literal>]
let file = @"/Users/mukundraghavsharma/Desktop/F#/FSharpAdvent/Data/LotrBook.txt"

let data = File.ReadAllText( file )
let split = data.Split('<')
let filtered = 
    split
    |> Array.filter( fun r -> r <> "" )

let getBookName ( input : string ) : Book = 
    match input with
    | " The Return Of The King " -> TheReturnOfTheKing
    | " The Two Towers " -> TheTwoTowers
    | " The Fellowship Of The Ring " -> TheFellowshipOfTheRing
    | _ -> failwith "Book not found!"

(* Overall Book Analysis *)
let allChapterData = 
    filtered
    |> Array.map( fun f -> 
        let split1 = f.Split('>')
        let split2 = split1.[ 0 ].Split('~')
        { BookName = getBookName( split2.[ 0 ] ); ChapterName = split2.[ 1 ]; ChapterData = split1.[ 1 ] })

let wordCount ( lines : string ) : int =
    Array.length ( lines.Split(' '))

let totalWordCount = 
    Array.fold( fun acc r -> acc + wordCount( r.ChapterData )) 0 allChapterData 

let uniqueWords ( data : LotrData[] ) = 
    let justChapterData = 
        data 
        |> Array.map( fun c -> c.ChapterData )
        |> String.concat ""
    let trimmedData = justChapterData.Trim()
    trimmedData.Split(' ')
    |> Array.distinct

let uniqueWordCount ( data : LotrData[] ) = 
    uniqueWords( data )
    |> Array.length

let allUniqueWordCount = uniqueWordCount( allChapterData )

let fellowshipOfTheRing =
    allChapterData
    |> Array.filter( fun a -> a.BookName = TheFellowshipOfTheRing )

let forWordCount = 
    Array.fold ( fun acc r -> acc + wordCount( r.ChapterData )) 0 fellowshipOfTheRing

let forUniqueWordCount = uniqueWordCount( fellowshipOfTheRing )

let twoTowers =
    allChapterData
    |> Array.filter( fun a -> a.BookName = TheTwoTowers )

let ttWordCount = 
    Array.fold ( fun acc r -> acc + wordCount( r.ChapterData )) 0 twoTowers 

let ttUniqueCount = uniqueWordCount( twoTowers ) 

let returnOfTheKing =
    allChapterData
    |> Array.filter( fun a -> a.BookName = TheReturnOfTheKing )

let rokWordCount = 
    Array.fold ( fun acc r -> acc + wordCount( r.ChapterData )) 0 returnOfTheKing 

let rokUniqueCount = uniqueWordCount( returnOfTheKing )