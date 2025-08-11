Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel

Public Class MsCountry
    <Key> _
    Public Property CountryId As String

    <DisplayName("Country")> _
    Public Property Description As String

    Public Function isDomestic() As Boolean
        If CountryId = "ID" Then
            Return True
        Else
            Return False
        End If
    End Function

End Class
