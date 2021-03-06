﻿namespace Common

module Serialization =

    open System.Web.Http
    open Newtonsoft.Json
    open System.Net.Http.Headers
    open Serilog
    open JsonConverters

    let configure (config: HttpConfiguration) =
        config.Formatters.XmlFormatter.UseXmlSerializer <- true

        config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(OptionConverter())

        // The following lines are for controlling the serialisation of F# properties from Records and Unions
        config.Formatters.JsonFormatter.SerializerSettings.ContractResolver <- Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver ()
        config.Formatters.JsonFormatter.SerializerSettings.DateFormatHandling <- DateFormatHandling.IsoDateFormat
        config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling <- DateTimeZoneHandling.Utc
        config.Formatters.JsonFormatter.SerializerSettings.MissingMemberHandling <- MissingMemberHandling.Error
        config.Formatters.JsonFormatter.SerializerSettings.Error <- new System.EventHandler<Serialization.ErrorEventArgs>(fun _ errorEvent ->
            let context = System.Web.HttpContext.Current
            let error = errorEvent.ErrorContext.Error
            context.AddError(error)
            errorEvent.ErrorContext.Handled <- true)
        config.Formatters.JsonFormatter.SupportedMediaTypes.Add(MediaTypeHeaderValue("text/html"))

        Log.Information("Configured JSON serialisation")

        config