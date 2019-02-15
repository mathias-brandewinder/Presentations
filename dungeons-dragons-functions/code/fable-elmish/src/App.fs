namespace Demo

module App =

    open Elmish
    open Elmish.React
    open Fable.Helpers.React
    open Fable.Helpers.React.Props

    open Domain

    // MODEL
    
    type Model = Domain.Position

    type Msg = Domain.Action

    let init () = Domain.initial

    // UPDATE

    let update (msg: Msg) (model: Model) =
        Domain.update msg model 

    // VIEW (rendered with React)

    let view (model: Model) dispatch =

        div []
            [ 
                div [] [ str (string model) ]

                div [] [ button [ OnClick (fun _ -> dispatch (Move N)) ] [ str "N" ]]
                div [] [ button [ OnClick (fun _ -> dispatch (Move W)) ] [ str "West" ]]
                div [] [ button [ OnClick (fun _ -> dispatch (Move S)) ] [ str "South" ]]
                div [] [ button [ OnClick (fun _ -> dispatch (Move E)) ] [ str "East" ]]
            ]

    // App
    Program.mkSimple init update view
    |> Program.withReact "elmish-app"
    |> Program.withConsoleTrace
    |> Program.run
