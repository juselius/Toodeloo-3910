module Toodeloo.Model

open Elmish
open System

type Todo = { 
      title       : string
      description : string 
      priority    : int
      due         : DateTime option
    }

type Model = { 
    entries    : Map<int, Todo>
    createForm : Todo
    editForm   : (int * Todo) option
    errorMsg   : string option
    currentId  : int
    showInfoPane : bool
    }

// local submodule
module Defaults =
    let defaultTodo = { 
        title = ""
        description = ""
        priority = 0
        due = None
        }

    let defaultModel = {
        entries = Map.empty
        createForm = defaultTodo
        errorMsg = None
        editForm = None
        currentId = 0
        showInfoPane = false
        }


// Example how to split Msg into submessages
type UpdateEntryMsg =
| UpdatePri of int
| UpdateTitle of string
| UpdateDescription of string
| UpdateDue of System.DateTime option

type NewEntryMsg = UpdateEntryMsg

type EditEntryMsg = UpdateEntryMsg

type Msg =
| NewEntry of NewEntryMsg
| SaveEntry of Todo
| DeleteEntry of int
| StartEdit of int
| EditEntry of EditEntryMsg 
| SaveEdit 
| CancelEdit 
| NotifyError of string
| ClearError
| ToggleInfoPane

let private notifyErr e = Cmd.ofMsg (Msg.NotifyError e)

let handleNewEntry (msg : NewEntryMsg) (model : Model) =
    let entry = 
        match msg with
        | UpdatePri y -> { model.createForm with priority = y }
        | UpdateDue y ->  { model.createForm with due = y }
        | UpdateTitle y -> { model.createForm with title = y }
        | UpdateDescription y -> { model.createForm with description = y }
    { model with createForm = entry }, Cmd.none

let saveEntry (x : Todo) (model : Model) =
    let newId = model.currentId + 1
    // Example validation, perform any kind of validation here and return
    let todo' = model.entries |> Map.add newId x 
    let model' = { 
        model with 
            entries = todo' 
            currentId = newId
            createForm = Defaults.defaultTodo
        }
    model', Cmd.none

let deleteEntry (x : int) (model : Model) =
    let model' = { model with entries = Map.remove x model.entries }
    model', Cmd.none

let private updateEntry (msg : EditEntryMsg) (entry : Todo) =
    match msg with
    | UpdateTitle t -> { entry with title = t}
    | UpdateDescription t -> { entry with description = t}
    | UpdatePri p -> { entry with priority = p}
    | UpdateDue d -> { entry with due = d}

let startEdit id model =
    match Map.tryFind id model.entries with
    | Some entry -> 
        let model' = { model with editForm = Some (id, entry) }
        model', Cmd.none
    | None -> model, notifyErr <| "TaskId not found, " + string id

let handleEditEntry (msg : UpdateEntryMsg) (model : Model) =
    match model.editForm with
    | Some (id, entry) -> 
        let entry' = updateEntry msg entry
        let model' = { model with editForm = Some (id, entry') }
        model', Cmd.none
    | None -> model, notifyErr "Error in error message"

let saveEdit model =
    match model.editForm with
    | Some (id, entry) -> 
        let model' = {
            model with 
                entries = Map.add id entry model.entries
                editForm = None 
            }
        model', Cmd.none
    | None -> model, notifyErr "Error in error message"

let cancelEdit model =
    let model' = { model with editForm = None }
    model', Cmd.none