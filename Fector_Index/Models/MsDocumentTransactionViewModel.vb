Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class MsDocumentTransactionViewModel
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

    <DisplayName("LHBU Purpose")> _
    <Required(ErrorMessage:="Please select LHBU purposes")> _
    Public Property SelectedLHBUPurposes As String()
    Public Property LHBUPurposes As List(Of MsPurpose)

    <DisplayName("LHBU Documents")> _
    <Required(ErrorMessage:="Please select LHBU documents")> _
    Public Property SelectedLHBUDocuments As String()
    Public Property LHBUDocuments As List(Of MsDocumentLHBU)

    Public Property EditBy As String
    Public Property EditDate As DateTime
    Public Property ApproveBy As String
    Public Property ApproveDate As DateTime
    Public Property Status As String

    Public Shared Function GetDocumentTransaction(ByVal DocumentId As Integer, ByVal db As FectorEntities) As MsDocumentTransactionViewModel
        Dim DocumentTransaction = db.DocumentTransaction.Where(Function(f) f.DocumentId = DocumentId).SingleOrDefault
        Dim LHBUDocs = (From mp In db.MappingDocument
                       Join lhbu In db.DocumentLHBU On mp.DocumentLHBUId Equals lhbu.DocumentId
                       Where mp.DocumentTransId = DocumentId
                       Select lhbu).ToList

        Dim Purposes = (From mp In db.MappingDocumentPurpose
               Join pur In db.Purposes On mp.PurposeLHBUId Equals pur.PurposeId
               Where mp.DocumentTransId = DocumentId
               Select pur).ToList

        Dim mdl As New MsDocumentTransactionViewModel
        With mdl
            .DocumentId = DocumentTransaction.DocumentId
            .Description = DocumentTransaction.Description
            .DocumentType = DocumentTransaction.DocumentType
            .CustomerType = DocumentTransaction.CustomerType
            .EditBy = DocumentTransaction.EditBy
            .EditDate = DocumentTransaction.EditDate
            .ApproveBy = DocumentTransaction.ApproveBy
            .ApproveDate = DocumentTransaction.ApproveDate
            .Status = DocumentTransaction.Status
            .LHBUDocuments = LHBUDocs
            Dim strLHBU(LHBUDocs.Count) As String
            Dim i As Integer = 0
            For Each lhbu As MsDocumentLHBU In LHBUDocs
                strLHBU(i) = lhbu.DocumentId
                i += 1
            Next
            mdl.SelectedLHBUDocuments = strLHBU
            Dim strPurpose(Purposes.Count) As String
            i = 0
            For Each pur As MsPurpose In Purposes
                strPurpose(i) = pur.PurposeId
                i += 1
            Next
            mdl.SelectedLHBUPurposes = strPurpose
        End With

        Return mdl
    End Function

End Class
