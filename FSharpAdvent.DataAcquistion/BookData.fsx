#r @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\packages\Newtonsoft.Json.10.0.3\lib\net45\Newtonsoft.Json.dll"

open System
open System.IO

open Newtonsoft.Json

type Book = 
    | TheFellowshipOfTheRing
    | TheTwoTowers
    | TheReturnOfTheKing

type LotrData = { BookName    : Book; 
                  ChapterName : string; 
                  ChapterData : string }

type CharacterMentions = { CharacterName : string; CharacterMentions : int; }
type ChapterWordCount  = { BookName : Book; Chapter : string; Count : int;  }

(*
let charactersToGetMentionsFor = 
    [
        "Anborn"
        "Angbor"
        "Arod"
        "Arwen"
        "Asfaloth"
        "Angelica"
        "Dora" 
        "Frodo" 
        "Baranor"
        "Beechbone"
        "Beregond"
        "Bergil"
        "Bilbo" 
        "Bob"
        "Folco"
        "Fredegar" 
        "Bombadil"
        "Boromir"
        "Bruno"
        "Melilot"
        "Meriadoc"
        "Rorimac"
        "Bregalad"
        "Milo" 
        "Barliman" 

        "Celeborn"
        "Ceorl"
        "Bowman"
        "Carl"
        "Rosie"
        "Tolman"
        "Wilcome"
        "Círdan"

        "Damrod"
        "Denethor" 
        "Derufin"
        "Dervorin"
        "Duilin"
        "Duinhir"
        "Durin's Bane"
        "Dáin II Ironfoot"
        "Déagol"
        "Déorwine"
        "Dúnhere"

        "Elfhelm"
        "Elladan" 
        "Elrohir"
        "Elrond"
        "Éomer"
        "Éowyn"

       "Erestor"
       "Erkenbrand"

       "Faramir" 
       "Fastred" 
       "Bill Ferny"
       "Fimbrethil"
       "Finglas"
       "Firefoot"
       "Fladrif"
       "Forlong"

       "Galadriel"
       "Galdor"
       "Hamfast" 
       "Samwise"
       "Gamling"
       "Gandalf"
       "Ghân-buri-Ghân"
       "Gildor" 
       "Gimli"
       "Glorfindel"
       "Gléowine"
       "Glóin"
       "Golasgil"
       "Goldberry"
       "Gollum"
       "Gorbag"
       "Gothmog" 
       "Grimbeorn"
       "Grimbold"
       "Grishnákh"
       "Gwaihir"
       "Gárulf"

       "Halbarad"
       "Haldi"
       "Harding"
       "Hasufel"
       "Mat Heahertoes"
       "Herefar"
       "Herubra"
       "Hirluin"
       "Hob"
       "Hugo"
       "Hurin"
       "Háma"

       "Imrahil"
       "Ingold"
       "Ioreth"

       "Lagduf"
       "Landroval"
       "Legolas"
       "Lindir"
       "Lugdush"
       "Mablung the Ranger"
       "Farmer Maggot"
       "Mauhúr"
       "Meneldor"
       "Mouth of Sauron"
       "Muzgash"
       "Nob"
       "Old Man Willow"
       "Old Noakes"
       "Orophin"
       "Otho Sackville-Baggins"
       "Mrs. Proudfoot"
       "Odo Proudfoot"
       "Sancho Proudfoot"
       "Radagast"
       "Radbug"
       "Rúmil"
       "Lobelia Sackville-Baggins"
       "Lotho Sackville-Baggins"
       "Ted Sandyman"
       "Saruman"
       "Shagrat"
       "Shelob"
       "Robin Smallburrow"
       "Snaga"
       "Targon"
       "The King of the Dead"
       "Thranduil"
       "Théoden"
       "Théodred"
       "Adelar"
       "Everard"
       "Pippin"
       "Treebeard"
       "Daddy Twofoot"
       "Ufthak"
       "Uglúk"
       "Watcher in the Water"
       "Will Whitfoot"
       "Widow Rumble"
       "Willie Banks"
       "Windfola"
       "Witch-king of Angmar"
       "Gríma"
       "Wídfara"
       "Éothain"
    ]
*)

[<Literal>]
let file = @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\Data\LotrBook.txt"

let data = File.ReadAllText( file )
let split = data.Split('<')
let filtered = 
    split
    |> Array.filter( fun r -> r <> "" )

let getBookName ( input : string ) : Book = 
    match input with
    | " The Return Of The King "     -> TheReturnOfTheKing
    | " The Two Towers "             -> TheTwoTowers
    | " The Fellowship Of The Ring " -> TheFellowshipOfTheRing
    | _ -> failwith "Book not found!"

let allChapterData = 
    filtered
    |> Array.map( fun f -> 
        let split1 = f.Split('>')
        let split2 = split1.[ 0 ].Split('~')
        { BookName = getBookName( split2.[ 0 ] ); ChapterName = split2.[ 1 ]; ChapterData = split1.[ 1 ].Trim() })

let jsonizeAllChapterData : string = 
    JsonConvert.SerializeObject ( allChapterData, Formatting.Indented )

let bookDataJsonFile = @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\Data\LordOfTheRingsBook.json"

let saveJsonData = 
    File.WriteAllText ( bookDataJsonFile, jsonizeAllChapterData )


let fellowshipOfTheRing =
    allChapterData
    |> Array.filter( fun a -> a.BookName = TheFellowshipOfTheRing )

let twoTowers =
    allChapterData
    |> Array.filter( fun a -> a.BookName = TheTwoTowers )

let returnOfTheKing =
    allChapterData
    |> Array.filter( fun a -> a.BookName = TheReturnOfTheKing )

(* Persist the Book Data *)

let splitValues = [| ' '; '.'; '-'; ','; '!' |]

let splitWords( data : LotrData[] ) : string [] =
    let splitAndCleaned = 
        data 
        |> Array.collect( fun c -> 
            c.ChapterData.Trim().Split( splitValues ))
        |> Array.map( fun c -> c.Replace("`", "")
                                .Replace("'", "")
                                .Replace("-", "")
                                .Replace(";", "")
                                .Replace("?", "")
                                .Replace("'", ""))
        |> Array.filter( fun c -> c <> "'" && c <> "" && c <> "-" && c.Length <> 0 )
    //File.AppendAllLines( __SOURCE_DIRECTORY__ + "/words.txt", splitAndCleaned)

    splitAndCleaned

(* Word Counts *)

let words ( lines : string ) : string[] = 
    let cleanedLines = 
        lines.Split( splitValues )
        |> Array.filter( fun c -> 
            c <> "'" && c <> "" && c <> "-" && c <> " " && c.Length <> 0 )
    cleanedLines

let wordCount ( lines : string ) : int =
    //printfn "%A" cleanedLines
    //File.AppendAllLines( __SOURCE_DIRECTORY__ + "/words.txt", cleanedLines )
    Array.length ( words( lines )) 

let totalWordCount = 
    splitWords( allChapterData )
    |> Array.length

let forWordCount = 
    splitWords( fellowshipOfTheRing )
    |> Array.length

let ttWordCount = 
    splitWords( twoTowers )
    |> Array.length

let rokWordCount = 
    splitWords( returnOfTheKing )
    |> Array.length

(* Unique Words *)

let uniqueWords ( data : LotrData[] ) = 
    splitWords( data )
    |> Array.distinct

let uniqueWordCount ( data : LotrData[] ) = 
    uniqueWords( data )
    |> Array.length

let allUniqueWordCount = uniqueWordCount( allChapterData )
let forUniqueWordCount = uniqueWordCount( fellowshipOfTheRing )
let ttUniqueCount      = uniqueWordCount( twoTowers ) 
let rokUniqueCount     = uniqueWordCount( returnOfTheKing )

(* Word Counts Per Chapter *)

let getChapterCounts ( book : LotrData[] ) : ChapterWordCount[] = 
    book
    |> Array.map( fun c -> 
        { BookName = c.BookName; 
          Chapter  = c.ChapterName; 
          Count    = wordCount( c.ChapterData )})

let allChapterCounts = getChapterCounts( allChapterData )
let forChapterCounts = getChapterCounts( fellowshipOfTheRing )
let ttChapterCounts  = getChapterCounts( twoTowers )
let rokChapterCounts = getChapterCounts( returnOfTheKing )

(* Unique Word Counts Per Chapter *)

let getUniqueWordsPerChapter( book : LotrData[] ) : ChapterWordCount[] =
    book
    |> Array.map( fun c -> 
        let allWords = words( c.ChapterData ) 
        let uniqueWordCount =
            allWords
            |> Array.distinct
            |> Array.length

        { BookName = c.BookName; 
          Chapter  = c.ChapterName; 
          Count    = uniqueWordCount })

let allUniqueWordsPerChapter = getUniqueWordsPerChapter( allChapterData )
let forUniqueWordsPerChapter = getUniqueWordsPerChapter( fellowshipOfTheRing )
let ttUniqueWordsPerChapter  = getUniqueWordsPerChapter( twoTowers )
let rokUniqueWordsPerChapter = getUniqueWordsPerChapter( returnOfTheKing )

(* Character Mentions *)
let charactersToGetMentionsFor = [] // TODO: Fix the character list above.
let characterMentions ( character : string ) ( book : LotrData[] )= 
    let characterMentions = 
        splitWords( book ) 
        |> Array.filter( fun c -> c = character )
        |> Array.length
    { CharacterName = character; CharacterMentions = characterMentions }

let characterMentionsAllBooks = 
    charactersToGetMentionsFor
    |> List.map ( fun c -> characterMentions c allChapterData )

let characterMentionsForFOR =
    charactersToGetMentionsFor
    |> List.map ( fun c -> characterMentions c fellowshipOfTheRing )

let characterMentionsForTT =
    charactersToGetMentionsFor
    |> List.map ( fun c -> characterMentions c twoTowers )

let characterMentionsForROK =
    charactersToGetMentionsFor
    |> List.map ( fun c -> characterMentions c returnOfTheKing )
