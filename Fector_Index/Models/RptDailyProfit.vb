Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class RptDailyProfit
    <DisplayName("Period")> _
    <Required(ErrorMessage:="Please fill period")> _
    <DisplayFormat(DataFormatString:="{0:yyyy/MM}", ApplyFormatInEditMode:=True)> _
    Public Property Period As String
End Class
