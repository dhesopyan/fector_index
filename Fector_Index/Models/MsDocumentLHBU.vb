Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class MsDocumentLHBU
    <Key> _
    <DisplayName("Document ID")> _
    <StringLength(20)> _
    <Required(ErrorMessage:="Please fill document id")> _
    Public Property DocumentId As String

    <DisplayName("Description")> _
    <StringLength(100)> _
    <Required(ErrorMessage:="Please fill description")> _
    Public Property Description As String


    Public Property EditBy As String
    Public Property EditDate As DateTime
    Public Property ApproveBy As String
    Public Property ApproveDate As DateTime
    Public Property Status As String

    Public Shared Function GetDocumentLHBU(ByVal DocumentId As String, ByVal db As FectorEntities) As MsDocumentLHBU
        Return db.DocumentLHBU.Where(Function(f) f.DocumentId = DocumentId And f.Status = "ACTIVE").SingleOrDefault
    End Function

    Public Shared Function GetAllDocumentLHBU(ByVal DocumentId As String, ByVal db As FectorEntities) As MsDocumentLHBU
        Return db.DocumentLHBU.Where(Function(f) f.DocumentId = DocumentId).SingleOrDefault
    End Function

    Public ReadOnly Property DocumentLHBUDisplay As String
        Get
            Return String.Format("{0} - {1}", DocumentId, Description)
        End Get
    End Property

End Class
