namespace Toodleloo.Tests
open OpenQA.Selenium.Chrome
open canopy.types

module UITests =
    open canopy.runner.classic
    open canopy.configuration
    open canopy.classic

    let testClient () =
        // NixOS hack, comment out on other systems
        let homeDir = System.Environment.GetEnvironmentVariable "HOME"
        chromeDir <- homeDir + "/.local/bin"

        start BrowserStartMode.Chrome //ChromeHeadless

        //this is how you define a test
        "taking canopy for a spin" &&& fun _ ->
            url "http://localhost:8080"
            "#title" == ""
            "#title" << "Canopy test"
            "#desc" << "Added by Canopy"
            click "#save"
            "#title_view" == "Canopy test"
        run ()
        Async.Sleep 1000 |> Async.RunSynchronously
        quit ()