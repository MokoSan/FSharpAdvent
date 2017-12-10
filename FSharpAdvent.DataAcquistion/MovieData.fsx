#r @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\packages\FSharp.Data.2.4.3\lib\net45\FSharp.Data.dll"

open System
open FSharp.Data

[<Literal>]
let fellowshipOfTheRingWiki  = @"https://en.wikipedia.org/wiki/The_Lord_of_the_Rings:_The_Fellowship_of_the_Ring"
[<Literal>]
let twoTowersWiki            = @"https://en.wikipedia.org/wiki/The_Lord_of_the_Rings:_The_Two_Towers"
[<Literal>]
let returnOfTheKingWiki      = @"https://en.wikipedia.org/wiki/The_Lord_of_the_Rings:_The_Return_of_the_King"

(* Movie Domain Model *)
type Movie =
    { Name             : string; 
      RuntimeInMinutes : int; 
      BudgetInMillions : int; 
      BoxOfficeRevenue : float;
      }

(* Entire Series - Overall Numbers *)

[<Literal>]
let lotrFilmSeries           = @"https://en.wikipedia.org/wiki/The_Lord_of_the_Rings_(film_series)"

type LotrFilmSeriesProvider  = HtmlProvider< lotrFilmSeries >
let lotrFilmSeriesProvider   = LotrFilmSeriesProvider.Load( lotrFilmSeries )

(** Table of Budget, Box Office and Running Time **)
let overallFirstTablesRows            = lotrFilmSeriesProvider.Tables.Table1.Rows

// Running Time
let overallRunningTime                = overallFirstTablesRows.[ 12 ].``The Lord of the Rings 2``  
let overallRunningTimeInMinutes       = int( overallRunningTime.Split(' ').[ 0 ] )

// Budget
let overallBudget                     = overallFirstTablesRows.[ 15 ].``The Lord of the Rings 2``
let overallBudgetInMillions           = int ( overallBudget.Split(' ').[ 0 ].Replace( "$", "" ))

// Box Office Revenue
let overallBoxOfficeRevenue           = overallFirstTablesRows.[ 16 ].``The Lord of the Rings 2``
let overallBoxOfficeRevenueInMillions = float( overallBoxOfficeRevenue.Split(' ').[ 0 ].Replace( "$", "" )) * 1000.

// List of Academy Award Nominations vs. Wins 

// Fellowship of the Ring
let forAcademyAwardsData  = lotrFilmSeriesProvider.Lists.``Academy Awards``.Values.[ 0 ]
let forNominationsAndWins = forAcademyAwardsData.Split('-').[ 1 ].Split(':') // Urghh, couldn't generalize the algorithm.. :(
let forNominations        = int ( forNominationsAndWins.[ 1 ].Split(',').[ 0 ].Replace( " ", "" ))
let forWins               = int ( forNominationsAndWins.[ 2 ].Replace( " ", "" ))

let getNominationData ( splitStrings : string[] ) : int =
    int ( splitStrings.[ 1 ].Split(',').[ 0 ].Replace( " ", "" ))

let getWinsData( splitStrings : string[] ) : int = 
    int ( splitStrings.[ 2 ].Replace( " ", "" ))

// The Two Towers
let ttAcademyAwardsData   = lotrFilmSeriesProvider.Lists.``Academy Awards``.Values.[ 1 ]
let ttNominationsAndWins  = ttAcademyAwardsData.Split('—').[ 1 ].Split(':')
let ttNominations         = getNominationData( ttNominationsAndWins ) 
let ttWins                = getWinsData( ttNominationsAndWins ) 

// The Return of the King
let rokAcademyAwardsData  = lotrFilmSeriesProvider.Lists.``Academy Awards``.Values.[ 2 ]
let rokNominationsAndWins = rokAcademyAwardsData.Split('—').[ 1 ].Split(':')
let rokNominations        = getNominationData( rokNominationsAndWins )
let rokWins               = getWinsData( rokNominationsAndWins )

// Overall
let overallNominations    = forNominations + ttNominations + rokNominations
let overallWins           = forWins        + ttWins        + rokWins 