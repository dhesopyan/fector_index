Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class MsNostro
    <Key> _
    Public Property BIC As String

    Public Property nostro As String
    Public Property LLD As Integer
    Public Property lastchange As DateTime
    Public Property lastchangeby As String
    Public Property lastapprove As DateTime
    Public Property lastapproveby As String
    Public Property flagstatus As Integer
End Class
