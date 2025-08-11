Public Class AppHelper
    Public Shared CS As String

    Public Shared Function dateConvert(ByVal formatedstring) As DateTime
        Dim dy As Integer = Left(formatedstring, 2)
        Dim mo As Integer = Mid(formatedstring, 4, 2)
        Dim yr As Integer = Mid(formatedstring, 7, 4)

        Return New Date(yr, mo, dy)
    End Function

    Public Shared Function timeConvert(ByVal formatedstring) As TimeSpan
        Dim hr As Integer = Left(formatedstring, 2)
        Dim mi As Integer = Mid(formatedstring, 4, 2)
        Dim se As Integer = Mid(formatedstring, 7, 4)

        Return New TimeSpan(hr, mi, se)
    End Function

    Public Shared Function dateTimeConvert(ByVal formatedstring) As DateTime
        Dim dy As Integer = Left(formatedstring, 2)
        Dim mo As Integer = Mid(formatedstring, 4, 2)
        Dim yr As Integer = Mid(formatedstring, 7, 4)
        Dim hr As Integer = Mid(formatedstring, 12, 2)
        Dim mi As Integer = Mid(formatedstring, 15, 2)
        Dim sc As Integer = Mid(formatedstring, 18, 2)

        Return New Date(yr, mo, dy, hr, mi, sc)
    End Function

    Public Shared Function checkDate(ByVal formatedstring) As Boolean
        Try
            Dim dy As Integer = Left(formatedstring, 2)
            Dim mo As Integer = Mid(formatedstring, 4, 2)
            Dim yr As Integer = Mid(formatedstring, 7, 4)
            Dim dt As New Date(yr, mo, dy)
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

End Class

Public Class DecimalModelBinder
    Implements IModelBinder

    Public Function BindModel(controllerContext As ControllerContext, bindingContext As ModelBindingContext) As Object Implements IModelBinder.BindModel
        Dim valueResult As ValueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName)
        Dim modelState As New ModelState() With {.Value = valueResult}
        Dim actualValue As Object = Nothing
        Try
            If valueResult.AttemptedValue = "" Then
                actualValue = CDec(0)
            Else
                actualValue = CDec(valueResult.AttemptedValue)
            End If
        Catch e As FormatException
            modelState.Errors.Add(e)
        End Try
        bindingContext.ModelState.Add(bindingContext.ModelName, modelState)
        Return actualValue
    End Function
End Class
