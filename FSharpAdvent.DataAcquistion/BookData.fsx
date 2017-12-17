#r @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\packages\Newtonsoft.Json.10.0.3\lib\net45\Newtonsoft.Json.dll"

open System
open System.IO

open Newtonsoft.Json

type Book = 
    | TheFellowshipOfTheRing
    | TheTwoTowers
    | TheReturnOfTheKing
    | TheHobbit

type BookData = { BookName    : Book; 
                  ChapterName : string; 
                  ChapterData : string }

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
       "Robin Smallburrow"
       "Snaga"
       "Targon"
       "Théoden"
       "Théodred"
       "Pippin"
       "Treebeard"
       "Witch-king of Angmar"
       "Gríma"
    ]
*)

[<Literal>]
let lotrBookTxt = @"..\Data\LotrBook.txt"
[<Literal>]
let lotrJsonFile = @"..\Data\LordOfTheRingsBook.json"

[<Literal>]
let hobbitBookTxt = @"..\Data\Hobbit.txt"
[<Literal>]
let hobbitJsonFile = @"..\Data\Hobbit.json"

(* Persist the Books Data Into JSON Form *)

let lotrTextData : string = 
    File.ReadAllText( lotrBookTxt )

let hobbitTextData : string = 
    File.ReadAllText( hobbitBookTxt )

let splitAndFilteredBookData ( bookTextData : string ) : string[] = 
     bookTextData.Split('<')
     |> Array.filter( fun r -> r <> "" )

let getBookName ( input : string ) : Book = 
    match input with
    | " The Return Of The King "     -> TheReturnOfTheKing
    | " The Two Towers "             -> TheTwoTowers
    | " The Fellowship Of The Ring " -> TheFellowshipOfTheRing
    | " The Hobbit "                 -> TheHobbit
    | _ -> failwith "Book not found!"

let allChapterData ( bookTextData : string ) : BookData[] = 
    splitAndFilteredBookData bookTextData 
    |> Array.map( fun f -> 
        let split1 = f.Split('>')
        let split2 = split1.[ 0 ].Split('~')
        { BookName = getBookName( split2.[ 0 ] ); ChapterName = split2.[ 1 ]; ChapterData = split1.[ 1 ].Trim() })

let saveJsonData ( jsonFilePath : string ) ( chapterData : BookData[] ) : unit = 
    let jsonizeAllChapterData ( allBookData : BookData[] ) : string = 
        JsonConvert.SerializeObject ( allBookData, Formatting.Indented )

    File.WriteAllText ( jsonFilePath, jsonizeAllChapterData( chapterData ))

let writeAllHobbitData : unit = 
    let allHobbitChapterData = allChapterData( hobbitTextData )
    saveJsonData hobbitJsonFile allHobbitChapterData |> ignore