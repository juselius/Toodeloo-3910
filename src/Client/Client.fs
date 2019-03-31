module Toodeloo.Client

open Elmish
open Elmish.React
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Elmish.Debug
open Elmish.HMR
open Fable.PowerPack.Fetch
open Thoth.Json
open Fulma

open Toodeloo.Model
open Toodeloo.View

let init () : Model * Cmd<Msg> =
    loadEntries Defaults.defaultModel

let update (msg : Msg) (model : Model) : Model * Cmd<Msg> = 
    match msg with
    | NewEntry msg -> handleNewEntry msg model
    | SaveEntry n -> saveEntry n model 
    | EditEntry msg -> handleEditEntry msg model 
    | StartEdit id -> startEdit id model 
    | SaveEdit -> saveEdit model
    | CancelEdit -> cancelEdit model 
    | DeleteEntry n -> deleteEntry n model 
    | NotifyError err -> { model with errorMsg = Some err }, Cmd.none
    | ClearError -> { model with errorMsg = None }, Cmd.none
    | ToggleInfoPane -> 
        { model with showInfoPane = not model.showInfoPane }, Cmd.none
    | EntrySaved _ -> model, Cmd.none // fix
    | EntriesLoadedOk entries -> initEntries model (entries |> Ok)
    | EntriesLoadedErr entries -> initEntries model (entries |> Error)

let view model dispatch =
    mainView model dispatch [
        Box.box' [] [
            Heading.h3 [] [ str "My Toodeloo" ]
            newEntryForm model dispatch
        ]
        Box.box' [] [ taskListView model dispatch ]
        (if model.showInfoPane then
            div [
                Props.OnClick (fun _ -> dispatch ToggleInfoPane)
            ] [ infoPane model dispatch ]
        else
            Button.button [
                Button.OnClick (fun _ -> dispatch ToggleInfoPane)
                Button.Props [ 
                    Style [Props.CSSProp.BackgroundColor "orange"] 
                ]
            ] [ str "Developer info" ]
        )
    ]

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
