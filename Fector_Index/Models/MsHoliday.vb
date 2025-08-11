Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel

Public Class MsHoliday
    <Key> _
    Public Property HolidayId As Integer

    Public Property HolidayDesc As String
    Public Property StartDate As DateTime
    Public Property EndDate As DateTime
    Public Property Nostro As String
End Class
