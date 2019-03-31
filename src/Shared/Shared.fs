module Shared

open System

type Todo = { 
    title       : string
    description : string 
    priority    : int
    due         : DateTime option
    }

let defaultTodo = { 
    title = ""
    description = ""
    priority = 0
    due = None
    }

