﻿namespace Snowflaqe.Tasks

open System
open System.IO
open Snowflaqe
open Snowflaqe.Types
open Microsoft.Build.Utilities
open Microsoft.Build.Framework
open Program
open System.Text

type public GenerateGraphQLClient () =
    inherit Task ()

    [<Required>]
    member val public Platform : string = String.Empty with get, set

    [<Required>]
    member val public Configuration : string = String.Empty with get, set

    member val public EmitMetadata = false with get, set

    member val public ErrorType = { typeName = "ErrorType"; typeDefinition = CodeGen.defaultErrorType () } with get, set

    member val public OutputPath : string = String.Empty with get, set

    member val public Project = "GraphqlClient" with get, set

    member val public Queries = "" with get, set

    member val public Schema = "" with get, set

    member val public Serializer = SerializerType.System with get, set

    member val public AsyncType = AsyncReturnType.Async with get, set

    member val public Target = "fable" with get, set

    [<Output>]
    member val public GeneratedFiles = Array.empty<string> with get, set

    static member private TryParseTarget =
        function
        | "fable" -> Some OutputTarget.Fable
        | "fsharp" -> Some OutputTarget.FSharp
        | "shared" -> Some OutputTarget.Shared
        | _ -> None

    override this.Execute () =
        if String.IsNullOrEmpty this.Queries then
            raise (Exception "Queries must be not null or empty")

        if String.IsNullOrEmpty this.Schema then
            raise (Exception "Schema must be not null or empty")

        if String.IsNullOrEmpty this.Target then
            raise (Exception "Target must be fable, fsharp or shared")

        if String.IsNullOrEmpty this.Project then
            raise (Exception "Project must be not null or empty")

        let config =
            { schema = this.Schema
              serializer = this.Serializer
              asyncReturnType = this.AsyncType
              queries = this.Queries
              project = this.Project
              output =
                if String.IsNullOrEmpty (this.OutputPath) then
                    Path.Combine ("obj", this.Configuration, this.Platform, "Snowflaqe")
                else
                    this.OutputPath
              errorType = this.ErrorType
              target =
                (GenerateGraphQLClient.TryParseTarget (this.Target.ToLower ()))
                |> Option.defaultValue OutputTarget.Fable
              createProjectFile = false
              overrideClientName = None
              copyLocalLockFileAssemblies = None
              emitMetadata = this.EmitMetadata
              normalizeEnumCases = true
              generateAndRestoreTaskPackage = true }

        use buffer = new MemoryStream ()
        let writer = new StreamWriter (buffer)
        let prevOut = Console.Out

        try
            Console.SetOut (writer)

            let validationCode = runConfig config

            let executionCode =
                if validationCode = 0 then
                    match generate config with
                    | Error code -> code
                    | Ok files ->
                        this.GeneratedFiles <-
                            files
                            |> Seq.map (fun f ->
                                Path.Combine (
                                    config.output,
                                    Path.GetFileName (f.Replace (@"\", Path.DirectorySeparatorChar.ToString ()))
                                ))
                            |> Seq.toArray

                        this.GeneratedFiles |> Seq.iter (this.Log.LogMessage)
                        0
                else
                    validationCode

            writer.Flush ()
            buffer.Seek (0L, SeekOrigin.Begin) |> ignore
            let reader = new StreamReader (buffer)
            let mutable prevCh = Unchecked.defaultof<char>

            while not (reader.EndOfStream) do
                let line = reader.ReadLine ()

                let rec processLine ch =
                    match ch with
                    | '⏳' ->
                        prevCh <- ch
                        this.Log.LogMessage (Microsoft.Build.Framework.MessageImportance.High, line)
                    | '❌' ->
                        prevCh <- ch
                        this.Log.LogError (line)
                    | '✔' ->
                        prevCh <- ch
                        this.Log.LogMessage (Microsoft.Build.Framework.MessageImportance.High, line)
                    | ' ' -> processLine prevCh
                    | _ -> this.Log.LogMessage (Microsoft.Build.Framework.MessageImportance.High, line)

                if not (String.IsNullOrWhiteSpace line) then
                    processLine line.[0]

            executionCode = 0
        finally
            Console.SetOut (prevOut)
