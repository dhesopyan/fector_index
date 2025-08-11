Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class MsDocumentTransaction
    <Key> _
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)> _
    Public Property DocumentId As Integer

    <DisplayName("Description")> _
    <StringLength(100)> _
    <Required(ErrorMessage:="Please fill description")> _
    Public Property Description As String

    <DisplayName("Document Type")> _
    <StringLength(100)> _
    <Required(ErrorMessage:="Please select document type")> _
    Public Property DocumentType As String

    <DisplayName("Customer Type")> _
    <StringLength(100)> _
    <Required(ErrorMessage:="Please select customer type")> _
    Public Property CustomerType As String

    Public Property EditBy As String
    Public Property EditDate As DateTime
    Public Property ApproveBy As String
    Public Property ApproveDate As DateTime
    Public Property Status As String

    Public Shared Function GetDocumentTransaction(ByVal DocumentId As Integer, ByVal db As FectorEntities) As MsDocumentTransaction
        Return db.DocumentTransaction.Where(Function(f) f.DocumentId = DocumentId And f.Status = "ACTIVE").SingleOrDefault
    End Function

    Public Shared Function GetAllDocumentTransaction(ByVal DocumentId As Integer, ByVal db As FectorEntities) As MsDocumentTransaction
        Return db.DocumentTransaction.Where(Function(f) f.DocumentId = DocumentId).SingleOrDefault
    End Function

    Public ReadOnly Property DocumentTransactionDisplay As String
        Get
            Return String.Format("{0} - {1}", DocumentId, Description)
        End Get
    End Property
End Class
