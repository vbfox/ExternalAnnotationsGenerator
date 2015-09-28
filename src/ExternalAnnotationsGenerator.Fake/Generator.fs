namespace ExternalAnnotationsGenerator

module Generator =

    open Fake
    open System
    open System.Text

    type GeneratorArgs = {
        ToolPath: string
        Version: string option
        OutDir: string option
    }

    let private DefaultArgs = {
        ToolPath = "Generator.exe"
        Version = None
        OutDir = None
    }

    let private buildArgString (args: GeneratorArgs) =
        let builder = StringBuilder ()

        if args.Version.IsSome then
            builder.AppendFormat(" -v \"{0}\"", args.Version.Value) |> ignore

        if args.OutDir.IsSome then
            builder.AppendFormat(" -d \"{0}\"", args.OutDir.Value) |> ignore

        builder.ToString()

    let generateAnnotations (argBuilder: GeneratorArgs -> GeneratorArgs) =
        let args = argBuilder DefaultArgs
        let argString = buildArgString args

        let conf (startInfo:Diagnostics.ProcessStartInfo) = 
            startInfo.FileName <- args.ToolPath
            startInfo.Arguments <- argString

        let result = ExecProcess conf (TimeSpan.FromMinutes(1.0))
        if result <> 0 then failwith "Generation failed"
        ()
