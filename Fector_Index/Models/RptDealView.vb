Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class RptDealView
    <DisplayName("Period")> _
    <Required(ErrorMessage:="Please fill start period")> _
    <DisplayFormat(DataFormatString:="{0:yyyy/MM/dd}", ApplyFormatInEditMode:=True)> _
    Public Property StartPeriod As String

    <Required(ErrorMessage:="Please fill end period")> _
    <DisplayFormat(DataFormatString:="{0:yyyy/MM/dd}", ApplyFormatInEditMode:=True)> _
    Public Property EndPeriod As String
End Class
