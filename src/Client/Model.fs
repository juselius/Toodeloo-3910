module Toodeloo.Model

open Elmish
open System

type TaskId = int

type Todo = 
    { taskId   : TaskId
      priority : int
      task     : string
      due      : DateTime 
    }

type Model =
    { entries    : Map<int, Todo>
      createForm : Todo
      editForm   : Todo option
      errorMsg   : string option
      currentId  : int
      showInfoPane : bool
    }

// local submodule
module Defaults =
    let defaultTodo = 
        { taskId = 0
          priority = 0
          task = ""
          due = DateTime.Today.AddDays 1.0
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
| UpdateTask of string
| UpdateDue of System.DateTime

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
        | UpdateTask y -> { model.createForm with task = y }
    { model with createForm = entry }, Cmd.none

let saveEntry (x : Todo) (model : Model) =
    let newId = model.currentId + 1
    // Example validation, perform any kind of validation here and return
    match x.taskId with
    | 0 -> 
        let todo' = model.entries |> Map.add newId { x with taskId = newId } 
        let model' = { 
            model with 
                entries = todo' 
                currentId = newId
                createForm = Defaults.defaultTodo
            }
        model', Cmd.none
    | _ -> model, notifyErr "What, what?"

let deleteEntry (x : int) (model : Model) =
    let model' = { model with entries = Map.remove x model.entries }
    model', Cmd.none

let private updateEntry (msg : EditEntryMsg) (entry : Todo) =
    match msg with
    | UpdateTask t -> { entry with task = t}
    | UpdatePri p -> { entry with priority = p}
    | UpdateDue d -> { entry with due = d}

let startEdit id model =
    match Map.tryFind id model.entries with
    | Some entry -> 
        let model' = { model with editForm = Some entry }
        model', Cmd.none
    | None -> model, notifyErr <| "TaskId not found, " + string id

let handleEditEntry (msg : UpdateEntryMsg) (model : Model) =
    match model.editForm with
    | Some entry -> 
        let entry' = updateEntry msg entry
        let model' = { model with editForm = Some entry' }
        model', Cmd.none
    | None -> model, notifyErr "Error in error message"

let saveEdit model =
    match model.editForm with
    Some entry -> 
        let model' = {
            model with 
                entries = Map.add entry.taskId entry model.entries
                editForm = None 
            }
        model', Cmd.none
    | None -> model, notifyErr "Error in error message"

let cancelEdit model =
    let model' = { model with editForm = None }
    model', Cmd.none