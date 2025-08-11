Imports System.ComponentModel.DataAnnotations

Public Class MsBranch
    <Key> _
    Public Property BranchId As String

    Public Property Name As String
    Public Property BICode As String
    Public Property BranchAbbr As String

    Public ReadOnly Property BranchDisplay As String
        Get
            Return String.Format("{0} - {1}", BranchId, Name)
        End Get
    End Property


End Class
