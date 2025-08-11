Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class ExtractDataManualViewModel
    <DisplayName("Date")> _
    <Required(ErrorMessage:="Please fill the date")> _
    <DisplayFormat(DataFormatString:="{0:yyyy/MM/dd}", ApplyFormatInEditMode:=True)> _
    Public Property TDate As String
End Class
