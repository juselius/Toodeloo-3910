namespace Toodleloo.Tests
open OpenQA.Selenium.Chrome
open canopy.types

module UITests =
    open canopy.runner.classic
    open canopy.configuration
    open canopy.classic
    open Expecto

    // [<Tests>]
    let testClient () =
        let homeDir = System.Environment.GetEnvironmentVariable "HOME"
        chromeDir <- homeDir + "/.local/bin"

        start BrowserStartMode.Chrome //Headless

        //this is how you define a test
        "taking canopy for a spin" &&& fun _ ->
            url "http://localhost:8080"

            "#title" == ""
            "#title" << "Canopy test"
            "#desc" << "Added by Canopy"
            click "#save"
            "#title_view" == "Canopy test"
        run ()

        printfn "press [enter] to exit the browser"
        System.Console.ReadLine () |> ignore

        quit ()