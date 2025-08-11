Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class MsSubjectStatus
    <Key> _
    Public Property StatusId As String

    Public Property Description As String
    Public Property FlagActive As Boolean

    Public ReadOnly Property DisplayValue As String
        Get
            Return StatusId & " - " & Description
        End Get
    End Property

End Class

