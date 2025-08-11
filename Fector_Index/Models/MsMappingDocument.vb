Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class MsMappingDocument
    <Key> _
    <Column(order:=1)> _
    <ForeignKey("DocumentTransaction")> _
    Public Property DocumentTransId As Integer
    Public Overridable Property DocumentTransaction As MsDocumentTransaction

    <Key> _
    <Column(order:=2)> _
    <ForeignKey("DocumentLHBU")> _
    Public Property DocumentLHBUId As String
    Public Overridable Property DocumentLHBU As MsDocumentLHBU

    Public Shared Function GetMapping(ByVal DocumentTransId As String, ByVal db As FectorEntities) As List(Of MsMappingDocument)
        Return db.MappingDocument.Where(Function(f) f.DocumentTransId = DocumentTransId).ToList
    End Function
End Class


Public Class MsMappingDocumentPurpose
    <Key> _
    <Column(order:=1)> _
    <ForeignKey("DocumentTransaction")> _
    Public Property DocumentTransId As Integer
    Public Overridable Property DocumentTransaction As MsDocumentTransaction

    <Key> _
    <Column(order:=2)> _
    <ForeignKey("Purpose")> _
    Public Property PurposeLHBUId As String
    Public Overridable Property Purpose As MsPurpose

    Public Shared Function GetMapping(ByVal DocumentTransId As String, ByVal db As FectorEntities) As List(Of MsMappingDocumentPurpose)
        Return db.MappingDocumentPurpose.Where(Function(f) f.DocumentTransId = DocumentTransId).ToList
    End Function
End Class
