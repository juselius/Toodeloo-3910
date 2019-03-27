namespace Shared

open System

type Todo = { 
    title       : string
    description : string 
    priority    : int
    due         : DateTime option
    }
