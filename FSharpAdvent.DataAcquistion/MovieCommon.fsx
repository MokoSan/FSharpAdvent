open System.IO

(* Movie Domain Model - Here is what I want. ^_^ *)
type MovieInfo = 
    { Name                       : string; 
      RuntimeInMinutes           : int; 
      BudgetInMillions           : int; 
      BoxOfficeRevenueInMillions : float;
      AcademyAwardNominations    : int;
      AcademyAwardWins           : int; 
      RottenTomatoesScore        : float; }

      static member ToCsv( instance : MovieInfo ) : string =
        sprintf "%A,%A,%A,%A,%A,%A,%A\n"  instance.Name instance.RuntimeInMinutes instance.BudgetInMillions instance.BoxOfficeRevenueInMillions instance.AcademyAwardNominations instance.AcademyAwardWins instance.RottenTomatoesScore
         
[<Literal>]
let movieOutputFile = @"C:\Users\MukundRaghavSharma\Desktop\F#\FSharpAdvent\Data\Movies.csv"

let appendHeaders = 
    File.AppendAllText( movieOutputFile , "Name,RuntimeInMinutes,BudgetInMillions,BoxOfficeRevenueInMillions,AcademyAwardNominations,AcademyAwardWins,RottenTomatoesScore\n" )

let appendCsvLineToMovieOutput ( allMoviesCsv : string list ) : unit =
    allMoviesCsv
    |> List.iter( fun a -> File.AppendAllText( movieOutputFile, a ))